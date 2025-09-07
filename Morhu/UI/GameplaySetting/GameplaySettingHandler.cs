using Morhu.Infrustructure;
using Morhu.Infrustructure.Services.AudioSystem;
using UnityEngine;

namespace Morhu.UI.GameplaySetting
{
    public class GameplaySettingHandler
    {
        private readonly GameplaySettingView _view;
        private bool _isClosed;

        public GameplaySettingHandler(GameplaySettingView view, IAudioService audioService, Game game)
        {
            _view = view;
            _view.Init(audioService, game);
            _isClosed = true;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_isClosed)
                    ShowSettingMenu();
                else
                    HideSettingMenu();
            }
        }

        private void ShowSettingMenu()
        {
            _isClosed = false;
            _view.Show();
        }

        private void HideSettingMenu()
        {
            _isClosed = true;
            _view.Hide();
        }
    }
}
