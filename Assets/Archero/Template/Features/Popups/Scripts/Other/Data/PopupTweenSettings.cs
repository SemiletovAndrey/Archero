using System;
using DG.Tweening;
using UnityEngine;

namespace Template.Features.Popups
{
    [Serializable]
    public class PopupTweenSettings
    {
        public bool Enabled = true;
 
        [Min(0f)] public float ShowDuration = 0.25f;
        [Min(0f)] public float HideDuration = 0.15f;
 
        public Ease ShowEase = Ease.OutQuad;
        public Ease HideEase = Ease.InQuad;
    }
}