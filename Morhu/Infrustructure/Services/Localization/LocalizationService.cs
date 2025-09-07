using Morhu.Infrustructure.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Morhu.Infrustructure.Services.Localization
{
    public enum Language { En, Uk }

    public class LocalizationService : ILocalizationService
    {
        private readonly List<LanguageID> _languageIDs;
        private readonly IPersistanceDataService _persistanceDataService;

        public LocalizationService(IPersistanceDataService persistanceDataService)
        {
            _persistanceDataService = persistanceDataService;
            _languageIDs = new List<LanguageID>();
        }

        public void ChangeLanguage(Language newLanguage)
        {
            _persistanceDataService.Data.Language = newLanguage;
            SetUpLanguageForAll(newLanguage);
        }

        public void UpdateLanguageToCurrent() =>
            SetUpLanguageForAll(_persistanceDataService.Data.Language);

        public void FindAllObjectWithLanguageIDInTheScene()
        {
            var languageIDs = Object.FindObjectsByType<LanguageID>(FindObjectsSortMode.None); //bad

            _languageIDs.Clear();
            _languageIDs.AddRange(languageIDs);
        }

        private void SetUpLanguageForAll(Language newLanguage)
        {
            foreach (var languageID in _languageIDs)
                languageID.UpdateText(newLanguage);
        }
    }
}
