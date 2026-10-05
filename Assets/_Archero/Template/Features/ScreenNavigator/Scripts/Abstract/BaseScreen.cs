using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Template.Features.ScreenNavigator
{
    public abstract class BaseScreen : MonoBehaviour
    {
        public abstract ScreenType ScreenType { get; protected set; }

        public void Init(ScreenType screenType)
        {
            ScreenType = screenType;
        }
    }
}