using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Template.Features.Popups
{
    public abstract class AnimatedPopup<TPayload> : BasePopupT<TPayload> where TPayload : IPopupPayload
    {
        [Header("Animation targets")]
        [Tooltip("Фон-затемнение, который перехватывает клики. Анимируется только фейдом.")]
        [SerializeField]
        private Transform _animationBackground;
 
        [Tooltip("Содержимое попапа БЕЗ фона. Анимируется только скейлом.")]
        [SerializeField]
        private Transform _animationBody;
 
        [Header("Animation")]
        [SerializeField] private PopupTweenSettings _backgroundAnimation = new();
        [SerializeField] private PopupBodyAnimationSettings _bodyAnimation = new();
        [SerializeField] private bool _ignoreTimeScale = true;
 
        private bool _animationRefsResolved;
        private CanvasGroup _backgroundGroup;
        private CanvasGroup _bodyGroup;
        private float _backgroundBaseAlpha;
        private Vector3 _bodyBaseScale;
        private bool _bodyBaseBlocksRaycasts;
 
        private Tween _tween;
 
        public bool IsAnimationPlaying => _tween != null && _tween.IsActive();
 
        protected PopupTweenSettings BackgroundSettings => _backgroundAnimation;
        protected PopupBodyAnimationSettings BodySettings => _bodyAnimation;
        protected Transform AnimationBody => _animationBody;
        protected CanvasGroup BackgroundGroup => _backgroundGroup;
        protected CanvasGroup BodyGroup => _bodyGroup;
        protected float BackgroundBaseAlpha => _backgroundBaseAlpha;
        protected Vector3 BodyBaseScale => _bodyBaseScale;
 
        public override UniTask Show()
        {
            return PlayAnimation(true, this.GetCancellationTokenOnDestroy());
        }
 
        public override UniTask Hide()
        {
            return PlayAnimation(false, this.GetCancellationTokenOnDestroy());
        }

        internal override void NotifyOpening()
        {
            base.NotifyOpening();
 
            StopAnimation();
            ResolveAnimationRefs();
            ApplyBackgroundHiddenState();
            ApplyBodyHiddenState();
        }
 
        #region Overridable
 
        protected virtual Tween BuildShowAnimation()
        {
            return Combine(BuildBackgroundShow(), BuildBodyShow());
        }
 
        protected virtual Tween BuildHideAnimation()
        {
            return Combine(BuildBodyHide(), BuildBackgroundHide());
        }
 
        protected virtual Tween BuildBackgroundShow()
        {
            PopupTweenSettings s = _backgroundAnimation;
            if (!s.Enabled || _backgroundGroup == null)
                return null;
 
            _backgroundGroup.alpha = 0f;
 
            if (s.ShowDuration <= 0f)
            {
                _backgroundGroup.alpha = _backgroundBaseAlpha;
                return null;
            }
 
            return FadeTo(_backgroundGroup, _backgroundBaseAlpha, s.ShowDuration, s.ShowEase);
        }
 
        protected virtual Tween BuildBackgroundHide()
        {
            PopupTweenSettings s = _backgroundAnimation;
            if (!s.Enabled || _backgroundGroup == null)
                return null;
 
            if (s.HideDuration <= 0f)
            {
                _backgroundGroup.alpha = 0f;
                return null;
            }
 
            return FadeTo(_backgroundGroup, 0f, s.HideDuration, s.HideEase);
        }
 
        protected virtual Tween BuildBodyShow()
        {
            PopupBodyAnimationSettings s = _bodyAnimation;
            if (!s.Enabled || _animationBody == null)
                return null;
 
            _animationBody.localScale = _bodyBaseScale * s.HiddenScale;
 
            if (s.ShowDuration <= 0f)
            {
                _animationBody.localScale = _bodyBaseScale;
                return null;
            }
 
            return _animationBody.DOScale(_bodyBaseScale, s.ShowDuration).SetEase(s.ShowEase);
        }
 
        protected virtual Tween BuildBodyHide()
        {
            PopupBodyAnimationSettings s = _bodyAnimation;
            if (!s.Enabled || _animationBody == null)
                return null;
 
            Vector3 hidden = _bodyBaseScale * s.HiddenScale;
 
            if (s.HideDuration <= 0f)
            {
                _animationBody.localScale = hidden;
                return null;
            }
 
            return _animationBody.DOScale(hidden, s.HideDuration).SetEase(s.HideEase);
        }
 
        protected virtual void ApplyBackgroundHiddenState()
        {
            if (_backgroundAnimation.Enabled && _backgroundGroup != null)
                _backgroundGroup.alpha = 0f;
        }
 
        protected virtual void ApplyBodyHiddenState()
        {
            if (_bodyAnimation.Enabled && _animationBody != null)
                _animationBody.localScale = _bodyBaseScale * _bodyAnimation.HiddenScale;
        }
 
        #endregion
 
        protected static Tween FadeTo(CanvasGroup group, float target, float duration, Ease ease)
        {
            return DOTween.To(() => group.alpha, value => group.alpha = value, target, duration).SetEase(ease);
        }
 
        private async UniTask PlayAnimation(bool show, CancellationToken cancellationToken)
        {
            StopAnimation();
            ResolveAnimationRefs();
            
            SetBodyBlocksRaycasts(false);
 
            Tween tween;
            try
            {
                tween = show ? BuildShowAnimation() : BuildHideAnimation();
            }
            catch
            {
                SetBodyBlocksRaycasts(true);
                throw;
            }
 
            if (tween == null)
            {
                SetBodyBlocksRaycasts(true);
                return;
            }
 
            UniTaskCompletionSource completion = new UniTaskCompletionSource();
 
            void Finish()
            {
                SetBodyBlocksRaycasts(true);
                completion.TrySetResult();
            }
 
            tween.SetAutoKill(true)
                .SetUpdate(_ignoreTimeScale)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                .OnComplete(Finish)
                .OnKill(Finish);
 
            _tween = tween;
 
            using (cancellationToken.Register(StopAnimation))
            {
                await completion.Task;
            }
        }
 
        private void StopAnimation()
        {
            Tween tween = _tween;
            _tween = null;
 
            if (tween != null && tween.IsActive())
                tween.Kill();
        }
 
        private void SetBodyBlocksRaycasts(bool restore)
        {
            if (_bodyGroup != null)
                _bodyGroup.blocksRaycasts = restore && _bodyBaseBlocksRaycasts;
        }
 
        private void ResolveAnimationRefs()
        {
            if (_animationRefsResolved)
                return;
 
            if (_animationBackground != null)
            {
                _backgroundGroup = GetOrAddCanvasGroup(_animationBackground.gameObject);
                _backgroundBaseAlpha = _backgroundGroup.alpha;
            }
 
            if (_animationBody != null)
            {
                _bodyGroup = GetOrAddCanvasGroup(_animationBody.gameObject);
                _bodyBaseScale = _animationBody.localScale;
                _bodyBaseBlocksRaycasts = _bodyGroup.blocksRaycasts;
            }
 
            _animationRefsResolved = true;
        }
 
        private static Tween Combine(Tween first, Tween second)
        {
            if (first == null)
                return second;
 
            if (second == null)
                return first;
 
            return DOTween.Sequence().Join(first).Join(second);
        }
 
        private static CanvasGroup GetOrAddCanvasGroup(GameObject target)
        {
            return target.TryGetComponent(out CanvasGroup group) ? group : target.AddComponent<CanvasGroup>();
        }
    }
}