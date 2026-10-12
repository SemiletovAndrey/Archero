using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace Template.Features.Localization
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizedTextField : MonoBehaviour
    {
        [SerializeField] private LocalizedString _localizedString;

        private TextMeshProUGUI _text;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            Subscribe();
            Refresh();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        public void Refresh()
        {
            if (_text == null)
            {
                return;
            }

            string value = _localizedString.GetLocalizedString();
            ApplyText(value);
        }

        private void Subscribe()
        {
            if (_localizedString != null)
            {
                _localizedString.StringChanged += UpdateText;
            }

            LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChanged;
        }

        private void Unsubscribe()
        {
            if (_localizedString != null)
            {
                _localizedString.StringChanged -= UpdateText;
            }

            LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;
        }

        private void OnSelectedLocaleChanged(Locale locale)
        {
            Refresh();
        }

        private void UpdateText(string value)
        {
            if (_text == null)
            {
                return;
            }

            ApplyText(value);
        }

        private void ApplyText(string value)
        {
            _text.text = value;
        }

        private bool HasValidEntryReference(TableEntryReference entryReference)
        {
            return !string.IsNullOrWhiteSpace(entryReference.Key) || entryReference.KeyId != 0;
        }
    }
}