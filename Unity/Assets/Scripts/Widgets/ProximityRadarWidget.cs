using UnityEngine;
using TMPro;

namespace HudLink.Widgets
{
    public class ProximityRadarWidget : BaseWidget
    {
        private TextMeshProUGUI _valueLabel;
        private TextMeshProUGUI _statusLabel;

        public override void Initialize(RectTransform slot)
        {
            base.Initialize(slot);
            WidgetStyles.CreateStyledBackground(transform, WidgetStyles.BgPrimary, WidgetStyles.AccentCyan);
            WidgetStyles.CreateHeader(transform, "\u25CE", "RADAR", WidgetStyles.AccentCyan);
            _valueLabel = WidgetStyles.CreateValueDisplay(transform, "0");
            WidgetStyles.CreateUnitLabel(transform, "NEAR");
            _statusLabel = WidgetStyles.CreateStatusBar(transform, "Scanning...");
        }

        public override void UpdateData(WidgetData data) { }
    }
}
