using System;
using DG.Tweening;
using Template.Features.UIExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Persistance.Features.LevelProgression
{
    public class LevelTileView : MonoBehaviour, IView
    {
        public event Action Clicked;
        
        [SerializeField] private Image _background;
        [SerializeField] private TextMeshProUGUI _levelNumberText;
        [SerializeField] private UIButton _button;
        
        [SerializeField] private Color _activeColor;
        [SerializeField] private Color _completedColor;
        [SerializeField] private Color _blockedColor;

        private void OnEnable()
        {
            _button.AssignAction(OnClick);
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }

        public void SetLevel(string levelNumber) => _levelNumberText.text = levelNumber;
        
        public void SetBlock() => _background.color = _blockedColor;
        
        public void SetCompleted() => _background.color = _completedColor;
        
        public void SetActiveColor() => _background.color = _activeColor;

        public Tween Show()
        {
            transform.DOKill();
            
            return transform
                .DOScale(1, 0.1f)
                .From(0)
                .SetUpdate(true)
                .Play();
        }

        public Tween Hide()
        {
            transform.DOKill();

            return DOTween.Sequence();
        }
        
        private void OnClick()
        {
            Clicked?.Invoke();
        }
    }
}