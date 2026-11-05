using System;
using Contexts.Scripts.Utilities.Reactive;
using Template.Features.Presenters;

namespace Persistance.Features.Wallet
{
    public class CurrencyPresenter : IDefaultClassPresenter
    {
        private readonly IReadOnlyVariable<int> _currency;
        private readonly CurrencyTypes _currencyType;
        private readonly CurrencyIconsConfig _currencyIconsConfig;

        private CurrencyIconTextView _currencyTextView;

        private IDisposable _disposable;

        public CurrencyPresenter(IReadOnlyVariable<int> currency, CurrencyTypes currencyType,
            CurrencyIconsConfig currencyIconsConfig, CurrencyIconTextView currencyTextView)
        {
            _currency = currency;
            _currencyType = currencyType;
            _currencyIconsConfig = currencyIconsConfig;
            _currencyTextView = currencyTextView;
        }

        public void Init()
        {
            UpdateTextView(_currency.Value);
            _currencyTextView.SetIcon(_currencyIconsConfig.GetSpriteFor(_currencyType));

            _disposable = _currency.Subscribe(OnCurrencyUpdate);
        }
        
        private void OnCurrencyUpdate(int arg1, int newValue)
        {
            UpdateTextView(newValue);
        }

        private void UpdateTextView(int value)
        {
            _currencyTextView.SetText(value.ToString());
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            _disposable = null;
        }
    }
}