using System;
using DG.Tweening;
using UnityEngine;

namespace Persistance.Features.LevelProgression
{
    [Serializable]
    public class TilesStaggerSettings
    {
        public bool Enabled = true;
 
        [Min(0f)] public float StartDelay = 0.1f;
 
        [Min(0f)] public float Step = 0.05f;
 
        [Min(0f)] public float MaxTotalStagger = 0.6f;
 
        [Min(0.01f)] public float TileDuration = 0.25f;
        public Ease TileEase = Ease.OutBack;
 
        [Range(0f, 1f)] public float StartScale = 0.7f;
    }
}