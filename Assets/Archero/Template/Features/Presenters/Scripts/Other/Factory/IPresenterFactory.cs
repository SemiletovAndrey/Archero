using System;
using Zenject;

namespace Template.Features.Presenters
{
    public interface IPresenterFactory
    {
        public TPresenter Create<TPresenter, TView>(TView view, Action<DiContainer> bindExtra = null)
            where TPresenter : class, IDisposable
            where TView : class;

    }
}