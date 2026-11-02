using System;
using DG.Tweening;
using UnityEngine;

namespace Template.Features.Popups
{
    [Serializable]
    public class PopupBodyAnimationSettings : PopupTweenSettings
    {
        [Tooltip("Масштаб тела в скрытом состоянии (относительно масштаба в префабе).\n" +
                 "0 - появление из точки. Значения > 0 дают резкое появление, т.к. тело не фейдится.")]
        [Range(0f, 1f)]
        public float HiddenScale = 0f;

        public PopupBodyAnimationSettings()
        {
            ShowEase = Ease.OutBack;
            HideEase = Ease.InBack;
        }
    }
}