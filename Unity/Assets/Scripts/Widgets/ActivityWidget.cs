/*
 Code Artifact: ActivityWidget.cs
 Description: Displays activity progress with an animated ring that responds to activity events.
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
using UnityEngine.UI;

namespace HudLink.Widgets
{
    /// <summary>
    /// Advanced visualization for daily fitness metrics (Steps, Calories, Minutes).
    /// Features smooth interpolation of circular fill amounts (like Activity Rings).
    /// </summary>
    public class ActivityWidget : BaseWidget
    {
        [Header("Activity Rings UI")]
        [Tooltip("The circular image representing daily goal progress.")]
        [SerializeField] private Image progressRingFill;
        [SerializeField] private float animationSpeed = 2f;

        private float targetFillAmount = 0f;
        private float currentFillAmount = 0f;

        public override void Initialize(RectTransform slot)
        {
            base.Initialize(slot);
            WidgetEventBus.Subscribe<ActivityEvent>(OnActivityUpdate);
            if (progressRingFill != null) progressRingFill.fillAmount = 0;
            Debug.Log($"[{WidgetId}] ActivityWidget Initialized.");
        }

        public override void UpdateData(WidgetData data)
        {
            // This widget currently receives updates via WidgetEventBus.
        }

        public override void Dispose()
        {
            WidgetEventBus.Unsubscribe<ActivityEvent>(OnActivityUpdate);
            base.Dispose();
        }

        private void OnActivityUpdate(ActivityEvent activityData)
        {
            // Clamp percentage and cache the target for smooth animation over time
            targetFillAmount = Mathf.Clamp01(activityData.DailyGoalPercentage);
            Debug.Log($"[{WidgetId}] Activity Goal updated to {activityData.DailyGoalPercentage * 100}%");
        }

        private void Update()
        {
            if (!Initialized) return;

            // Smoothly animate the ring fill based on the target value
            if (Mathf.Abs(currentFillAmount - targetFillAmount) > 0.001f)
            {
                currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, Time.deltaTime * animationSpeed);
                if (progressRingFill != null)
                {
                    progressRingFill.fillAmount = currentFillAmount;
                    
                    // Dynamic color switching: Green if complete, cyan if in progress
                    progressRingFill.color = currentFillAmount >= 0.99f ? Color.green : Color.cyan;
                }
            }
        }
    }
}
