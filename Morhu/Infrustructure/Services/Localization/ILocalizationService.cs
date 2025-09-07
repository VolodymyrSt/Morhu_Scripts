namespace Morhu.Infrustructure.Services.Localization
{
    public interface ILocalizationService
    {
        void ChangeLanguage(Language newLanguage);
        void FindAllObjectWithLanguageIDInTheScene();
        void UpdateLanguageToCurrent();
    }
}