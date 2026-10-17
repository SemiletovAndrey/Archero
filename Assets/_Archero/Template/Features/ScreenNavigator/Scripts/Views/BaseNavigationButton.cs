using Cysharp.Threading.Tasks;
using Template.Scripts.Views;
using Zenject;

namespace Template.Features.ScreenNavigator
{
    public abstract class BaseNavigationButton<TItem, TData> : UIButton
        where TItem : BaseNavigationItem<TData>
        where TData : BaseNavigationItemPayload
    {
        private IScreenNavigator _screenNavigator;

        /// <summary>
        /// Пока идет переход, повторные нажатия игнорируются: иначе они встали бы в очередь и повторили навигацию.
        /// </summary>
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

            // Forget: ошибка навигации попадет в лог UniTask, а не потеряется.
            _screenNavigator.NavigateTo<TItem, TData>(GetData()).Forget();
        }

        protected abstract TData GetData();
    }
}