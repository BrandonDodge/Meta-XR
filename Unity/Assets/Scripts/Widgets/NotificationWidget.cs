// See HudLink.Core.ProjectContributionLedger for sprint attribution and dated maintenance notes.
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HudLink.Widgets
{
    public class NotificationWidget : BaseWidget
    {
        private TextMeshProUGUI _titleLabel;
        private TextMeshProUGUI _statusLabel;
        private Image _statusDot;

        public override void Initialize(RectTransform slot)
        {
            base.Initialize(slot);

            WidgetStyles.CreateStyledBackground(transform, WidgetStyles.BgPrimary, WidgetStyles.AccentBlue);
            WidgetStyles.CreateHeader(transform, "\u2709", "NOTIFICATIONS", WidgetStyles.AccentBlue);

            var contentGo = new GameObject("Content");
            contentGo.transform.SetParent(transform, false);
            var contentRect = contentGo.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 0.2f);
            contentRect.anchorMax = new Vector2(1, 1f - WidgetStyles.HeaderHeight);
            contentRect.offsetMin = new Vector2(WidgetStyles.PaddingOuter, 0);
            contentRect.offsetMax = new Vector2(-WidgetStyles.PaddingOuter, -WidgetStyles.PaddingInner);
            _titleLabel = contentGo.AddComponent<TextMeshProUGUI>();
            _titleLabel.text = "No notifications";
            _titleLabel.fontSize = 16;
            _titleLabel.color = WidgetStyles.TextSecondary;
            _titleLabel.alignment = TextAlignmentOptions.MidlineLeft;
            // Use the current TMP API so wrapping stays explicit and warning-free.
            _titleLabel.textWrappingMode = TextWrappingModes.Normal;
            _titleLabel.overflowMode = TextOverflowModes.Ellipsis;

            _statusLabel = WidgetStyles.CreateStatusBar(transform);
            _statusDot = WidgetStyles.CreateStatusDot(transform, WidgetStyles.TextMuted);
        }

        public override void UpdateData(WidgetData data)
        {
            if (data is not NotificationWidgetData notifData) return;

            if (notifData.IsRedacted)
            {
                _titleLabel.text = notifData.Title ?? "Notification";
                _titleLabel.color = WidgetStyles.TextSecondary;
                _statusLabel.text = $"{notifData.AppName} \u2022 Content hidden";
                _statusLabel.color = WidgetStyles.AccentYellow;
                _statusDot.color = WidgetStyles.AccentYellow;
            }
            else
            {
                _titleLabel.text = notifData.Title ?? "No notifications";
                _titleLabel.color = WidgetStyles.TextPrimary;
                _statusLabel.text = notifData.AppName ?? "";
                _statusLabel.color = WidgetStyles.AccentBlue;
                _statusDot.color = WidgetStyles.AccentBlue;
            }
        }
    }
}
