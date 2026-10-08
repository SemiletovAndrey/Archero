using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Template.Features.ProjectInit;
using UnityEngine;

namespace Template.Features.GameFlow.Scripts.Other.FeatureInitializer
{
    public class FeatureInitializer : IFeatureInitializer
    {
        public async UniTask InitAll(BaseFeatureSet featureSet, CancellationToken ct)
        {
            foreach (IInitializableFeature feature in featureSet.Features)
            {
                ct.ThrowIfCancellationRequested();

                if (!featureSet.TryMarkStarted(feature))
                {
                    continue;
                }

                try
                {
                    // У InitFeature() нет токена, поэтому AttachExternalCancellation лишь перестаёт ЖДАТЬ
                    // при отмене; сама инициализация фичи продолжит работу.
                    await feature.InitFeature().AttachExternalCancellation(ct);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);

                    if (feature is ICriticalFeature)
                    {
                        throw new InvalidOperationException(
                            $"Critical feature '{feature.FeatureName}' failed to initialize.", exception);
                    }
                }
            }
        }

        public async UniTask DeinitAll(BaseFeatureSet featureSet)
        {
            IReadOnlyList<IInitializableFeature> started = featureSet.Started;

            for (int i = started.Count - 1; i >= 0; i--)
            {
                if (started[i] is not IDeinitializableFeature feature)
                {
                    continue;
                }

                try
                {
                    await feature.DeinitFeature();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            featureSet.ClearStarted();
        }
    }
}