using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Morhu.Infrustructure.Services.Localization
{
    public class LanguageID : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI _massage;

        [Header("Localization")]
        [SerializeField] private List<LenguageData> _lenguageDatas = new();

        private void OnValidate()
        {
            if (_massage == null) 
                _massage = GetComponent<TextMeshProUGUI>();
        }

        public void UpdateText(Language language)
        {
            if (_lenguageDatas.Count == 0) return;

            foreach (var data in _lenguageDatas)
            {
                if (data.Language == language)
                {
                    _massage.text = data.Text;
                    break;
                }
            }
        }
    }
}
