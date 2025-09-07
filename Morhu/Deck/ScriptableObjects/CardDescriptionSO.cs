using Morhu.Infrustructure.Services.Localization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Morhu.Deck.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Card Description", menuName = "Deck/CardDescription")]
    public class CardDescriptionSO : ScriptableObject
    {
        public List<CardDescriptionLocalizedData> CardDescriptionLocalizedDatas = new();

        private void OnValidate()
        {
            if (CardDescriptionLocalizedDatas.Count == 0) return;

            var usedLanguages = new HashSet<Language>();

            foreach (var data in CardDescriptionLocalizedDatas)
            {
                if (data == null) continue;

                if (!usedLanguages.Add(data.Language))
                    throw new Exception($"Duplicate language found: {data.Language} in CardDescriptionLocalizedDatas.");
            }
        }

        public CardDescriptionData GetDataByLanguage(Language Language)
        {
            foreach (var localizedData in CardDescriptionLocalizedDatas)
                if (localizedData.Language == Language)
                    return localizedData.Data;

            return null;
        }
    }

    [Serializable]
    public class CardDescriptionLocalizedData
    {
        public Language Language;
        public CardDescriptionData Data;
    }

    [Serializable]
    public class CardDescriptionData
    {
        public string Name;
        public string AbilityDescription;
        public string Aditional;
    }
}
