/*
 Code Artifact: NotificationWidget.cs
 Description: Renders notification titles, privacy-redacted content, and source app status.
 Programmer: HudLink development team (Brandon Dodge, Zach Sevart, Asa Maker, Jonathan Gott, Josh Dwoskin)
 Date Created: 2026-03-06
 Revision History:
 - 2026-04-13 - Brandon.Dodge - Update contribution ledger and Unity scripts
 - 2026-04-26 - HudLink development team - Added release prologue comments and tightened sprint-review documentation.
 - 2026-04-26 - HudLink development team - Matched notification cards to the virtual demo headset style.
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
    public class NotificationWidget : BaseWidget
    {
        private TextMeshProUGUI _titleLabel;
        private TextMeshProUGUI _statusLabel;
        private Image _statusDot;

        public override void Initialize(RectTransform slot)
        {
            base.Initialize(slot);

            WidgetStyles.CreateStyledBackground(transform, WidgetStyles.BgPrimary, WidgetStyles.AccentBlue);
            // Use plain text so the default TextMesh Pro font does not warn about missing symbol glyphs.
            WidgetStyles.CreateHeader(transform, "MSG", "NOTICES", WidgetStyles.AccentBlue);

            var contentGo = new GameObject("Content");
            contentGo.transform.SetParent(transform, false);
            var contentRect = contentGo.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 0.2f);
            contentRect.anchorMax = new Vector2(1, 1f - WidgetStyles.HeaderHeight);
            contentRect.offsetMin = new Vector2(WidgetStyles.PaddingOuter, 0);
            contentRect.offsetMax = new Vector2(-WidgetStyles.PaddingOuter, -WidgetStyles.PaddingInner);
            _titleLabel = contentGo.AddComponent<TextMeshProUGUI>();
            _titleLabel.text = "No notifications";
            _titleLabel.fontSize = 15;
            _titleLabel.color = WidgetStyles.TextSecondary;
            _titleLabel.alignment = TextAlignmentOptions.MidlineLeft;
            // Use the current TMP API so wrapping stays explicit and warning-free.
            _titleLabel.textWrappingMode = TextWrappingModes.Normal;
            _titleLabel.enableAutoSizing = true;
            _titleLabel.fontSizeMin = 10;
            _titleLabel.fontSizeMax = 15;
            _titleLabel.overflowMode = TextOverflowModes.Truncate;

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
