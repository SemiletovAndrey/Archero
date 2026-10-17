using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Template.Features.ScreenNavigator
{
    public abstract class ActivatableScreen<TData> : BaseScreen where TData : BaseNavigationItemPayload
    {
        /// <summary>
        /// Принимает данные. Вызывается при открытии и при повторном выборе уже открытого экрана.
        /// Экраны кэшируются (ScreenEntryBase.Pooled), поэтому здесь же нужно сбрасывать состояние:
        /// Awake и DI выполняются один раз.
        /// </summary>
        public abstract UniTask Activate(TData data);

        /// <summary>Экран становится видимым: анимация появления.</summary>
        public virtual UniTask Open(NavigationItemOpenPayload payload)
        {
            return UniTask.CompletedTask;
        }

        /// <summary>Экран закрывается: анимация скрытия. После нее экран скрывается и возвращается в фабрику.</summary>
        public virtual UniTask Close(NavigationItemClosePayload payload)
        {
            return UniTask.CompletedTask;
        }
    }
}