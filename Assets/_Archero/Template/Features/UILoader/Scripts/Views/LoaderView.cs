using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Template.Features.UILoader
{
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup), typeof(GraphicRaycaster))]
    public class LoaderView : MonoBehaviour, ILoaderView
    {
        private const float MaxFadeStep = 1f / 30f;

        [Tooltip("Выше диапазонов UILayerType (до 299), чтобы загрузка закрывала все.")] [SerializeField]
        private int _sortingOrder = 1000;

        [SerializeField] private float _fadeDuration = 0.2f;
        [SerializeField] private float _minVisibleTime = 0.3f;

        [Tooltip("Необязательно. Показывается только после первого SetProgress.")] [SerializeField]
        private Slider _progressBar;

        [Tooltip("Необязательно. Показывается, когда задано сообщение.")] [SerializeField]
        private TMP_Text _message;

        private Canvas _canvas;
        private CanvasGroup _group;

        private int _animationVersion;

        public float MinVisibleTime => _minVisibleTime;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _group = GetComponent<CanvasGroup>();

            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.overrideSorting = true;
            _canvas.sortingOrder = _sortingOrder;

            ApplyHidden();
        }

        public async UniTask Show()
        {
            int version = BeginShow();

            await Fade(1f, version);
        }

        public UniTask ShowWithoutAnimation()
        {
            BeginShow();

            _group.alpha = 1f;

            return UniTask.CompletedTask;
        }

        public async UniTask Hide()
        {
            if (!_canvas.enabled)
                return;

            int version = ++_animationVersion;

            await Fade(0f, version);

            if (version != _animationVersion)
                return;

            ApplyHidden();
            ResetContent();
        }

        public void SetProgress(float progress)
        {
            if (_progressBar == null)
                return;

            _progressBar.gameObject.SetActive(true);
            _progressBar.value = Mathf.Clamp01(progress);
        }

        public void SetMessage(string message)
        {
            if (_message == null)
                return;

            _message.text = message ?? string.Empty;
            _message.gameObject.SetActive(!string.IsNullOrEmpty(message));
        }

        private int BeginShow()
        {
            int version = ++_animationVersion;

            _canvas.enabled = true;
            _group.blocksRaycasts = true;

            return version;
        }

        private void ApplyHidden()
        {
            _group.alpha = 0f;
            _group.blocksRaycasts = false;
            _canvas.enabled = false;
        }

        private void ResetContent()
        {
            if (_progressBar != null)
                _progressBar.gameObject.SetActive(false);

            if (_message != null)
            {
                _message.text = string.Empty;
                _message.gameObject.SetActive(false);
            }
        }

        private async UniTask Fade(float target, int version)
        {
            float start = _group.alpha;

            float duration = _fadeDuration * Mathf.Abs(target - start);
            float time = 0f;

            while (time < duration)
            {
                if (version != _animationVersion || _group == null)
                    return;

                time += Mathf.Min(Time.unscaledDeltaTime, MaxFadeStep);
                _group.alpha = Mathf.Lerp(start, target, time / duration);

                await UniTask.Yield();
            }

            if (version == _animationVersion && _group != null)
                _group.alpha = target;
        }
    }
}