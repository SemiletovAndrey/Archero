using System;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Template.Features.Config.Local;
using UnityEngine;
using Zenject;

namespace Template.Features.ScreenNavigator
{
    public class DynamicNavigationScreenActivator<TItem, TData> : NavigationItemActivator<TItem, TData>
        where TItem : BaseNavigationItem<TData>
        where TData : BaseNavigationItemPayload
    {
        private IScreenFactory _screenFactory;
        private ILocalConfigService _configService;
        
        private ScreenConfig _config;

        private ActivatableScreen<TData> _screen;
        private ScreenType _screenType;
        private GameObject _prefab;

        [Inject]
        private void Construct(IScreenFactory screenFactory, ILocalConfigService configService)
        {
            _screenFactory = screenFactory;
            _configService = configService;
            
            _config = _configService.GetConfig<ScreenConfig>();
        }

        protected override async UniTask Activate(TData data)
        {
            if (_screen == null)
            {
                var entry = _config.Entries.First(e => e.Type == GetScreenTypeForItem(typeof(TItem)));
                _screenType = entry.Type;
                _prefab = entry.Prefab;

                _screen = await _screenFactory.Create<ActivatableScreen<TData>>(_screenType, _prefab, transform);
            }
            await _screen.Activate(data);
        }

        protected override async UniTask Open(NavigationItemOpenPayload payload)
        {
            if (_screen != null)
                await _screen.Open(payload);
        }

        protected override async UniTask Close(NavigationItemClosePayload payload)
        {
            if (_screen != null)
            {
                await _screen.Close(payload);
                _screenFactory.Destroy(_screen, _screenType);
                _screen = null;
            }
        }
        
        protected virtual ScreenType GetScreenTypeForItem(Type itemType)
        {
            var attr = itemType.GetCustomAttribute<ScreenTypeAttribute>();
            if (attr != null) return attr.Type;
            throw new Exception($"No ScreenType defined for {itemType.Name}");
        }
    }
}