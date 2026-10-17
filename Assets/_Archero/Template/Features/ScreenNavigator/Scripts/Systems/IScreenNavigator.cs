using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Template.Features.ScreenNavigator
{
    public interface IScreenNavigator
    {
        /// <summary>Активный путь изменился. Передается актуальный путь от корня до листа (только для чтения).</summary>
        public event Action<IReadOnlyList<BaseNavigationItem>> PathChanged;

        /// <summary>Навигация завершилась ошибкой. Тот же сбой получает и вызывающий через await.</summary>
        public event Action<Exception> NavigationFailed;

        /// <summary>Активный путь от корня до листа.</summary>
        public IReadOnlyList<BaseNavigationItem> CurrentPath { get; }

        /// <summary>Идет переход или в очереди есть еще переходы. Удобно, чтобы игнорировать ввод.</summary>
        public bool IsNavigating { get; }

        /// <summary>
        /// Ставит переход в очередь и завершается, когда он выполнен (или бросает исключение, если он не удался).
        /// Не вызывайте await на NavigateTo из обработчиков событий элементов (Opened, Closed, Activated):
        /// очередь ждет завершения обработчика, и вы получите взаимную блокировку. Используйте Forget().
        /// </summary>
        public UniTask NavigateTo<TItem, TPayload>(TPayload payload)
            where TItem : BaseNavigationItem<TPayload>
            where TPayload : BaseNavigationItemPayload;

        public UniTask NavigateTo<TItem>()
            where TItem : BaseNavigationItem<EmptyNavigationItemPayload>;

    }
}