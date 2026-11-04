using UnityEngine;

namespace Persistance.Features.Wallet
{
    public interface ICurrencyView
    {
        public CurrencyTypes CurrencyType { get; }

        public void SetIcon(Sprite sprite);
        public void SetText(string text);
    }
}