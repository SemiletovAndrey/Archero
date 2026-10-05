using Cysharp.Threading.Tasks;

namespace Template.Features.ScreenNavigator
{
    public interface INavigationCommand
    {
        public UniTask GetCommandAwaiter();
        public UniTask Execute(ScreenNavigator screenNavigator);
    }
}