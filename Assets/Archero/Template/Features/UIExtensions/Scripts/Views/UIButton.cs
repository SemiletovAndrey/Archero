using System;
using UnityEngine;
using UnityEngine.UI;

namespace Template.Features.UIExtensions
{
    public class UIButton : MonoBehaviour
    {
        [field: SerializeField] public Button Button { get; private set; }
        
        private Action _onClick;

        public event Action BeforeClicked;
        public event Action AfterClicked;
        
        private void Awake()
        {
            Button.onClick.AddListener(OnButtonClicked);
        }
        
        public void SetAvailability(bool state)
        {
            Button.interactable = state;
        }
        
        public void AssignAction(Action newAction)
        {
            _onClick = newAction;
        }

        protected virtual void OnClick()
        {
            _onClick?.Invoke();
        }

        private void OnButtonClicked()
        {
            BeforeClicked?.Invoke();
            OnClick();
            AfterClicked?.Invoke();
            
            PlayButtonClickAnimation();
        }

        private void PlayButtonClickAnimation()
        {
            
        }
    }
}