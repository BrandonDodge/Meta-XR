using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HudLink.Widgets
{
    /// <summary>
    /// Displays GPS info: speed, heading, and signal status.
    /// Acceptance criteria (R8): Shows speed + heading OR "location acquired" indicator.
    /// Handles no-permission/no-signal gracefully.
    /// </summary>
    public class GPSWidget : BaseWidget
    {
        private TextMeshProUGUI _speedLabel;
        private TextMeshProUGUI _headingLabel;
        private TextMeshProUGUI _statusLabel;

        private static readonly string[] CardinalDirections =
            { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };

        public override void Initialize(RectTransform slot)
        {
            base.Initialize(slot);

            CreateBackground(new Color(0.15f, 0.15f, 0.15f, 0.7f));

            _speedLabel = CreateLabel("SpeedValue", 30, TextAlignmentOptions.Center);
            _speedLabel.text = "-- mph";
            var speedRect = _speedLabel.rectTransform;
            speedRect.anchorMin = new Vector2(0, 0.4f);
            speedRect.anchorMax = new Vector2(1, 0.85f);
            speedRect.offsetMin = Vector2.zero;
            speedRect.offsetMax = Vector2.zero;

            _headingLabel = CreateLabel("Heading", 16, TextAlignmentOptions.Center);
            _headingLabel.text = "--";
            _headingLabel.color = new Color(0.7f, 0.7f, 0.7f);
            var headingRect = _headingLabel.rectTransform;
            headingRect.anchorMin = new Vector2(0, 0.2f);
            headingRect.anchorMax = new Vector2(1, 0.45f);
            headingRect.offsetMin = Vector2.zero;
            headingRect.offsetMax = Vector2.zero;

            _statusLabel = CreateLabel("GpsStatus", 10, TextAlignmentOptions.Center);
            _statusLabel.text = "No GPS";
            _statusLabel.color = new Color(0.5f, 0.5f, 0.5f);
            var statusRect = _statusLabel.rectTransform;
            statusRect.anchorMin = new Vector2(0, 0f);
            statusRect.anchorMax = new Vector2(1, 0.2f);
            statusRect.offsetMin = Vector2.zero;
            statusRect.offsetMax = Vector2.zero;
        }

        public override void UpdateData(WidgetData data)
        {
            if (data is not GpsWidgetData gpsData) return;

            if (!gpsData.HasFix)
            {
                _speedLabel.text = "-- mph";
                _headingLabel.text = "--";
                _statusLabel.text = "Acquiring signal...";
                return;
            }

            _speedLabel.text = $"{gpsData.SpeedMph:F1} mph";
            _headingLabel.text = DegreesToCardinal(gpsData.HeadingDegrees);
            _statusLabel.text = "GPS Active";
            _statusLabel.color = new Color(0.4f, 1f, 0.5f);
        }

        private string DegreesToCardinal(float degrees)
        {
            int index = Mathf.RoundToInt(degrees / 45f) % 8;
            if (index < 0) index += 8;
            return $"{CardinalDirections[index]} ({degrees:F0}\u00b0)";
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
