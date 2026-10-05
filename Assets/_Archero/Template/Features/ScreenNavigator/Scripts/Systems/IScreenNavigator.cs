using System;
using Cysharp.Threading.Tasks;

namespace Template.Features.ScreenNavigator
{
    public interface IScreenNavigator
    {
        public UniTask NavigateTo<TItem, TPayload>(TPayload payload)
            where TItem : BaseNavigationItem<TPayload>
            where TPayload : BaseNavigationItemPayload;

        public UniTask NavigateTo<TItem>() 
            where TItem : BaseNavigationItem<EmptyNavigationItemPayload>;
    }
}