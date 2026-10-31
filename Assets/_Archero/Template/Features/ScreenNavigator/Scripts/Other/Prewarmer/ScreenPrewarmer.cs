using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using Template.Features.Config.Local;
using UnityEngine;

namespace Template.Features.ScreenNavigator
{
    public class ScreenPrewarmer : IScreenPrewarmer
    {
        private readonly IScreenFactory _screenFactory;
        private readonly IScreenNavigator _navigator;
        private readonly ILocalConfigService _configService;

        public ScreenPrewarmer(IScreenFactory screenFactory, IScreenNavigator navigator,
            ILocalConfigService configService)
        {
            _screenFactory = screenFactory;
            _navigator = navigator;
            _configService = configService;
        }

        public async UniTask Prewarm(IEnumerable<ScreenType> screens, CancellationToken cancellationToken = default)
        {
            GeneralScreenConfigs config = _configService.GetConfig<GeneralScreenConfigs>();

            foreach (ScreenType screenType in screens)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await UniTask.WaitUntil(() => !_navigator.IsNavigating, cancellationToken: cancellationToken);

                try
                {
                    if (!config.TryGetScreenConfig(screenType, out ScreenEntryBase entry))
                        continue;

                    if (!entry.Pooled)
                    {
                        Debug.LogWarning($"[{nameof(ScreenPrewarmer)}] ScreenType.{screenType} is not pooled, " +
                                         "so there is nowhere to keep a prewarmed instance. Skipped.");
                        continue;
                    }

                    await _screenFactory.Prewarm(entry, cancellationToken);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                await UniTask.NextFrame(cancellationToken);
            }
        }

        public UniTask PrewarmChildrenOf(BaseNavigationItem item, CancellationToken cancellationToken = default)
        {
            List<ScreenType> screens = new();

            foreach (BaseNavigationItem child in item.Children)
            {
                ScreenTypeAttribute attribute = child.GetType().GetCustomAttribute<ScreenTypeAttribute>(inherit: false);

                if (attribute != null && attribute.Type != ScreenType.None)
                    screens.Add(attribute.Type);
            }

            return Prewarm(screens, cancellationToken);
        }
    }
}