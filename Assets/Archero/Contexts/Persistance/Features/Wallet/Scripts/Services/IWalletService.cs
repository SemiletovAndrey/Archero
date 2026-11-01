using System.Collections.Generic;
using Contexts.Scripts.Utilities.Reactive;
using Persistance.Features.User.Features.JsonUser;
using Template.Features.DataStorage;

namespace Persistance.Features.Wallet
{
    public interface IWalletService : IDataReader<PlayerData>, IDataWriter<PlayerData>
    {
        public List<CurrencyTypes> AvailableCurrencies { get; }
        public IReadOnlyVariable<int> GetCurrency(CurrencyTypes type);
        
        public bool EnoughCurrency(CurrencyTypes type, int amount);
        public void Add(CurrencyTypes type, int amount);
        public void SpendCurrency(CurrencyTypes type, int amount);
    }
}