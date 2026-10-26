using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Template.Features.DataStorage;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Template.Features.Localization
{
    public class LocalizationService : ILocalizationService
    {
        private readonly ISaveLoadService _saveLoadService;

        private LocalizationUserData userData;

        public event Action LocaleChanged;

        public LocalizationService(ISaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
        }

        public async Task InitAsync()
        {
            await InitializeLocalization();

            await LoadOrCreateLocalizationUserData();

            Locale locale = GetLocaleByCode(userData.SelectedLocaleCode);

            if (locale != null)
            {
                LocalizationSettings.SelectedLocale = locale;
            }
        }

        public async UniTask SetLocale(Locale locale)
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

            await SaveLocale(locale.Identifier.Code);

            LocaleChanged?.Invoke();
        }

        public Locale GetSelectedLocale()
        {
            return LocalizationSettings.SelectedLocale;
        }

        private async UniTask SaveLocale(string localeCode)
        {
            if (userData == null)
            {
                await LoadOrCreateLocalizationUserData();
            }

            userData.SelectedLocaleCode = localeCode;
            _saveLoadService.Save(userData);
        }
        
        private async UniTask LoadOrCreateLocalizationUserData()
        {
            var (sucess, data) = await _saveLoadService.TryLoad<LocalizationUserData>();
            if (sucess)
            {
                userData = data;
            }
            else
            {
                userData = new LocalizationUserData();
            }
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