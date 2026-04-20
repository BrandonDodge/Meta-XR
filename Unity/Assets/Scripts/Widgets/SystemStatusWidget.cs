using UnityEngine;
using TMPro;

namespace HudLink.Widgets
{
    public class SystemStatusWidget : BaseWidget
    {
        private TextMeshProUGUI _valueLabel;
        private TextMeshProUGUI _statusLabel;

        public override void Initialize(RectTransform slot)
        {
            base.Initialize(slot);
            WidgetStyles.CreateStyledBackground(transform, WidgetStyles.BgPrimary, WidgetStyles.AccentBlue);
            WidgetStyles.CreateHeader(transform, "\u2699", "SYSTEM", WidgetStyles.AccentBlue);
            _valueLabel = WidgetStyles.CreateValueDisplay(transform, "OK");
            _valueLabel.fontSize = 32;
            _statusLabel = WidgetStyles.CreateStatusBar(transform, "All systems nominal");
            _statusLabel.color = WidgetStyles.AccentGreen;
        }

        public override void UpdateData(WidgetData data) { }
    }
}
