using System;
using Zenject;

namespace Template.Features.Presenters
{
    public interface IPresenterFactory
    {
        /// <summary>
        /// Создаёт презентер. View регистрируется под типом TView (интерфейс, который запрашивает конструктор презентера).
        /// Через bindExtra можно добавить ещё привязки только для этого презентера (например, контроллер попапа).
        /// Все остальные зависимости берутся из контейнера, в котором зарегистрирована фабрика.
        /// </summary>
        TPresenter Create<TPresenter, TView>(TView view, Action<DiContainer> bindExtra = null)
            where TPresenter : class, IDisposable
            where TView : class;

    }
}