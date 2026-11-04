using System;
using System.Collections.Generic;
using Template.Features.Config.Local;
using UnityEngine;

namespace Persistance.Features.Wallet
{
    public class WalletPresenter : IDisposable
    {
        private readonly List<CurrencyPresenter> _currencyPresenters = new();

        public WalletPresenter(IWalletView view, IWalletService walletService, ILocalConfigService localConfigService)
        {
            CurrencyIconsConfig currencyConfig = localConfigService.GetConfig<CurrencyIconsConfig>();

            foreach (CurrencyIconTextView currencyView in view.CurrencyIconTextViews)
            {
                CurrencyPresenter currencyPresenter = new CurrencyPresenter(
                    walletService.GetCurrency(currencyView.CurrencyType), currencyView.CurrencyType, currencyConfig,
                    currencyView);
                _currencyPresenters.Add(currencyPresenter);

                currencyPresenter.Enable();
            }
        }

        public void Dispose()
        {
            foreach (CurrencyPresenter presenter in _currencyPresenters)
                presenter.Dispose();

            _currencyPresenters.Clear();
        }
    }
}