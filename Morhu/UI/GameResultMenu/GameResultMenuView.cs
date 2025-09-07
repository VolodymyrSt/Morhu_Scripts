using DG.Tweening;
using Morhu.Infrustructure;
using Morhu.Infrustructure.Services.AudioSystem;
using Morhu.Infrustructure.States;
using Morhu.UI.MenuHUB;
using Morhu.Util;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Morhu.UI.GameResultMenu
{
    public enum ResultLocalizationSprite {YouWonEn, YouLostEn, YouWonUk, YouLostUk }

    public class GameResultMenuView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Transform _root;
        [SerializeField] private Image _resultImage;
        [SerializeField] private Button _menuButton;
        [SerializeField] private Button _restartButton;

        [Header("Result Source Images")]
        [SerializeField] private Sprite _playerLostEnSprite;
        [SerializeField] private Sprite _playerLostUkSprite;
        [SerializeField] private Sprite _playerWonEnSprite;
        [SerializeField] private Sprite _playerWonUkSprite;

        [SerializeField] private List<ButtonAnimator> _buttonAnimators = new();

        public void Init(Game game, IAudioService audioService)
        {
            _menuButton.onClick.AddListener(() => {
                game.StateMachine.Enter<LoadMenuState>();
                audioService.BuildSound().WithParent(transform).Play("ClickButton");
            });

            _restartButton.onClick.AddListener(() => {
                game.StateMachine.Enter<LoadGameplayState>();
                audioService.BuildSound().WithParent(transform).Play("ClickButton");
            });

            foreach (var animator in _buttonAnimators)
                animator.Init(audioService);
        }

        private void Start() => Hide();

        private void Hide()
        {
            _resultImage.transform.localScale = Vector3.zero;
            _root.SetActive(false);
        }

        public void ShowResult(ResultLocalizationSprite result)
        {
            _root.SetActive(true);

             switch (result)
            {
                case ResultLocalizationSprite.YouLostEn:
                    _resultImage.sprite = _playerLostEnSprite;
                    break;
                case ResultLocalizationSprite.YouWonEn:
                    _resultImage.sprite = _playerWonEnSprite;
                    break;
                case ResultLocalizationSprite.YouWonUk:
                    _resultImage.sprite = _playerWonUkSprite;
                    break;
                case ResultLocalizationSprite.YouLostUk:
                    _resultImage.sprite = _playerLostUkSprite;
                    break;
                default:
                    throw new System.Exception($"Cant find ResultLocalizationType {result}");
            }

            _resultImage.transform.DOScale(Vector3.one, Constants.ANIMATION_DURATION)
                .SetEase(Ease.Linear)
                .Play();
        }
    }
}
