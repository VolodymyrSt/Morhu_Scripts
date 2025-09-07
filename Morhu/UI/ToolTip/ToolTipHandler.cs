using System;
using UnityEngine;

namespace Morhu.UI.ToolTip
{
    public class ToolTipHandler
    {
        private readonly ToolTipView _view;
        private readonly Canvas _canvas;
        private Vector2 _offset;

        private bool _isToolTipStopped = false;

        public ToolTipHandler(ToolTipView view, Canvas canvas)
        {
            _view = view;
            _canvas = canvas;
            _view.Init();
        }

        public void Tick()
        {
            if (!_view.IsVisible) return;
            if (_isToolTipStopped) return;
            PerformFollowing();
        }

        public void Activate(string headerText, string content1Text, string content2Text, ToolTipPosition toolTipPosition)
        {
            if (_isToolTipStopped) return;

            _view.Show(headerText, content1Text, content2Text);
            SetPosition(toolTipPosition);
        }

        public void Disactivate() => _view.Hide();

        private void PerformFollowing()
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform
                , Input.mousePosition, _canvas.worldCamera, out localPoint))

            _view.SetLocalPosition(localPoint + _offset);
        }

        private void SetPosition(ToolTipPosition toolTipPosition)
        {
            switch (toolTipPosition) 
            {
                case ToolTipPosition.TopLeft:
                    _offset = new(-100f, 100f);
                    break;
                case ToolTipPosition.TopRight:
                    _offset = new(100f, 100f);
                    break;
                case ToolTipPosition.BottomLeft:
                    _offset = new(-100f, -100f);
                    break;
                case ToolTipPosition.BottomRight:
                    _offset = new(100f, -100f);
                    break;
                default:
                    throw new Exception("Unrecognized tooltip position");
            }
        }

        public void StopToolTipPerformance() => 
            _isToolTipStopped = true;
        public void ResetToolTipPerformance() => 
            _isToolTipStopped = false;
    }

    public enum ToolTipPosition
    {
        TopLeft, TopRight, BottomRight, BottomLeft
    }
}
