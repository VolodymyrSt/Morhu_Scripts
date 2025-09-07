using Morhu.Infrustructure.Services.Localization;
using UnityEngine;

namespace Morhu.Infrustructure.Data
{
    public class PersistanceDataService : IPersistanceDataService
    {
        private readonly SaveLoadHandler _saveLoadHandler;

        public GameData Data { get; private set; }

        public PersistanceDataService() => 
            _saveLoadHandler = new SaveLoadHandler(Application.persistentDataPath, "Data.json");

        public void Load()
        {
            Data = _saveLoadHandler.Load();
            Data ??= new GameData(startedGameVolume: 1f, startedLanguage: Language.En);
        }

        public void Save() => 
            _saveLoadHandler.Save(Data);
    }
}
