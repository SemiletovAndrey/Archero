using System;
using Contexts.Scripts.Utilities.Reactive;
using Template.Features.UIExtensions;

namespace Persistance.Features.Wallet
{
    public class CurrencyPresenter
    {
        private readonly IReadOnlyVariable<int> _currency;
        private readonly CurrencyTypes _currencyType;
        private readonly CurrencyIconsConfig _currencyIconsConfig;

        private IconTextView _currencyTextView;

        private IDisposable _disposable;

        public CurrencyPresenter(IReadOnlyVariable<int> currency, CurrencyTypes currencyType,
            CurrencyIconsConfig currencyIconsConfig, IconTextView currencyTextView)
        {
            _currency = currency;
            _currencyType = currencyType;
            _currencyIconsConfig = currencyIconsConfig;
            _currencyTextView = currencyTextView;
        }

        public void Enable()
        {
            UpdateTextView(_currency.Value);
            _currencyTextView.SetIcon(_currencyIconsConfig.GetSpriteFor(_currencyType));

            _disposable = _currency.Subscribe(OnCurrencyUpdate);
        }

        public void Disable()
        {
            _disposable.Dispose();
        }
        
        private void OnCurrencyUpdate(int arg1, int newValue)
        {
            UpdateTextView(newValue);
        }

        private void UpdateTextView(int value)
        {
            _currencyTextView.SetText(value.ToString());
        }
    }
}