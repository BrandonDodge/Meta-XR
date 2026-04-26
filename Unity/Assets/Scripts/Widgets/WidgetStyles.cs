/*
 Code Artifact: WidgetStyles.cs
 Description: Centralizes widget colors, spacing, font sizes, and factory helpers for common UI elements.
 Programmer: HudLink development team (Brandon Dodge, Zach Sevart, Asa Maker, Jonathan Gott, Josh Dwoskin)
 Date Created: 2026-03-27
 Revision History:
 - 2026-03-27 - Zach Sevart - Improve widget UI styling and add team setup guide
 - 2026-04-26 - HudLink development team - Added release prologue comments and tightened sprint-review documentation.
 - 2026-04-26 - HudLink development team - Matched runtime headset widgets to the virtual demo edge-card HUD style.
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
    /// <summary>
    /// Shared visual constants and helper methods for consistent widget styling.
    /// All widgets should use these values to maintain a cohesive HUD appearance.
    /// </summary>
    public static class WidgetStyles
    {
        // Background
        public static readonly Color BgPrimary = new(0.08f, 0.09f, 0.12f, 0.94f);
        public static readonly Color BgHeader = new(0.05f, 0.08f, 0.1f, 0.15f);
        public static readonly float CornerRadius = 8f;

        // Text
        public static readonly Color TextPrimary = new(0.95f, 0.95f, 0.97f);
        public static readonly Color TextSecondary = new(0.6f, 0.63f, 0.7f);
        public static readonly Color TextMuted = new(0.4f, 0.42f, 0.48f);

        // Accent colors
        public static readonly Color AccentBlue = new(0.35f, 0.55f, 1f);
        public static readonly Color AccentGreen = new(0.3f, 0.85f, 0.5f);
        public static readonly Color AccentYellow = new(1f, 0.78f, 0.25f);
        public static readonly Color AccentRed = new(1f, 0.35f, 0.35f);
        public static readonly Color AccentCyan = new(0.3f, 0.85f, 0.9f);

        // Spacing
        public const float PaddingOuter = 22f;
        public const float PaddingInner = 8f;
        public const float HeaderHeight = 0.24f; // As fraction of widget height

        // Font sizes (scaled for world-space at 0.001 canvas scale)
        public const int FontSizeTitle = 13;
        public const int FontSizeValue = 30;
        public const int FontSizeUnit = 13;
        public const int FontSizeStatus = 11;

        /// <summary>
        /// Creates a styled background panel with optional border accent.
        /// </summary>
        public static Image CreateStyledBackground(Transform parent, Color bgColor, Color? borderColor = null)
        {
            if (parent.GetComponent<RectMask2D>() == null)
            {
                parent.gameObject.AddComponent<RectMask2D>();
            }

            // Main background
            var bgGo = new GameObject("Background");
            bgGo.transform.SetParent(parent, false);
            bgGo.transform.SetAsFirstSibling();

            var bgRect = bgGo.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            var bgImg = bgGo.AddComponent<Image>();
            bgImg.color = bgColor;

            // Top accent border
            if (borderColor.HasValue)
            {
                var borderGo = new GameObject("AccentBorder");
                borderGo.transform.SetParent(bgGo.transform, false);

                var borderRect = borderGo.AddComponent<RectTransform>();
                borderRect.anchorMin = new Vector2(0, 1f);
                borderRect.anchorMax = Vector2.one;
                borderRect.offsetMin = new Vector2(0, -4f);
                borderRect.offsetMax = Vector2.zero;

                var borderImg = borderGo.AddComponent<Image>();
                borderImg.color = borderColor.Value;
            }

            return bgImg;
        }

        /// <summary>
        /// Creates a header bar at the top of the widget with an icon label and title.
        /// </summary>
        public static TextMeshProUGUI CreateHeader(Transform parent, string icon, string title, Color accentColor)
        {
            var headerGo = new GameObject("Header");
            headerGo.transform.SetParent(parent, false);

            var headerRect = headerGo.AddComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0, 1f - HeaderHeight);
            headerRect.anchorMax = Vector2.one;
            headerRect.offsetMin = new Vector2(PaddingOuter, 0);
            headerRect.offsetMax = new Vector2(-PaddingOuter, 0);

            // Header background
            var tmp = headerGo.AddComponent<TextMeshProUGUI>();
            tmp.text = $"{icon} + {title}";
            tmp.fontSize = FontSizeTitle;
            tmp.color = accentColor;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            tmp.fontStyle = FontStyles.Bold;
            tmp.enableAutoSizing = true;
            tmp.fontSizeMin = 9;
            tmp.fontSizeMax = FontSizeTitle;
            tmp.overflowMode = TextOverflowModes.Truncate;
            tmp.margin = new Vector4(2f, 0f, 2f, 0f);

            return tmp;
        }

        /// <summary>
        /// Creates a large value display in the center of the widget.
        /// </summary>
        public static TextMeshProUGUI CreateValueDisplay(Transform parent, string defaultText = "--")
        {
            var go = new GameObject("Value");
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0.3f);
            rect.anchorMax = new Vector2(0.68f, 1f - HeaderHeight);
            rect.offsetMin = new Vector2(PaddingOuter, 0);
            rect.offsetMax = new Vector2(0, -PaddingInner);

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = defaultText;
            tmp.fontSize = FontSizeValue;
            tmp.color = TextPrimary;
            tmp.alignment = TextAlignmentOptions.BottomLeft;
            tmp.fontStyle = FontStyles.Bold;
            tmp.enableAutoSizing = true;
            tmp.fontSizeMin = 16;
            tmp.fontSizeMax = FontSizeValue;
            tmp.overflowMode = TextOverflowModes.Truncate;
            tmp.margin = new Vector4(2f, 0f, 2f, 0f);

            return tmp;
        }

        /// <summary>
        /// Creates a unit label next to the value.
        /// </summary>
        public static TextMeshProUGUI CreateUnitLabel(Transform parent, string unit)
        {
            var go = new GameObject("Unit");
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.68f, 0.33f);
            rect.anchorMax = new Vector2(1f, 0.56f);
            rect.offsetMin = new Vector2(PaddingInner, 0);
            rect.offsetMax = new Vector2(-PaddingOuter, 0);

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = unit;
            tmp.fontSize = FontSizeUnit;
            tmp.color = TextSecondary;
            tmp.alignment = TextAlignmentOptions.BottomLeft;
            tmp.enableAutoSizing = true;
            tmp.fontSizeMin = 9;
            tmp.fontSizeMax = FontSizeUnit;
            tmp.overflowMode = TextOverflowModes.Truncate;

            return tmp;
        }

        /// <summary>
        /// Creates a status bar at the bottom of the widget.
        /// </summary>
        public static TextMeshProUGUI CreateStatusBar(Transform parent, string defaultText = "")
        {
            var go = new GameObject("Status");
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = new Vector2(1f, 0.22f);
            rect.offsetMin = new Vector2(PaddingOuter, PaddingInner);
            rect.offsetMax = new Vector2(-PaddingOuter, 0);

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = defaultText;
            tmp.fontSize = FontSizeStatus;
            tmp.color = TextMuted;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            tmp.enableAutoSizing = true;
            tmp.fontSizeMin = 8;
            tmp.fontSizeMax = FontSizeStatus;
            tmp.overflowMode = TextOverflowModes.Truncate;
            tmp.margin = new Vector4(2f, 0f, 18f, 0f);

            return tmp;
        }

        /// <summary>
        /// Creates a small indicator dot for connection/signal status.
        /// </summary>
        public static Image CreateStatusDot(Transform parent, Color color)
        {
            var go = new GameObject("StatusDot");
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.anchoredPosition = new Vector2(-PaddingOuter, -42f);
            rect.sizeDelta = new Vector2(14f, 14f);

            var img = go.AddComponent<Image>();
            img.color = color;

            return img;
        }
    }
}
