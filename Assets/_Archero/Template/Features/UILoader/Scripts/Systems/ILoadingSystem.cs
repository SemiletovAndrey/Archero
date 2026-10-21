using System;
using Cysharp.Threading.Tasks;

namespace Template.Features.UILoader
{
    public interface ILoadingSystem
    {
        public bool IsVisible { get; }

        /// <summary>
        /// Показывает загрузку и завершается, когда она полностью видима. Возвращает хэндл: загрузка скроется,
        /// когда будут отпущены все выданные хэндлы. Отпускайте хэндл в finally или через using.
        /// </summary>
        public UniTask<ILoadingHandle> Show(string message = null);

        /// <summary>Показывает загрузку на время работы и гарантированно отпускает ее, даже если работа упала.</summary>
        public UniTask Run(Func<UniTask> work, string message = null);

        public UniTask<T> Run<T>(Func<UniTask<T>> work, string message = null);
    }

    /// <summary>Один запрос на показ загрузки.</summary>
    public interface ILoadingHandle : IDisposable
    {
        /// <summary>Прогресс 0..1. Загрузка у всех запросов общая, поэтому побеждает последнее значение.</summary>
        public void SetProgress(float progress);

        public void SetMessage(string message);

        /// <summary>
        /// Отпускает запрос и завершается, когда загрузка скрыта (или осталась показанной, потому что ее держит
        /// другой запрос). Повторный вызов ничего не делает. Dispose делает то же без ожидания.
        /// </summary>
        public UniTask Hide();

    }
}