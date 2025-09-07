using Morhu.Deck.ScriptableObjects;
using Morhu.Infrustructure.Services.Localization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Morhu.Items.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Item Description", menuName = "Item/ItemDescription")]
    public class ItemDescriptionSO : ScriptableObject
    {
        public List<ItemDescriptionLocalizedData> ItemDescriptionLocalizedDatas = new();

        private void OnValidate()
        {
            if (ItemDescriptionLocalizedDatas.Count == 0) return;

            var usedLanguages = new HashSet<Language>();

            foreach (var data in ItemDescriptionLocalizedDatas)
            {
                if (data == null) continue;

                if (!usedLanguages.Add(data.Language))
                    throw new Exception($"Duplicate language found: {data.Language} in ItemDescriptionLocalizedDatas.");
            }
        }

        public ItemDescriptionData GetDataByLanguage(Language Language)
        {
            foreach (var localizedData in ItemDescriptionLocalizedDatas)
                if (localizedData.Language == Language)
                    return localizedData.Data;

            return null;
        }
    }

    [Serializable]
    public class ItemDescriptionLocalizedData
    {
        public Language Language;
        public ItemDescriptionData Data;
    }

    [Serializable]
    public class ItemDescriptionData
    {
        public string Name;
        public string AbilityDescription;
        public string HowToUse;
    }
}
