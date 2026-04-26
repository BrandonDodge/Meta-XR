/*
 Code Artifact: StressMonitorWidget.cs
 Description: Uses HRV stress events to fade a warning vignette when stress passes a configured threshold.
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
    /// Tracks user stress parameters using derived Heart Rate Variability (HRV).
    /// Warns the user or tints the screen edge if stress climbs into a 'fight or flight' zone.
    /// </summary>
    public class StressMonitorWidget : BaseWidget
    {
        [Header("Biofeedback Settings")]
        [Range(0f, 1f)] public float StressThreshold = 0.7f;
        [SerializeField] private CanvasGroup warningVignette;

        private float currentStress = 0f;

        public override void Initialize(RectTransform slot)
        {
            base.Initialize(slot);
            WidgetEventBus.Subscribe<HRVStressEvent>(OnHRVUpdate);
            if (warningVignette != null) warningVignette.alpha = 0f;
            Debug.Log($"[{WidgetId}] StressMonitorWidget Initialized.");
        }

        public override void UpdateData(WidgetData data)
        {
            // This widget currently receives updates via WidgetEventBus.
        }

        public override void Dispose()
        {
            WidgetEventBus.Unsubscribe<HRVStressEvent>(OnHRVUpdate);
            base.Dispose();
        }

        private void OnHRVUpdate(HRVStressEvent stressEvent)
        {
            currentStress = stressEvent.StressLevel;
        }

        private void Update()
        {
            if (!Initialized || warningVignette == null) return;

            // Interpolate a subtle red/orange vignette on the AR display 
            // if the user's autonomic system indicates elevated stress.
            float targetAlpha = currentStress > StressThreshold ? (currentStress - StressThreshold) * 2f : 0f;
            warningVignette.alpha = Mathf.Lerp(warningVignette.alpha, targetAlpha, Time.deltaTime);
        }
    }
}
