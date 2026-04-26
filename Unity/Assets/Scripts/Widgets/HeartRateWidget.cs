/*
 Code Artifact: HeartRateWidget.cs
 Description: Renders heart-rate data, validity state, stale-data warnings, and heart-rate color ranges.
 Programmer: HudLink development team (Brandon Dodge, Zach Sevart, Asa Maker, Jonathan Gott, Josh Dwoskin)
 Date Created: 2026-03-06
 Revision History:
 - 2026-03-27 - Zach Sevart - Improve widget UI styling and add team setup guide
 - 2026-04-26 - HudLink development team - Added release prologue comments and tightened sprint-review documentation.
 - 2026-04-26 - HudLink development team - Matched the headset widget header to the virtual demo card style.
 Preconditions: Unity calls this artifact on the main thread; required scene references and serialized fields are assigned before runtime use.
 Acceptable Inputs: Valid Unity objects, event payloads, widget data, enum values, and inspector settings documented by the fields below.
 Unacceptable Inputs: Missing required scene references, null widget components, invalid slot choices, or sensor values outside the documented model ranges.
 Postconditions: HUD state, widget UI, event subscriptions, or diagnostic output is updated according to the public method that was called.
 Return Values: Unity lifecycle methods return void; helper methods return the type named in the method signature, or null only where the method documents a missing reference.
 Error and Exception Conditions: Unity may log errors or warnings for missing references, wrong prefab setup, invalid data, or unsupported slot assignments.
 Side Effects: May create, parent, destroy, activate, deactivate, or recolor Unity GameObjects and may publish or subscribe to HUD events.
 Invariants: Widget IDs stay stable for routing, the center safe zone remains clear, and UI updates run on Unity's main thread.
 Known Faults: Live Android bridge input is not complete yet, so several demo paths still use mock data or simulated events.
 Major Blocks: The inline comments below mark lifecycle hooks, data routing, validation, persistence, and UI update blocks.
 Line Comments: Important statements and branches carry short notes where a teammate would otherwise need extra context.
*/
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HudLink.Widgets
{
    public class HeartRateWidget : BaseWidget
    {
        private TextMeshProUGUI _valueLabel;
        private TextMeshProUGUI _unitLabel;
        private TextMeshProUGUI _statusLabel;
        private Image _statusDot;
        private Image _bgPanel;

        private int _lastBpm;
        private float _lastUpdateTime;

        public override void Initialize(RectTransform slot)
        {
            base.Initialize(slot);

            _bgPanel = WidgetStyles.CreateStyledBackground(transform, WidgetStyles.BgPrimary, WidgetStyles.AccentRed);
            WidgetStyles.CreateHeader(transform, "HR", "HEALTH", WidgetStyles.AccentRed);

            _valueLabel = WidgetStyles.CreateValueDisplay(transform);
            _unitLabel = WidgetStyles.CreateUnitLabel(transform, "BPM");
            _statusLabel = WidgetStyles.CreateStatusBar(transform, "Waiting for data...");
            _statusDot = WidgetStyles.CreateStatusDot(transform, WidgetStyles.TextMuted);
        }

        public override void UpdateData(WidgetData data)
        {
            if (data is not HeartRateWidgetData hrData) return;

            _lastUpdateTime = Time.time;

            if (!hrData.IsValid)
            {
                _valueLabel.text = "--";
                _valueLabel.color = WidgetStyles.TextMuted;
                _statusLabel.text = "No signal";
                _statusDot.color = WidgetStyles.AccentRed;
                return;
            }

            _lastBpm = hrData.Bpm;
            _valueLabel.text = _lastBpm.ToString();
            _valueLabel.color = GetHeartRateColor(_lastBpm);
            _statusLabel.text = "Live";
            _statusLabel.color = WidgetStyles.AccentGreen;
            _statusDot.color = WidgetStyles.AccentGreen;
        }

        private void Update()
        {
            if (!Initialized) return;

            float elapsed = Time.time - _lastUpdateTime;
            if (_lastUpdateTime > 0 && elapsed > 5f)
            {
                _statusLabel.text = $"{elapsed:F0}s ago";
                _statusLabel.color = elapsed > 10f ? WidgetStyles.AccentRed : WidgetStyles.TextMuted;
                _statusDot.color = elapsed > 10f ? WidgetStyles.AccentRed : WidgetStyles.AccentYellow;
            }
        }

        private Color GetHeartRateColor(int bpm)
        {
            if (bpm < 60) return WidgetStyles.AccentBlue;
            if (bpm < 100) return WidgetStyles.AccentGreen;
            if (bpm < 140) return WidgetStyles.AccentYellow;
            return WidgetStyles.AccentRed;
        }
    }
}
