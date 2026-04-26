/*
 Code Artifact: GPSWidget.cs
 Description: Renders speed, heading, and GPS fix status in a compact HUD widget.
 Programmer: HudLink development team (Brandon Dodge, Zach Sevart, Asa Maker, Jonathan Gott, Josh Dwoskin)
 Date Created: 2026-03-06
 Revision History:
 - 2026-03-27 - Zach Sevart - Improve widget UI styling and add team setup guide
 - 2026-04-26 - HudLink development team - Added release prologue comments and tightened sprint-review documentation.
 - 2026-04-26 - HudLink development team - Matched the headset widget layout to the virtual demo card style.
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
    public class GPSWidget : BaseWidget
    {
        private TextMeshProUGUI _speedLabel;
        private TextMeshProUGUI _unitLabel;
        private TextMeshProUGUI _headingLabel;
        private TextMeshProUGUI _statusLabel;
        private Image _statusDot;

        private static readonly string[] CardinalDirections =
            { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };

        public override void Initialize(RectTransform slot)
        {
            base.Initialize(slot);

            WidgetStyles.CreateStyledBackground(transform, WidgetStyles.BgPrimary, WidgetStyles.AccentCyan);
            // Use plain text so the default TextMesh Pro font does not warn about missing symbol glyphs.
            WidgetStyles.CreateHeader(transform, "GPS", "LOCATION", WidgetStyles.AccentCyan);

            _speedLabel = WidgetStyles.CreateValueDisplay(transform);
            _unitLabel = WidgetStyles.CreateUnitLabel(transform, "MPH");

            var headingGo = new GameObject("Heading");
            headingGo.transform.SetParent(transform, false);
            var headingRect = headingGo.AddComponent<RectTransform>();
            headingRect.anchorMin = new Vector2(0.55f, 0.55f);
            headingRect.anchorMax = new Vector2(1f, 0.8f);
            headingRect.offsetMin = new Vector2(0, 0);
            headingRect.offsetMax = new Vector2(-WidgetStyles.PaddingOuter, 0);
            _headingLabel = headingGo.AddComponent<TextMeshProUGUI>();
            _headingLabel.text = "--";
            _headingLabel.fontSize = 18;
            _headingLabel.color = WidgetStyles.TextSecondary;
            _headingLabel.alignment = TextAlignmentOptions.MidlineRight;
            _headingLabel.enableAutoSizing = true;
            _headingLabel.fontSizeMin = 10;
            _headingLabel.fontSizeMax = 18;
            _headingLabel.overflowMode = TextOverflowModes.Truncate;

            _statusLabel = WidgetStyles.CreateStatusBar(transform, "No GPS");
            _statusDot = WidgetStyles.CreateStatusDot(transform, WidgetStyles.TextMuted);
        }

        public override void UpdateData(WidgetData data)
        {
            if (data is not GpsWidgetData gpsData) return;

            if (!gpsData.HasFix)
            {
                _speedLabel.text = "--";
                _speedLabel.color = WidgetStyles.TextMuted;
                _headingLabel.text = "--";
                _statusLabel.text = "Acquiring signal...";
                _statusLabel.color = WidgetStyles.AccentYellow;
                _statusDot.color = WidgetStyles.AccentYellow;
                return;
            }

            _speedLabel.text = $"{gpsData.SpeedMph:F1}";
            _speedLabel.color = WidgetStyles.TextPrimary;
            _headingLabel.text = DegreesToCardinal(gpsData.HeadingDegrees);
            _headingLabel.color = WidgetStyles.AccentCyan;
            _statusLabel.text = "GPS Active";
            _statusLabel.color = WidgetStyles.AccentGreen;
            _statusDot.color = WidgetStyles.AccentGreen;
        }

        private string DegreesToCardinal(float degrees)
        {
            int index = Mathf.RoundToInt(degrees / 45f) % 8;
            if (index < 0) index += 8;
            return $"{CardinalDirections[index]} {degrees:F0} deg";
        }
    }
}
