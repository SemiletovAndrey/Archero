using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Template.Features.UIExtensions
{
    public class IconTextView : MonoBehaviour, IView
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _image;

        public void SetText(string text)
        {
            _text.text = text;
        }

        public void SetIcon(Sprite sprite)
        {
            _image.sprite = sprite;
        }
    }
}