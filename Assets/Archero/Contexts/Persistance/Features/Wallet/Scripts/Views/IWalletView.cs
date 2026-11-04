using System.Collections.Generic;

namespace Persistance.Features.Wallet
{
    public interface IWalletView
    {
        public IReadOnlyList<CurrencyIconTextView> CurrencyIconTextViews { get; }
        public bool TryGetCurrencyIconTextView(CurrencyTypes currencyTypes, out CurrencyIconTextView currencyIconTextView);
    }
}