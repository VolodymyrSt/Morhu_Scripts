using Di;
using Morhu.Infrustructure.AssetManagement;
using Morhu.Infrustructure.Data;
using Morhu.Infrustructure.Services.AudioSystem;
using Morhu.Infrustructure.Services.AudioSystem.ScriptableObjects;
using Morhu.UI.MenuHUB;
using UnityEngine;

namespace Morhu.Infrustructure.Installers
{
    public class MenuInstaller : LocalInstaller
    {
        [SerializeField] private MenuHub _menuHub;

        [Header("Sounds")]
        [SerializeField] private CompositionsHolderSO _compositionsHolderSO;

        [Header("Setting")]
        [SerializeField] private MenuSettingView _menuSettingView;

        protected override void InstallBindings()
        {
            BindAudioSystem();
            BindMenuHub();
            BindSetting();
        }

        private void BindMenuHub() => 
            Container.BindInstance(_menuHub).NeedToBeConstructed();

        private void BindSetting()
        {
            Container.BindInstance(_menuSettingView);
            Container.Bind(c => new MenuSettingHandler(_menuSettingView,
                c.Resolve<IAudioService>(), c.Resolve<Game>(), c.Resolve<IPersistanceDataService>())).AsSingle().NotLazy();
        }

        private void BindAudioSystem()
        {
            Container.Bind<IAudioService>(c => new AudioService(_compositionsHolderSO
                , c.Resolve<AssetProvider>(), c.Resolve<IPersistanceDataService>())).AsSingle();
        }
    }
}
