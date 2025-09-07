using System;
using System.IO;
using UnityEngine;

namespace Morhu.Infrustructure.Data
{
    public class SaveLoadHandler
    {
        private const string ENCRYPTION_DECRYPTION_WORD = "trytohackme";
        private readonly string _dataDirectoryPath = "";
        private readonly string _dataFileName = "";

        public SaveLoadHandler(string dataDirectoryPath, string dataFileName)
        {
            _dataDirectoryPath = dataDirectoryPath;
            _dataFileName = dataFileName;
        }

        public void Save(GameData data)
        {
            var fullPath = Path.Combine(_dataDirectoryPath, _dataFileName);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            var gameData = JsonUtility.ToJson(data, true);
            gameData = EncryptDecrypt(gameData);

            try
            {
                using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                    using (var writer = new StreamWriter(stream))
                        writer.Write(gameData);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public GameData Load()
        {
            var fullPath = Path.Combine(_dataDirectoryPath, _dataFileName);
            GameData dataToLoad = null;

            if (File.Exists(fullPath))
            {
                string encryptedData = "";
                string decryptedData = "";

                try
                {
                    using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                        using (var reader = new StreamReader(stream))
                            encryptedData = reader.ReadToEnd();

                    decryptedData = EncryptDecrypt(encryptedData);
                    dataToLoad = JsonUtility.FromJson<GameData>(decryptedData);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return dataToLoad;
        }

        private string EncryptDecrypt(string data)
        {
            var key = ENCRYPTION_DECRYPTION_WORD;
            var result = new char[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                result[i] = (char)(data[i] ^ key[i % key.Length]);
            }
            return new string(result);
        }
    }
}
