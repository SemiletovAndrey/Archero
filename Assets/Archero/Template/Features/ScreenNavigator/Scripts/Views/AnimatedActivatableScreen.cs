using System;
using System.Threading;
using Cysharp.Threading.Tasks;
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


    public abstract class AnimatedActivatableScreen<TData> : ActivatableScreen<TData>
        where TData : BaseNavigationItemPayload
    {
        [Header("Open animation")]
        [Tooltip("Что анимируется. Если не задано - корень экрана.\n" +
                 "Для полноэкранных экранов укажите контейнер с содержимым БЕЗ фона: иначе при появлении " +
                 "из прозрачности сквозь экран будет видна камера (скайбокс).")]
        [SerializeField]
        private Transform _animationTarget;

        [SerializeField] private ScreenOpenAnimationSettings _openAnimation = new ScreenOpenAnimationSettings();

        private bool _animationRefsResolved;
        private CanvasGroup _rootGroup;
        private CanvasGroup _targetGroup;
        private Vector3 _baseScale;
        private float _baseAlpha;
        private bool _baseBlocksRaycasts;

        private Tween _openTween;

        public bool IsOpenAnimationPlaying => _openTween != null && _openTween.IsActive();

        protected ScreenOpenAnimationSettings OpenAnimationSettings => _openAnimation;
        protected Transform AnimationTarget => _animationTarget != null ? _animationTarget : transform;
        protected CanvasGroup AnimationGroup => _targetGroup;
        protected Vector3 BaseScale => _baseScale;
        protected float BaseAlpha => _baseAlpha;

        public override UniTask Open(NavigationItemOpenPayload payload)
        {
            return PlayOpenAnimation(this.GetCancellationTokenOnDestroy());
        }

        public override UniTask Close(NavigationItemClosePayload payload)
        {
            StopOpenAnimation();
            return base.Close(payload);
        }
        
        protected virtual Tween BuildOpenAnimation()
        {
            ScreenOpenAnimationSettings settings = _openAnimation;
            if (!settings.Fade && !settings.Scale)
                return null;

            Sequence sequence = DOTween.Sequence();

            if (settings.Fade)
            {
                _targetGroup.alpha = 0f;

                sequence.Join(
                    DOTween.To(() => _targetGroup.alpha, value => _targetGroup.alpha = value,
                            _baseAlpha, settings.Duration)
                        .SetEase(settings.FadeEase));
            }

            if (settings.Scale)
            {
                AnimationTarget.localScale = _baseScale * settings.StartScale;

                sequence.Join(AnimationTarget.DOScale(_baseScale, settings.Duration).SetEase(settings.ScaleEase));
            }

            return sequence;
        }

        private async UniTask PlayOpenAnimation(CancellationToken cancellationToken)
        {
            StopOpenAnimation();

            if (!_openAnimation.Enabled || _openAnimation.Duration <= 0f)
                return;

            ResolveAnimationRefs();
            
            _rootGroup.blocksRaycasts = false;

            Tween tween;
            try
            {
                tween = BuildOpenAnimation();
            }
            catch
            {
                _rootGroup.blocksRaycasts = _baseBlocksRaycasts;
                throw;
            }

            if (tween == null)
            {
                _rootGroup.blocksRaycasts = _baseBlocksRaycasts;
                return;
            }

            UniTaskCompletionSource completion = new UniTaskCompletionSource();
            
            void Finish()
            {
                if (_rootGroup != null)
                    _rootGroup.blocksRaycasts = _baseBlocksRaycasts;

                completion.TrySetResult();
            }

            tween.SetAutoKill(true)
                .SetUpdate(_openAnimation.IgnoreTimeScale)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                .OnComplete(Finish)
                .OnKill(Finish);

            _openTween = tween;

            using (cancellationToken.Register(StopOpenAnimation))
            {
                await completion.Task;
            }
        }

        private void StopOpenAnimation()
        {
            Tween tween = _openTween;
            _openTween = null;

            if (tween != null && tween.IsActive())
                tween.Kill();
        }

        private void ResolveAnimationRefs()
        {
            if (_animationRefsResolved)
                return;

            _rootGroup = GetOrAddCanvasGroup(gameObject);
            _targetGroup = GetOrAddCanvasGroup(AnimationTarget.gameObject);
            
            _baseScale = AnimationTarget.localScale;
            _baseAlpha = _targetGroup.alpha;
            _baseBlocksRaycasts = _rootGroup.blocksRaycasts;

            _animationRefsResolved = true;
        }

        private static CanvasGroup GetOrAddCanvasGroup(GameObject target)
        {
            return target.TryGetComponent(out CanvasGroup group) ? group : target.AddComponent<CanvasGroup>();
        }
    }
}