using System;
using System.Collections.Generic;
using System.Linq;
using Contexts.Scripts.Utilities.Reactive;
using Persistance.Features.User.Features.JsonUser;
using Template.Features.DataStorage;
using UnityEngine;

namespace Persistance.Features.Wallet
{
    public class WalletService : IWalletService, IDisposable
    {
        private readonly Dictionary<CurrencyTypes, ReactiveVariable<int>> _currencies;
        private IDataProvider<PlayerData> _playerDataProvider;

        public WalletService(IDataProvider<PlayerData> playerDataProvider)
        {
            Dictionary<CurrencyTypes, ReactiveVariable<int>> currencies =
                new Dictionary<CurrencyTypes, ReactiveVariable<int>>();

            foreach (CurrencyTypes currencyTypes in Enum.GetValues(typeof(CurrencyTypes)))
            {
                currencies[currencyTypes] = new ReactiveVariable<int>();
            }

            _currencies = currencies;

            _playerDataProvider = playerDataProvider;
        }

        public void Init()
        {
            _playerDataProvider.RegisterWriter(this);
            _playerDataProvider.RegisterReader(this);
        }

        public void Dispose()
        {
            _playerDataProvider.UnRegisterWriter(this);
            _playerDataProvider.UnRegisterReader(this);
        }

        public List<CurrencyTypes> AvailableCurrencies => _currencies.Keys.ToList();

        public IReadOnlyVariable<int> GetCurrency(CurrencyTypes type) => _currencies[type];

        public bool EnoughCurrency(CurrencyTypes type, int amount)
        {
            if (amount < 0)
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

            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            _currencies[type].Value -= amount;
        }

        public void ReadFrom(PlayerData data)
        {
            foreach (var currency in data.WalletData)
            {
                if (_currencies.ContainsKey(currency.Key))
                {
                    _currencies[currency.Key].Value = currency.Value;
                }
                else
                {
                    _currencies.Add(currency.Key, new ReactiveVariable<int>(currency.Value));
                }
            }
        }

        public void WriteTo(PlayerData data)
        {
            foreach (var currency in _currencies)
            {
                if (_currencies.ContainsKey(currency.Key))
                {
                    data.WalletData[currency.Key] = currency.Value.Value;
                }
                else
                {
                    data.WalletData.Add(currency.Key, currency.Value.Value);
                }
            }
        }
    }
}