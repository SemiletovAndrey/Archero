using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Template.Features.DataStorage;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Template.Features.Localization
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IDataStorageService _storageService;

        private LocalizationUserData userData;

        public event Action LocaleChanged;

        public LocalizationService(IDataStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task InitAsync()
        {
            await InitializeLocalization();

            userData = _storageService.LoadOrCreate(() => new LocalizationUserData());

            Locale locale = GetLocaleByCode(userData.SelectedLocaleCode);

            if (locale != null)
            {
                LocalizationSettings.SelectedLocale = locale;
            }
        }

        public void SetLocale(Locale locale)
        {
            if (locale == null)
            {
                return;
            }

            if (LocalizationSettings.SelectedLocale == locale)
            {
                return;
            }

            LocalizationSettings.SelectedLocale = locale;

            SaveLocale(locale.Identifier.Code);

            LocaleChanged?.Invoke();
        }

        public Locale GetSelectedLocale()
        {
            return LocalizationSettings.SelectedLocale;
        }

        private void SaveLocale(string localeCode)
        {
            if (userData == null)
            {
                userData = _storageService.LoadOrCreate(() => new LocalizationUserData());
            }

            userData.SelectedLocaleCode = localeCode;
            _storageService.Save(userData);
        }

        private Locale GetLocaleByCode(string localeCode)
        {
            if (string.IsNullOrWhiteSpace(localeCode))
            {
                return null;
            }

            return LocalizationSettings.AvailableLocales.Locales.Find(locale => locale.Identifier.Code == localeCode);
        }

        private async UniTask InitializeLocalization()
        {
            await LocalizationSettings.InitializationOperation.ToUniTask();
        }
    }
}