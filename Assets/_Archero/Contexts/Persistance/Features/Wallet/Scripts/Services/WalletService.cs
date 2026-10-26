using System;
using System.Collections.Generic;
using System.Linq;
using Contexts.Scripts.Utilities.Reactive;

namespace Persistance.Features.Wallet
{
    public class WalletService : IWalletService
    {
        private readonly Dictionary<CurrencyTypes, ReactiveVariable<int>> _currencies;

        public WalletService()
        {
            Dictionary<CurrencyTypes, ReactiveVariable<int>>  currencies = new Dictionary<CurrencyTypes, ReactiveVariable<int>>();

            foreach (CurrencyTypes currencyTypes in Enum.GetValues(typeof(CurrencyTypes)))
            {
                currencies[currencyTypes] = new ReactiveVariable<int>();
            }

            _currencies = currencies;
        }

        public List<CurrencyTypes> AvailableCurrencies => _currencies.Keys.ToList();
        
        public IReadOnlyVariable<int> GetCurrency(CurrencyTypes type) => _currencies[type];

        public bool EnoughCurrency(CurrencyTypes type, int amount)
        {
            if(amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));
            
            return _currencies[type].Value >= amount;
        }

        public void Add(CurrencyTypes type, int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));
            
            _currencies[type].Value += amount;
        }
        
        public void SpendCurrency(CurrencyTypes type, int amount)
        {
            if (EnoughCurrency(type, amount))
                throw new InvalidOperationException($"Not enough: {type.ToString()}");
            
            if(amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));
            
            _currencies[type].Value -= amount;
        }
    }
}