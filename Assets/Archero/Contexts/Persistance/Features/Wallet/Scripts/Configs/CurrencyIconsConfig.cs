using System;
using System.Collections.Generic;
using System.Linq;
using Template.Features.Config.Local;
using UnityEngine;

namespace Persistance.Features.Wallet
{
    [CreateAssetMenu(fileName = "Config_Wallet_CurrencyIcons", menuName = "SO/Persistence/Wallet/CurrencyIconsConfig")]
    public class CurrencyIconsConfig : BaseLocalFeatureConfig
    {
        [SerializeField] private List<CurrencyConfig> _configs;
        
        public Sprite GetSpriteFor(CurrencyTypes currencyType) => _configs.First(config => config.CurrencyType == currencyType).Sprite;
        
        [Serializable]
        private class CurrencyConfig
        {
            [field: SerializeField] public CurrencyTypes CurrencyType { get; private set; }
            [field: SerializeField] public Sprite Sprite { get; private set; }
        }
    }
}