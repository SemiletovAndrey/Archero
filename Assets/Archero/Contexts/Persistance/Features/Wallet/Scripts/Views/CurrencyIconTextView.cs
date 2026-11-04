using Template.Features.UIExtensions;
using UnityEngine;

namespace Persistance.Features.Wallet
{
    public class CurrencyIconTextView : IconTextView, ICurrencyView
    {
        [field: SerializeField] public CurrencyTypes  CurrencyType { get; private set; }
    }
}