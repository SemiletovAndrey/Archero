using System;
using DG.Tweening;
using UnityEngine;

namespace Template.Features.ScreenNavigator
{
    [Serializable]
    public class ScreenCloseAnimationSettings
    {
        public bool Enabled = true;
 
        [Min(0f)] public float Duration = 0.15f;
 
        public bool Fade = true;
        public Ease FadeEase = Ease.InQuad;
 
        public bool Scale = true;
        [Range(0.5f, 1f)] public float EndScale = 0.9f;
        public Ease ScaleEase = Ease.InQuad;
 
        public bool IgnoreTimeScale = true;
    }
}