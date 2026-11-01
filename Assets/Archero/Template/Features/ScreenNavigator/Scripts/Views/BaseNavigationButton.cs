using Cysharp.Threading.Tasks;
using Template.Features.UIExtensions;
using Zenject;

namespace Template.Features.ScreenNavigator
{
    public abstract class BaseNavigationButton<TItem, TData> : UIButton
        where TItem : BaseNavigationItem<TData>
        where TData : BaseNavigationItemPayload
    {
        private IScreenNavigator _screenNavigator;
        
        protected virtual bool IgnoreWhileNavigating => true;

        [Inject]
        private void Construct(IScreenNavigator screenNavigator)
        {
            _screenNavigator = screenNavigator;
        }

        protected override void OnClick()
        {
            if (IgnoreWhileNavigating && _screenNavigator.IsNavigating)
                return;

            _screenNavigator.NavigateTo<TItem, TData>(GetData()).Forget();
        }

        protected abstract TData GetData();
    }
}