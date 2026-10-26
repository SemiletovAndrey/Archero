using System;
using System.Collections.Generic;
using System.Linq;
using Template.Features.Config.Local;
using UnityEngine;

namespace Persistance.Features.Wallet
{
    [CreateAssetMenu(fileName = "Config_Wallet_Start", menuName = "SO/Persistence/Wallet/StartWalletConfig")]
    public class StartWalletConfig : BaseLocalFeatureConfig
    {
        [SerializeField] private List<CurrencyConfig> _currencies;
        
        public int GetValueFor(CurrencyTypes currencyType) => _currencies.First(config => config.CurrencyType == currencyType).Value;
        
        [Serializable]
        private class CurrencyConfig
        {
            [field: SerializeField] public CurrencyTypes CurrencyType { get; private set; }
            [field: SerializeField] public int Value { get; private set; }
        }
    }
}