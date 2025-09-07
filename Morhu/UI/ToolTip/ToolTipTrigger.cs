using Di;

namespace Morhu.UI.ToolTip
{
    public class ToolTipTrigger
    {
        private readonly string _toolTipHeaderText;
        private readonly string _toolTipContent1Text;
        private readonly string _toolTipContent2Text;

        private readonly ToolTipHandler _toolTip;
        private readonly ToolTipPosition _toolTipPosition;

        public ToolTipTrigger(ToolTipHandler toolTip, string toolTipHeaderText, string toolTipContent1Text,string toolTipContent2Text, ToolTipPosition toolTipPosition)
        {
            _toolTip = toolTip;
            _toolTipHeaderText = toolTipHeaderText;
            _toolTipContent1Text = toolTipContent1Text;
            _toolTipContent2Text = toolTipContent2Text;
            _toolTipPosition = toolTipPosition;
        }

        public void Show() => 
            _toolTip.Activate(_toolTipHeaderText, _toolTipContent1Text, _toolTipContent2Text, _toolTipPosition);

        public void Hide() =>
            _toolTip.Disactivate();
    }
}
