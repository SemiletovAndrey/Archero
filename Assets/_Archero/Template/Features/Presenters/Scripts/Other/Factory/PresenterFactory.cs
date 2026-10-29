using System;
using Zenject;

namespace Template.Features.Presenters
{
    public class PresenterFactory : IPresenterFactory
    {
        private readonly DiContainer _container;

        public PresenterFactory(DiContainer container)
        {
            _container = container;
        }

        public TPresenter Create<TPresenter, TView>(TView view, Action<DiContainer> bindExtra = null)
            where TPresenter : class, IDisposable
            where TView : class
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            // Временный дочерний контейнер: видит всё, что видит _container, плюс View и доп. привязки.
            // Благодаря этому презентеру не нужно использовать все переданные объекты (в отличие от extra args).
            DiContainer sub = _container.CreateSubContainer();
            sub.Bind<TView>().FromInstance(view);
            bindExtra?.Invoke(sub);

            return sub.Instantiate<TPresenter>();
        }

    }
}