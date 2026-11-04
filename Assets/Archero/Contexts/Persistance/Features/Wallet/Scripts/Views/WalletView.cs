using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Persistance.Features.Wallet
{
    public class WalletView : MonoBehaviour, IWalletView
    {
        [SerializeField] private List<CurrencyIconTextView> _currencyIconTextViews;
        
        public IReadOnlyList<CurrencyIconTextView> CurrencyIconTextViews => _currencyIconTextViews;

        public bool TryGetCurrencyIconTextView(CurrencyTypes currencyTypes, out CurrencyIconTextView currencyIconTextView)
        {
            currencyIconTextView = _currencyIconTextViews.FirstOrDefault(item => item.CurrencyType == currencyTypes);
            return currencyIconTextView != null;
        }
    }
}