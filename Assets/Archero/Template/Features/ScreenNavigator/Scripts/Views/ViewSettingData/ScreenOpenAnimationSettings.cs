using System;
using DG.Tweening;
using UnityEngine;

namespace Template.Features.ScreenNavigator
{
    [Serializable]
    public class ScreenOpenAnimationSettings
    {
        public bool Enabled = true;

        [Min(0f)] public float Duration = 0.25f;

        public bool Fade = true;
        public Ease FadeEase = Ease.OutQuad;

        public bool Scale = true;
        [Range(0.5f, 1f)] public float StartScale = 0.9f;
        public Ease ScaleEase = Ease.OutBack;
        
        public bool IgnoreTimeScale = true;
    }
}