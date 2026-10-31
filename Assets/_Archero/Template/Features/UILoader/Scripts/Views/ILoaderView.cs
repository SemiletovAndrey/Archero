using Cysharp.Threading.Tasks;

namespace Template.Features.UILoader
{
    public interface ILoaderView
    {
        public float MinVisibleTime { get; }

        public UniTask Show();
        public UniTask ShowWithoutAnimation();

        public UniTask Hide();

        public void SetProgress(float progress);

        public void SetMessage(string message);

    }
}