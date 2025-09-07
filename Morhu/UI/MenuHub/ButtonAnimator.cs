using DG.Tweening;
using Morhu.Infrustructure.Services.AudioSystem;
using Morhu.Util;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Morhu.UI.MenuHUB
{
    public class ButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private IAudioService _audioService;

        public void Init(IAudioService audioService) => 
            _audioService = audioService;

        private void OnEnable() => 
            transform.DOScale(Constants.BUTTON_UNSCALED, 0).Play();

        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.DOScale(Constants.BUTTON_SCALED, Constants.ANIMATION_DURATION)
                .SetEase(Ease.InOutCubic)
                .Play()
                .OnComplete(() => {
                    _audioService.BuildSound().WithParent(transform).Play("SelectButton");
                });
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.DOKill();
            transform.DOScale(Constants.BUTTON_UNSCALED, Constants.ANIMATION_DURATION)
                .SetEase(Ease.InOutCubic)
                .Play();
        }
    }
}
