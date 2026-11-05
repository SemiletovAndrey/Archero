using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Template.Features.ScreenNavigator;
using Template.Features.UIExtensions;
using TMPro;
using UnityEngine;

namespace Persistance.Features.LevelProgression
{
    public class ChoiceLevelScreenView : AnimatedActivatableScreen<ChoiceLevelPayload>, IChoiceLevelScreenView
    {
        private struct TileState
        {
            public Transform Transform;
            public CanvasGroup Group;
            public Vector3 BaseScale;
        }

        [SerializeField] private UIButton _closeButton;
        [SerializeField] private UIButton _backgroundCloseButton;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private LevelTilesListView _levelTilesListView;
 
        [Header("Tiles animation")]
        [SerializeField] private TilesStaggerSettings _tilesAnimation = new();
 
        private readonly List<TileState> _animatedTiles = new();
        
        public event Action BackClicked;
        
        public LevelTilesListView LevelTilesListView => _levelTilesListView;
 
        public override UniTask Activate(ChoiceLevelPayload data)
        {
            _closeButton.AssignAction(BackButton);
            _backgroundCloseButton.AssignAction(BackButton);
            return UniTask.CompletedTask;
        }

        public void SetTitle(string title) => _titleText.text = title;
        
        protected override Tween BuildOpenAnimation()
        {
            Sequence sequence = DOTween.Sequence();
            bool hasTweens = false;
 
            Tween screenTween = base.BuildOpenAnimation();
            if (screenTween != null)
            {
                sequence.Join(screenTween);
                hasTweens = true;
            }
 
            if (AppendTilesAnimation(sequence))
                hasTweens = true;
 
            if (hasTweens)
                return sequence;
 
            sequence.Kill();
            return null;
        }
 
        public override UniTask Close(NavigationItemClosePayload payload)
        {
            UniTask closing = base.Close(payload);
            RestoreTiles();
            return closing;
        }
        
        private void BackButton() => BackClicked?.Invoke();
 
        private bool AppendTilesAnimation(Sequence sequence)
        {
            TilesStaggerSettings s = _tilesAnimation;
            if (!s.Enabled || _levelTilesListView == null)
                return false;
 
            RestoreTiles();
 
            int count = _levelTilesListView.Elements.Count;
            if (count == 0)
                return false;
 
            float step = count > 1 ? Mathf.Min(s.Step, s.MaxTotalStagger / (count - 1)) : 0f;
 
            for (int i = 0; i < count; i++)
            {
                Transform tile = _levelTilesListView.Elements[i].transform;
                CanvasGroup group = tile.TryGetComponent(out CanvasGroup existing)
                    ? existing
                    : tile.gameObject.AddComponent<CanvasGroup>();
 
                Vector3 baseScale = tile.localScale;
                _animatedTiles.Add(new TileState { Transform = tile, Group = group, BaseScale = baseScale });
                
                group.alpha = 0f;
                tile.localScale = baseScale * s.StartScale;
 
                float at = s.StartDelay + step * i;
 
                sequence.Insert(at, DOTween.To(() => group.alpha, value => group.alpha = value, 1f, s.TileDuration));
                sequence.Insert(at, tile.DOScale(baseScale, s.TileDuration).SetEase(s.TileEase));
            }
            
            return true;
        }
 
        private void RestoreTiles()
        {
            for (int i = 0; i < _animatedTiles.Count; i++)
            {
                TileState state = _animatedTiles[i];
 
                if (state.Group != null)
                    state.Group.alpha = 1f;
 
                if (state.Transform != null)
                    state.Transform.localScale = state.BaseScale;
            }
 
            _animatedTiles.Clear();
        }
    }
}