/*
 Code Artifact: EnvironmentWidget.cs
 Description: Receives environmental readings and chooses a display color for weather, UV, and air-quality status.
 Programmer: HudLink development team (Brandon Dodge, Zach Sevart, Asa Maker, Jonathan Gott, Josh Dwoskin)
 Date Created: 2026-03-06
 Revision History:
 - 2026-04-13 - Brandon.Dodge - Update contribution ledger and Unity scripts
 - 2026-04-26 - HudLink development team - Added release prologue comments and tightened sprint-review documentation.
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

namespace HudLink.Widgets
{
    /// <summary>
    /// Consolidates external contextual data (Weather, AQI, UV) into a simple AR badge.
    /// Highly useful for running/cycling scenarios in AR glasses.
    /// </summary>
    public class EnvironmentWidget : BaseWidget
    {
        [Header("Environmental Thresholds")]
        [SerializeField] private Color safeAQIColor = Color.green;
        [SerializeField] private Color warningAQIColor = new Color(1f, 0.5f, 0f); // Orange
        [SerializeField] private Color hazardousAQIColor = Color.red;

        private float tempC;
        private int aqi;
        private int uv;

        public override void Initialize(RectTransform slot)
        {
            base.Initialize(slot);
            WidgetEventBus.Subscribe<EnvironmentDataEvent>(OnEnvironmentUpdate);
            Debug.Log($"[{WidgetId}] EnvironmentWidget Initialized.");
        }

        public override void UpdateData(WidgetData data)
        {
            // This widget currently receives updates via WidgetEventBus.
        }

        public override void Dispose()
        {
            WidgetEventBus.Unsubscribe<EnvironmentDataEvent>(OnEnvironmentUpdate);
            base.Dispose();
        }

        private void OnEnvironmentUpdate(EnvironmentDataEvent envData)
        {
            tempC = envData.TemperatureC;
            aqi = envData.AirQualityIndex;
            uv = envData.UVIndex;
            UpdateEnvironmentalVisuals();
        }

        private void UpdateEnvironmentalVisuals()
        {
            Color currentAQIColor = safeAQIColor;
            if (aqi > 100) currentAQIColor = warningAQIColor;
            if (aqi > 150) currentAQIColor = hazardousAQIColor;

            // In a real implementation we update a Canvas / TextMeshPro element here
            Debug.Log($"[{WidgetId}] Weather: {tempC}\u00b0C | UV: {uv} | AQI: {aqi} (Color: {currentAQIColor})");
        }
    }
}
