using Cysharp.Threading.Tasks;

namespace Template.Features.UILoader
{
    public interface ILoaderView
    {
        /// <summary>Минимальное время, которое загрузка остается видимой после показа: так нет мигания на быстрых загрузках.</summary>
        public float MinVisibleTime { get; }

        /// <summary>Плавно показывает загрузку. К моменту завершения она закрывает UI и блокирует ввод.</summary>
        public UniTask Show();

        /// <summary>Плавно скрывает загрузку и возвращает ввод.</summary>
        public UniTask Hide();

        public void SetProgress(float progress);

        public void SetMessage(string message);

    }
}