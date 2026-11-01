using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Template.Scripts.Utils
{
    public class TextWithFormat : MonoBehaviour
    {
        [SerializeField] private TextProvider _format;
        [Space]
        [SerializeField] private bool _useDefaultArgument;
        [SerializeField] private TextProvider _defaultArgument;
        [Space]
        [SerializeField] private TextMeshProUGUI _text;

        private object[] _argumentsOverride;
        private string _formatOverride;
        
        public TextMeshProUGUI Text => _text;

        private void OnEnable()
        {
            if (_format.ProviderType == ProviderType.Localized)
            {
                LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChanged;
            }

            if (_useDefaultArgument)
            {
                UpdateTextResult();
            }
        }

        private void OnDisable()
        {
            if (_format.ProviderType == ProviderType.Localized)
            {
                LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;
            }
        }

        public void SetArgs(params object[] args)
        {
            _argumentsOverride = args;
            
            UpdateTextResult();
        }

        public void Refresh()
        {
            _formatOverride = null;
            _argumentsOverride = null;
            
            UpdateTextResult();
        }
        
        public void OverrideFormat(string format)
        {
            _formatOverride = format;
            
            UpdateTextResult();
        }

        private void UpdateTextResult()
        {
            string format = GetFormat();
            object[] args = GetArguments();
            
            _text.text = string.Format(format, args);
        }

        private void OnSelectedLocaleChanged(Locale locale)
        {
            UpdateTextResult();
        }

        private string GetFormat()
        {
            return _formatOverride ?? _format.GetText();
        }

        private object[] GetArguments()
        {
            return _argumentsOverride ?? new object[] { _defaultArgument.GetText() };
        }

        [Serializable]
        private class TextProvider
        {
            [SerializeField] private ProviderType _providerType = ProviderType.Simple;
            [SerializeField, TextArea] private string _simpleText = "{0}";
            [SerializeField] private LocalizedString _localizedText;

            public ProviderType ProviderType => _providerType;

            public string GetText()
            {
                return _providerType switch
                {
                    ProviderType.Simple => _simpleText,
                    ProviderType.Localized => _localizedText.GetLocalizedString(),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        private enum ProviderType
        {
            Simple = 0,
            Localized = 1
        }
    }
}