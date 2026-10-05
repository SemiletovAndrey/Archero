using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Template.Features.Config.Local;

namespace Template.Features.Popups
{
    public class PopupSystem : IPopupSystem
    {
        private readonly IPopupFactory _factory;
        private readonly ILocalConfigService _localConfigService;

        private Dictionary<Type, PopupType> _typeToPopupType = new();
        private Dictionary<PopupType, BasePopup> _activePopups = new();

        private PopupConfig _config;

        public PopupSystem(ILocalConfigService localConfigService, IPopupFactory factory)
        {
            _localConfigService = localConfigService;
            _factory = factory;

            BuildTypeMap();
        }

        public void Init()
        {
            _config = GetConfig();
        }

        public UniTask Open(PopupType popupType)
        {
            return OpenInternal<BasePopup, EmptyPopupPayload>(popupType, default);
        }

        public UniTask<TPopup> Open<TPopup>() where TPopup : BasePopup<EmptyPopupPayload>
        {
            if (!_typeToPopupType.TryGetValue(typeof(TPopup), out var popupType))
                throw new Exception($"No PopupType registered for {typeof(TPopup).Name}");
            return OpenInternal<TPopup, EmptyPopupPayload>(popupType, default);
        }

        public UniTask<TPopup> Open<TPopup, TPayload>(TPayload payload)
            where TPopup : BasePopup<TPayload>
            where TPayload : IPopupPayload
        {
            if (!_typeToPopupType.TryGetValue(typeof(TPopup), out var popupType))
                throw new Exception($"No PopupType registered for {typeof(TPopup).Name}");
            return OpenInternal<TPopup, TPayload>(popupType, payload);
        }

        private async UniTask<TPopup> OpenInternal<TPopup, TPayload>(PopupType popupType, TPayload payload)
            where TPopup : BasePopup
            where TPayload : IPopupPayload
        {
            if (_activePopups.TryGetValue(popupType, out var existing))
            {
                if (existing is BasePopup<TPayload> typed && typeof(TPayload) != typeof(EmptyPopupPayload))
                    typed.Activate(payload);
                return existing as TPopup;
            }

            var entry = _config.Entries.FirstOrDefault(e => e.Type == popupType);
            if (entry == null || entry.Prefab == null)
                throw new Exception($"No prefab configured for PopupType.{popupType}");

            TPopup popup = await _factory.Create<TPopup>(entry.Prefab);
            popup.PopupType = popupType;
            popup.SetCloseHandler(HandleCloseExternal);

            if (popup is BasePopup<TPayload> typedPopup && typeof(TPayload) != typeof(EmptyPopupPayload))
                await typedPopup.Activate(payload);

            _activePopups[popupType] = popup;

            await popup.Show();
            return popup;
        }

        public UniTask Close(PopupType popupType)
        {
            if (!_activePopups.TryGetValue(popupType, out var popup))
                return UniTask.CompletedTask;

            return ClosePopupInstance(popupType, popup);
        }

        public UniTask Close<TPopup>() where TPopup : BasePopup
        {
            if (!_typeToPopupType.TryGetValue(typeof(TPopup), out var popupType))
                return UniTask.CompletedTask;

            return Close(popupType);
        }

        private async UniTask ClosePopupInstance(PopupType popupType, BasePopup popup)
        {
            _activePopups.Remove(popupType);
            await popup.Deactivate();
            await popup.Hide();
            _factory.Destroy(popup);
        }
        
        private void HandleCloseExternal(PopupType popupType)
        {
            Close(popupType).Forget();
        }

        private void BuildTypeMap()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (assembly.FullName.StartsWith("System") ||
                    assembly.FullName.StartsWith("Unity") ||
                    assembly.FullName.StartsWith("Mono.") ||
                    assembly.FullName.StartsWith("mscorlib"))
                    continue;

                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException)
                {
                    continue;
                }

                foreach (var type in types)
                {
                    if (!typeof(BasePopup).IsAssignableFrom(type) || type.IsAbstract)
                        continue;

                    var attr = type.GetCustomAttribute<PopupTypeAttribute>();
                    if (attr != null)
                    {
                        _typeToPopupType[type] = attr.Type;
                    }
                }
            }
        }

        private PopupConfig GetConfig()
        {
            return _config ??= _localConfigService.GetConfig<PopupConfig>();
        }
    }
}