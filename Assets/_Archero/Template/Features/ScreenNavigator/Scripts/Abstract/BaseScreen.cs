using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Template.Features.ScreenNavigator
{
    public abstract class BaseScreen : MonoBehaviour
    {
        private Canvas _canvas;

        /// <summary>Выставляется фабрикой при создании.</summary>
        public ScreenType ScreenType { get; private set; }

        internal void Init(ScreenType screenType)
        {
            ScreenType = screenType;
        }

        /// <summary>
        /// Включает и выключает отрисовку экрана (Canvas.enabled). Экран создается невидимым,
        /// чтобы не показывать устаревшие данные, пока идет Activate.
        /// </summary>
        public void SetVisible(bool visible)
        {
            if (_canvas == null)
                _canvas = GetComponent<Canvas>();

            if (_canvas != null)
                _canvas.enabled = visible;
        }

    }
}