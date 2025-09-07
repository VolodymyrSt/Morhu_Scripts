using Morhu.Infrustructure.Services.Localization;
using Morhu.Util;
using System;

namespace Morhu.Infrustructure.Data
{
    [Serializable]
    public class GameData
    {
        public float GameVolume;
        public Language Language;

        public GameData(float startedGameVolume, Language startedLanguage)
        {
            GameVolume = startedGameVolume;
            Language = startedLanguage;
        }
    }
}
