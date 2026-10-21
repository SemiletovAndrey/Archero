using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Template.Features.UILoader
{
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup), typeof(GraphicRaycaster))]
    public class LoaderView : MonoBehaviour, ILoaderView
    {
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
            int version = ++_animationVersion;

            _canvas.enabled = true;

            // Ввод блокируем сразу, не дожидаясь конца анимации.
            _group.blocksRaycasts = true;

            await Fade(1f, version);
        }

        public async UniTask Hide()
        {
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

        private void ApplyHidden()
        {
            _group.alpha = 0f;
            _group.blocksRaycasts = false;
            _canvas.enabled = false;
        }

        // Следующий показ начинается с чистого состояния.
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
            float time = 0f;

            while (time < _fadeDuration)
            {
                if (version != _animationVersion)
                    return;

                time += Time.unscaledDeltaTime;
                _group.alpha = Mathf.Lerp(start, target, time / _fadeDuration);

                await UniTask.Yield();
            }

            if (version == _animationVersion)
                _group.alpha = target;
        }
    }
}