using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Template.Features.ScreenNavigator
{
    public abstract class AnimatedActivatableScreen<TData> : ActivatableScreen<TData>
        where TData : BaseNavigationItemPayload
    {
        [Header("Open animation")] [SerializeField]
        private Transform _animationTarget;

        [SerializeField] private ScreenOpenAnimationSettings _openAnimation = new ScreenOpenAnimationSettings();

        [Header("Close animation")] [SerializeField]
        private ScreenCloseAnimationSettings _closeAnimation = new ScreenCloseAnimationSettings();

        private bool _animationRefsResolved;

        private CanvasGroup _rootGroup;
        private CanvasGroup _targetGroup;

        private Vector3 _baseScale;
        private float _baseAlpha;
        private bool _baseBlocksRaycasts;

        private Tween _tween;
        private bool _tweenIsClosing;
        private bool _inputBlocked;

        private bool IsPlaying => _tween != null && _tween.IsActive();

        public bool IsOpenAnimationPlaying => IsPlaying && !_tweenIsClosing;
        public bool IsCloseAnimationPlaying => IsPlaying && _tweenIsClosing;

        protected ScreenOpenAnimationSettings OpenAnimationSettings => _openAnimation;
        protected ScreenCloseAnimationSettings CloseAnimationSettings => _closeAnimation;

        protected Transform AnimationTarget =>
            _animationTarget != null ? _animationTarget : transform;

        protected CanvasGroup AnimationGroup => _targetGroup;
        protected Vector3 BaseScale => _baseScale;
        protected float BaseAlpha => _baseAlpha;

        public override UniTask Open(NavigationItemOpenPayload payload)
        {
            ResolveAnimationRefs();

            RestoreVisualState();
            PlayAnimation(false, this.GetCancellationTokenOnDestroy()).Forget();

            return UniTask.CompletedTask;
            //return PlayAnimation(false, this.GetCancellationTokenOnDestroy());
        }

        public override async UniTask Close(NavigationItemClosePayload payload)
        {
            await PlayAnimation(true, this.GetCancellationTokenOnDestroy());
            await base.Close(payload);
        }

        public void SetInputBlocked(bool blocked)
        {
            ResolveAnimationRefs();

            _inputBlocked = blocked;
            ApplyRaycasts();
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
                    DOTween.To(
                            () => _targetGroup.alpha,
                            value => _targetGroup.alpha = value,
                            _baseAlpha,
                            settings.Duration)
                        .SetEase(settings.FadeEase));
            }

            if (settings.Scale)
            {
                AnimationTarget.localScale = _baseScale * settings.StartScale;

                sequence.Join(
                    AnimationTarget
                        .DOScale(_baseScale, settings.Duration)
                        .SetEase(settings.ScaleEase));
            }

            return sequence;
        }

        protected virtual Tween BuildCloseAnimation()
        {
            ScreenCloseAnimationSettings settings = _closeAnimation;

            if (!settings.Fade && !settings.Scale)
                return null;

            Sequence sequence = DOTween.Sequence();

            if (settings.Fade)
            {
                sequence.Join(
                    DOTween.To(
                            () => _targetGroup.alpha,
                            value => _targetGroup.alpha = value,
                            0f,
                            settings.Duration)
                        .SetEase(settings.FadeEase));
            }

            if (settings.Scale)
            {
                sequence.Join(
                    AnimationTarget
                        .DOScale(_baseScale * settings.EndScale, settings.Duration)
                        .SetEase(settings.ScaleEase));
            }

            return sequence;
        }

        private async UniTask PlayAnimation(
            bool closing,
            CancellationToken cancellationToken)
        {
            StopTween();

            bool enabled = closing
                ? _closeAnimation.Enabled && _closeAnimation.Duration > 0f
                : _openAnimation.Enabled && _openAnimation.Duration > 0f;

            if (!enabled)
                return;

            ResolveAnimationRefs();

            Tween tween = closing
                ? BuildCloseAnimation()
                : BuildOpenAnimation();

            if (tween == null)
                return;

            UniTaskCompletionSource completion = new();

            bool finished = false;

            void Finish()
            {
                if (finished)
                    return;

                finished = true;

                if (_tween == tween)
                {
                    _tween = null;
                    _tweenIsClosing = false;
                }

                completion.TrySetResult();
            }

            bool ignoreTimeScale = closing
                ? _closeAnimation.IgnoreTimeScale
                : _openAnimation.IgnoreTimeScale;

            tween.SetAutoKill(true)
                .SetUpdate(ignoreTimeScale)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                .OnComplete(Finish)
                .OnKill(Finish);

            _tween = tween;
            _tweenIsClosing = closing;

            using (cancellationToken.Register(StopTween))
            {
                await completion.Task;
            }
        }

        private void StopTween()
        {
            Tween tween = _tween;
            _tween = null;
            _tweenIsClosing = false;

            if (tween != null && tween.IsActive())
                tween.Kill();
        }

        private void RestoreVisualState()
        {
            if (!_animationRefsResolved)
                return;

            _targetGroup.alpha = _baseAlpha;
            AnimationTarget.localScale = _baseScale;

            ApplyRaycasts();
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

            ApplyRaycasts();
        }

        private void ApplyRaycasts()
        {
            if (_rootGroup != null)
            {
                _rootGroup.blocksRaycasts =
                    _baseBlocksRaycasts && !_inputBlocked;
            }
        }

        private static CanvasGroup GetOrAddCanvasGroup(GameObject target)
        {
            return target.TryGetComponent(out CanvasGroup group)
                ? group
                : target.AddComponent<CanvasGroup>();
        }

        private void OnDestroy()
        {
            StopTween();
        }
    }
}