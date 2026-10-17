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
        private ScreenConfig _config;

        private ActivatableScreen<TData> _screen;

        // Запись конфига ищем один раз, а не при каждой активации.
        private ScreenEntryBase _entry;

        [Inject]
        private void Construct(IScreenFactory screenFactory, ILocalConfigService configService)
        {
            _screenFactory = screenFactory;
            _config = configService.GetConfig<ScreenConfig>();
        }

        protected override async UniTask Activate(TData data)
        {
            if (_screen == null)
            {
                _entry ??= ResolveEntry();

                // Экран создается под этим объектом: так он попадает в иерархию текущей сцены.
                _screen = await _screenFactory.Create<ActivatableScreen<TData>>(_entry, transform);
            }

            await _screen.Activate(data);
        }

        protected override async UniTask Open(NavigationItemOpenPayload payload)
        {
            if (_screen == null)
                return;

            _screen.SetVisible(true);
            await _screen.Open(payload);
        }

        protected override async UniTask Close(NavigationItemClosePayload payload)
        {
            if (_screen == null)
                return;

            // Обнуляем сразу: повторный Close или Activate не должен работать с освобождаемым экраном.
            ActivatableScreen<TData> screen = _screen;
            _screen = null;

            try
            {
                await screen.Close(payload);
            }
            finally
            {
                // Даже если анимация закрытия упала, экран должен вернуться в фабрику.
                screen.SetVisible(false);
                _screenFactory.Release(screen);
            }
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Объект уничтожается (например, при выгрузке сцены): возвращаем экран, чтобы не потерять ассет.
            // Если экран создан под этим объектом, лучше не включать для него Pooled.
            if (_screen != null)
            {
                _screenFactory.Release(_screen);
                _screen = null;
            }
        }

        protected virtual ScreenType GetScreenTypeForItem(Type itemType)
        {
            ScreenTypeAttribute attribute = itemType.GetCustomAttribute<ScreenTypeAttribute>();
            if (attribute != null)
                return attribute.Type;

            throw new InvalidOperationException($"No ScreenType defined for {itemType.Name}");
        }

        private ScreenEntryBase ResolveEntry()
        {
            ScreenType screenType = GetScreenTypeForItem(typeof(TItem));

            if (!_config.TryGetEntry(screenType, out ScreenEntryBase entry))
                throw new InvalidOperationException(
                    $"No screen configured for ScreenType.{screenType} (item {typeof(TItem).Name}).");

            return entry;
        }

    }
}