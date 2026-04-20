using UnityEngine;
using TMPro;

namespace HudLink.Widgets
{
    public class EnvironmentWidget : BaseWidget
    {
        private TextMeshProUGUI _valueLabel;
        private TextMeshProUGUI _statusLabel;

        public override void Initialize(RectTransform slot)
        {
            base.Initialize(slot);
            WidgetStyles.CreateStyledBackground(transform, WidgetStyles.BgPrimary, WidgetStyles.AccentYellow);
            WidgetStyles.CreateHeader(transform, "\u2600", "ENVIRONMENT", WidgetStyles.AccentYellow);
            _valueLabel = WidgetStyles.CreateValueDisplay(transform, "--\u00b0");
            WidgetStyles.CreateUnitLabel(transform, "F");
            _statusLabel = WidgetStyles.CreateStatusBar(transform, "No data");
        }

        public override void UpdateData(WidgetData data) { }
    }
}
