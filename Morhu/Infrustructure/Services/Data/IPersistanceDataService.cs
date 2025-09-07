namespace Morhu.Infrustructure.Data
{
    public interface IPersistanceDataService
    {
        GameData Data { get; }

        void Load();
        void Save();
    }
}