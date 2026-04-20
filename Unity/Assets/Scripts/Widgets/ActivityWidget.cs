using UnityEngine;
using TMPro;

namespace HudLink.Widgets
{
    public class ActivityWidget : BaseWidget
    {
        private TextMeshProUGUI _valueLabel;
        private TextMeshProUGUI _statusLabel;

        public override void Initialize(RectTransform slot)
        {
            base.Initialize(slot);
            WidgetStyles.CreateStyledBackground(transform, WidgetStyles.BgPrimary, WidgetStyles.AccentGreen);
            WidgetStyles.CreateHeader(transform, "\u2605", "ACTIVITY", WidgetStyles.AccentGreen);
            _valueLabel = WidgetStyles.CreateValueDisplay(transform, "0%");
            WidgetStyles.CreateUnitLabel(transform, "GOAL");
            _statusLabel = WidgetStyles.CreateStatusBar(transform, "No data");
        }

        public override void UpdateData(WidgetData data) { }
    }
}
