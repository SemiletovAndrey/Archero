using System.Collections.Generic;
using Contexts.Scripts.Utilities.Reactive;

namespace Persistance.Features.Wallet
{
    public interface IWalletService
    {
        public List<CurrencyTypes> AvailableCurrencies { get; }
        public IReadOnlyVariable<int> GetCurrency(CurrencyTypes type);
        
        public bool EnoughCurrency(CurrencyTypes type, int amount);
        public void Add(CurrencyTypes type, int amount);
        public void SpendCurrency(CurrencyTypes type, int amount);
    }
}