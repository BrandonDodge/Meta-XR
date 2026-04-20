using UnityEngine;
using TMPro;

namespace HudLink.Widgets
{
    public class MediaWidget : BaseWidget
    {
        private TextMeshProUGUI _titleLabel;
        private TextMeshProUGUI _statusLabel;

        public override void Initialize(RectTransform slot)
        {
            base.Initialize(slot);
            WidgetStyles.CreateStyledBackground(transform, WidgetStyles.BgPrimary, new Color(0.8f, 0.4f, 1f));
            WidgetStyles.CreateHeader(transform, "\u266B", "MEDIA", new Color(0.8f, 0.4f, 1f));

            var contentGo = new GameObject("Content");
            contentGo.transform.SetParent(transform, false);
            var contentRect = contentGo.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 0.2f);
            contentRect.anchorMax = new Vector2(1, 1f - WidgetStyles.HeaderHeight);
            contentRect.offsetMin = new Vector2(WidgetStyles.PaddingOuter, 0);
            contentRect.offsetMax = new Vector2(-WidgetStyles.PaddingOuter, -WidgetStyles.PaddingInner);
            _titleLabel = contentGo.AddComponent<TextMeshProUGUI>();
            _titleLabel.text = "No media playing";
            _titleLabel.fontSize = 16;
            _titleLabel.color = WidgetStyles.TextSecondary;
            _titleLabel.alignment = TextAlignmentOptions.MidlineLeft;

            _statusLabel = WidgetStyles.CreateStatusBar(transform);
        }

        public override void UpdateData(WidgetData data) { }
    }
}
