using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Template.Features.ScreenNavigator
{
    public interface IScreenNavigator
    {
        public event Action<IReadOnlyList<BaseNavigationItem>> PathChanged;

        public event Action<Exception> NavigationFailed;

        public IReadOnlyList<BaseNavigationItem> CurrentPath { get; }

        public bool IsNavigating { get; }
        
        public UniTask NavigateTo<TItem, TPayload>(TPayload payload, CancellationToken cancellationToken = default)
            where TItem : BaseNavigationItem<TPayload>
            where TPayload : BaseNavigationItemPayload;

        public UniTask NavigateTo<TItem>(CancellationToken cancellationToken = default)
            where TItem : BaseNavigationItem<EmptyNavigationItemPayload>;

    }
}