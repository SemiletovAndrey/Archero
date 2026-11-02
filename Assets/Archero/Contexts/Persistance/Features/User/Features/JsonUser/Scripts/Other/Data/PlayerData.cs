using System.Collections.Generic;
using Persistance.Features.Wallet;
using Template.Features.DataStorage;

namespace Persistance.Features.User.Features.JsonUser
{
    public class PlayerData : ISaveData
    {
        public Dictionary<CurrencyTypes, int> WalletData;
        public List<int> CompletedLevels;

    }
}