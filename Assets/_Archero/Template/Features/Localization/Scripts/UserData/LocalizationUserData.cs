using Newtonsoft.Json;
using Template.Features.DataStorage;
using Template.Scripts.Attributes;
using UnityEngine.Localization.Settings;

namespace Template.Features.Localization
{
    [FormerName("Data-LocalizationUserModel")]
    public class LocalizationUserData: ISaveData
    {
        public string SelectedLocaleCode { get; set; }
        
        [JsonConstructor]
        public LocalizationUserData(string selectedLocaleCode)
        {
            SelectedLocaleCode = selectedLocaleCode;
        }

        public LocalizationUserData()
        {
            SelectedLocaleCode = LocalizationSettings.SelectedLocale.Identifier.Code;
        }
    }
}