using System;
using System.Collections.Generic;
using Persistance.Features.Wallet;
using Template.Features.Config.Local;
using Template.Features.DataStorage;

namespace Persistance.Features.User.Features.JsonUser
{
    public class PlayerDataProvider : DataProvider<PlayerData>  
    {
        private readonly ILocalConfigService _localConfigService;
        
        public PlayerDataProvider(ISaveLoadService saveLoadService, ILocalConfigService localConfigService) : base(saveLoadService)
        {
            _localConfigService = localConfigService;
        }

        protected override PlayerData GetOriginData()
        {
            return new PlayerData()
            {
                WalletData = InitWalletData(),
            };
        }

        private Dictionary<CurrencyTypes, int> InitWalletData()
        {
            Dictionary<CurrencyTypes, int> walletData = new();
            StartWalletConfig config = _localConfigService.GetConfig<StartWalletConfig>();

            foreach (CurrencyTypes currencyType in Enum.GetValues(typeof(CurrencyTypes)))
            {
                if(currencyType == CurrencyTypes.None) continue;
                
                walletData[currencyType] = config.GetValueFor(currencyType);
            }
            
            return walletData;
        }
    }
}