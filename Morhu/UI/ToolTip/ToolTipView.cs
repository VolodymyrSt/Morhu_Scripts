using DG.Tweening;
using Morhu.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Morhu.UI.ToolTip
{
    public class ToolTipView : MonoBehaviour
    {
        [Header("Base")]
        [SerializeField] private RectTransform _root;
        [SerializeField] private TextMeshProUGUI _headerText;
        [SerializeField] private TextMeshProUGUI _content1Text;
        [SerializeField] private TextMeshProUGUI _content2Text;
        [SerializeField] private LayoutElement _layoutElement;

        public bool IsVisible => _root.gameObject.activeSelf;

        public void Init() => HideAtStart();

        private void Update()
        {
            var headerTextLength = _headerText.text.Length;
            var content1TextLength = _content1Text.text.Length;
            var content2TextLength = _content2Text.text.Length;
            _layoutElement.enabled = headerTextLength > Constants.CHARACTER_WRAP_LIMIT || 
                content1TextLength  > Constants.CHARACTER_WRAP_LIMIT ||
                content2TextLength > Constants.CHARACTER_WRAP_LIMIT ? true : false;
        }

        public void Show(string headerText, string content1Text, string content2Text)
        {
            _headerText.text = headerText;
            _content1Text.text = content1Text;
            _content2Text.text = content2Text;

            _root.gameObject.SetActive(true);

            _root.DOKill();
            _root.DOScale(1f, Constants.ANIMATION_DURATION)
                .SetEase(Ease.OutQuad)
                .Play();
        }

        public void Hide()
        {
            _root.DOKill();
            _root.DOScale(0f, Constants.ANIMATION_DURATION)
                .SetEase(Ease.InQuad)
                .Play()
                .OnComplete(() => _root.gameObject.SetActive(false));
        }

        public void SetLocalPosition(Vector3 localPosition) => 
            _root.localPosition = localPosition;

        private void HideAtStart()
        {
            _root.localScale = Vector3.zero;
            _root.gameObject.SetActive(false);
        }
    }
}
