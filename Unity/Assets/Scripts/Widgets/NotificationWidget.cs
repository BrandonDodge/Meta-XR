using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HudLink.Widgets
{
    /// <summary>
    /// Displays minimal phone notifications with privacy controls.
    /// Acceptance criteria (R10): Shows caller ID / text snippet,
    /// supports "redacted mode" (title only), per-app enable/disable.
    /// </summary>
    public class NotificationWidget : BaseWidget
    {
        private TextMeshProUGUI _appLabel;
        private TextMeshProUGUI _titleLabel;
        private TextMeshProUGUI _statusLabel;

        public override void Initialize(RectTransform slot)
        {
            base.Initialize(slot);

            CreateBackground(new Color(0.15f, 0.15f, 0.15f, 0.7f));

            _appLabel = CreateLabel("AppName", 12, TextAlignmentOptions.TopLeft);
            _appLabel.text = "";
            _appLabel.color = new Color(0.6f, 0.6f, 0.8f);
            var appRect = _appLabel.rectTransform;
            appRect.anchorMin = new Vector2(0.05f, 0.65f);
            appRect.anchorMax = new Vector2(0.95f, 0.9f);
            appRect.offsetMin = Vector2.zero;
            appRect.offsetMax = Vector2.zero;

            _titleLabel = CreateLabel("NotifTitle", 18, TextAlignmentOptions.Left);
            _titleLabel.text = "No notifications";
            var titleRect = _titleLabel.rectTransform;
            titleRect.anchorMin = new Vector2(0.05f, 0.2f);
            titleRect.anchorMax = new Vector2(0.95f, 0.65f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;

            _statusLabel = CreateLabel("NotifStatus", 10, TextAlignmentOptions.BottomLeft);
            _statusLabel.text = "";
            _statusLabel.color = new Color(0.5f, 0.5f, 0.5f);
            var statusRect = _statusLabel.rectTransform;
            statusRect.anchorMin = new Vector2(0.05f, 0.02f);
            statusRect.anchorMax = new Vector2(0.95f, 0.2f);
            statusRect.offsetMin = Vector2.zero;
            statusRect.offsetMax = Vector2.zero;
        }

        public override void UpdateData(WidgetData data)
        {
            if (data is not NotificationWidgetData notifData) return;

            _appLabel.text = notifData.AppName ?? "";

            if (notifData.IsRedacted)
            {
                _titleLabel.text = notifData.Title ?? "Notification";
                _statusLabel.text = "Content hidden";
            }
            else
            {
                _titleLabel.text = notifData.Title ?? "No notifications";
                _statusLabel.text = "";
            }
        }

        private Image CreateBackground(Color color)
        {
            var go = new GameObject("Background");
            go.transform.SetParent(transform, false);
            go.transform.SetAsFirstSibling();

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var img = go.AddComponent<Image>();
            img.color = color;
            return img;
        }
    }
}
