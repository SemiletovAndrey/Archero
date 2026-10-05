using Template.Scripts.Views;
using Zenject;

namespace Template.Features.ScreenNavigator
{
    public abstract class BaseNavigationButton<TItem, TData> : UIButton
        where TItem : BaseNavigationItem<TData>
        where TData : BaseNavigationItemPayload
    {
        private IScreenNavigator _screenNavigator;
        
        [Inject]
        private void Construct(IScreenNavigator screenNavigator)
        {
            _screenNavigator = screenNavigator;
        }
        
        protected override void OnClick()
        {
            _screenNavigator.NavigateTo<TItem, TData>(GetData());
        }

        protected abstract TData GetData();
    }
}