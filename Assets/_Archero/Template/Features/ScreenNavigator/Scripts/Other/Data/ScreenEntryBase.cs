using System;

namespace Template.Features.ScreenNavigator
{
    [Serializable]
    public abstract class ScreenEntryBase
    {
        public ScreenType Type;

        /// <summary>
        /// true - при уходе с экрана экземпляр не уничтожается, а прячется и используется при следующем показе
        /// (быстрый возврат, нет Instantiate, DI и Awake). Экран должен сам сбрасывать состояние в Activate.
        /// </summary>
        public bool Pooled = true;

        /// <summary>Назначен ли источник префаба.</summary>
        public abstract bool HasSource { get; }

    }
}