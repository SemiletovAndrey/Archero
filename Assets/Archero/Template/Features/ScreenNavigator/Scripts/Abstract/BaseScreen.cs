using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Template.Features.ScreenNavigator
{
    public abstract class BaseScreen : MonoBehaviour
    {
        private Canvas _canvas;

        public ScreenType ScreenType { get; private set; }

        internal void Init(ScreenType screenType)
        {
            ScreenType = screenType;
        }
        
        public void SetVisible(bool visible)
        {
            if (_canvas == null)
                _canvas = GetComponent<Canvas>();

            if (_canvas != null)
                _canvas.enabled = visible;
        }
    }
}