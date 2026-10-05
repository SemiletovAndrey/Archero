using System;
using System.Threading.Tasks;
using UnityEngine.Localization;

namespace Template.Features.Localization
{
    public interface ILocalizationService
    {
        public Task InitAsync();

        public event Action LocaleChanged;

        public Locale GetSelectedLocale();
        public void SetLocale(Locale locale);
    }
}