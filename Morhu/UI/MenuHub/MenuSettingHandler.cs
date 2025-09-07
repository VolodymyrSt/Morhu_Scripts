using Morhu.Infrustructure;
using Morhu.Infrustructure.Data;
using Morhu.Infrustructure.Services.AudioSystem;

namespace Morhu.UI.MenuHUB
{
    public class MenuSettingHandler
    {
        private readonly MenuSettingView _view;

        public MenuSettingHandler(MenuSettingView view, IAudioService audioService, Game game, IPersistanceDataService persistanceDataService)
        {
            _view = view;
            _view.Init(audioService, game, persistanceDataService);
        }
    }
}
