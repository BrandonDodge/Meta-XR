using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HudLink.Widgets
{
    /// <summary>
    /// Displays live heart rate with value, unit, and last-updated indicator.
    /// Acceptance criteria (R5): Shows HR value (bpm), "last updated" indicator,
    /// handles invalid/unavailable HR with "No signal" message.
    /// </summary>
    public class HeartRateWidget : BaseWidget
    {
        private TextMeshProUGUI _valueLabel;
        private TextMeshProUGUI _unitLabel;
        private TextMeshProUGUI _statusLabel;
        private Image _bgPanel;

        private int _lastBpm;
        private float _lastUpdateTime;

        public override void Initialize(RectTransform slot)
        {
            base.Initialize(slot);

            _bgPanel = CreateBackground(new Color(0.15f, 0.15f, 0.15f, 0.7f));

            _valueLabel = CreateLabel("HeartRateValue", 36, TextAlignmentOptions.Center);
            _valueLabel.text = "--";
            var valueRect = _valueLabel.rectTransform;
            valueRect.anchorMin = new Vector2(0, 0.3f);
            valueRect.anchorMax = new Vector2(1, 0.85f);
            valueRect.offsetMin = Vector2.zero;
            valueRect.offsetMax = Vector2.zero;

            _unitLabel = CreateLabel("HeartRateUnit", 14, TextAlignmentOptions.Center);
            _unitLabel.text = "BPM";
            _unitLabel.color = new Color(0.7f, 0.7f, 0.7f);
            var unitRect = _unitLabel.rectTransform;
            unitRect.anchorMin = new Vector2(0, 0.15f);
            unitRect.anchorMax = new Vector2(1, 0.35f);
            unitRect.offsetMin = Vector2.zero;
            unitRect.offsetMax = Vector2.zero;

            _statusLabel = CreateLabel("HeartRateStatus", 10, TextAlignmentOptions.Center);
            _statusLabel.text = "Waiting for data...";
            _statusLabel.color = new Color(0.5f, 0.5f, 0.5f);
            var statusRect = _statusLabel.rectTransform;
            statusRect.anchorMin = new Vector2(0, 0f);
            statusRect.anchorMax = new Vector2(1, 0.18f);
            statusRect.offsetMin = Vector2.zero;
            statusRect.offsetMax = Vector2.zero;
        }

        public override void UpdateData(WidgetData data)
        {
            if (data is not HeartRateWidgetData hrData) return;

            _lastUpdateTime = Time.time;

            if (!hrData.IsValid)
            {
                _valueLabel.text = "--";
                _valueLabel.color = Color.gray;
                _statusLabel.text = "No signal";
                return;
            }

            _lastBpm = hrData.Bpm;
            _valueLabel.text = _lastBpm.ToString();
            _valueLabel.color = GetHeartRateColor(_lastBpm);
            _statusLabel.text = "Live";
        }

        private void Update()
        {
            if (!Initialized) return;

            // Stale data indicator — if no update in 5 seconds, show staleness
            float elapsed = Time.time - _lastUpdateTime;
            if (_lastUpdateTime > 0 && elapsed > 5f)
            {
                _statusLabel.text = $"{elapsed:F0}s ago";
                _statusLabel.color = elapsed > 10f ? new Color(1f, 0.4f, 0.4f) : new Color(0.5f, 0.5f, 0.5f);
            }
        }

        private Color GetHeartRateColor(int bpm)
        {
            if (bpm < 60) return new Color(0.4f, 0.7f, 1f);       // Low — blue
            if (bpm < 100) return new Color(0.4f, 1f, 0.5f);      // Normal — green
            if (bpm < 140) return new Color(1f, 0.85f, 0.3f);     // Elevated — yellow
            return new Color(1f, 0.4f, 0.4f);                      // High — red
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
