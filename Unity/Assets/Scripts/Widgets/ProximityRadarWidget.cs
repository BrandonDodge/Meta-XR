/*
 Code Artifact: ProximityRadarWidget.cs
 Description: Shows a nearby target on a small radar display using distance and bearing event data.
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
    /// High-order experimental tracking widget (Sprint 6 target).
    /// Tracks friends or points of interest relative to user gaze/heading in a mini-map radar layout.
    /// </summary>
    public class ProximityRadarWidget : BaseWidget
    {
        [Header("Radar Settings")]
        [SerializeField] private float maxRadarRangeMeters = 50f;
        [SerializeField] private GameObject blipPrefab;
        [SerializeField] private Transform radarCenter;
        
        // Active tracking
        private string trackingId;
        private Vector2 bearingCoordinates; // Map user directly

        public override void Initialize(RectTransform slot)
        {
            base.Initialize(slot);
            WidgetEventBus.Subscribe<ProximityAlertEvent>(OnProximityUpdate);
            Debug.Log($"[{WidgetId}] ProximityRadarWidget Initialized.");
        }

        public override void UpdateData(WidgetData data)
        {
            // This widget currently receives updates via WidgetEventBus.
        }

        public override void Dispose()
        {
            WidgetEventBus.Unsubscribe<ProximityAlertEvent>(OnProximityUpdate);
            base.Dispose();
        }

        private void OnProximityUpdate(ProximityAlertEvent proxEvent)
        {
            // In a real application, we would manage a pool of targets.
            // For this impressive demo, we track the loudest proximity alert.
            trackingId = proxEvent.TargetId;
            bearingCoordinates = proxEvent.RelativeBearing;
            
            float dist = proxEvent.DistanceMeters;
            float scaleFactor = Mathf.Clamp01(1.0f - (dist / maxRadarRangeMeters));
            
            Debug.Log($"[{WidgetId}] Radar Target '{trackingId}' locked. Dist: {dist}m, Scale Output: {scaleFactor}");
            
            UpdateRadarVisuals(scaleFactor);
        }

        private void UpdateRadarVisuals(float scale)
        {
            if (blipPrefab != null && radarCenter != null)
            {
                // Position the blip relative to bearing polar coordinates
                blipPrefab.transform.localPosition = new Vector3(bearingCoordinates.x, bearingCoordinates.y, 0);
                
                // Scale it up if they are closer to you
                blipPrefab.transform.localScale = Vector3.one * (scale * 2f);
            }
        }
    }
}
