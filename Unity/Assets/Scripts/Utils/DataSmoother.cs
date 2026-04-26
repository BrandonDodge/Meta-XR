/*
 Code Artifact: DataSmoother.cs
 Description: Smooths numeric sensor values so widgets do not jump sharply between samples.
 Programmer: HudLink development team (Brandon Dodge, Zach Sevart, Asa Maker, Jonathan Gott, Josh Dwoskin)
 Date Created: 2026-03-23
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

namespace HudLink.Utils
{
    /// <summary>
    /// Handles buffering and smoothing of incoming data.
    /// Reduces abrupt value jumps and prevents noisy UI behavior.
    /// Meets FR-4.2 (Latency & Smoothing).
    /// </summary>
    public class DataSmoother
    {
        private float currentValue;
        private float targetValue;
        private float smoothSpeed;
        private bool isInitialized = false;

        public DataSmoother(float initialSmoothSpeed = 5f)
        {
            smoothSpeed = initialSmoothSpeed;
        }

        /// <summary>
        /// Updates the target destination for the smoothing function.
        /// </summary>
        public void SetTarget(float newTarget)
        {
            targetValue = newTarget;
            if (!isInitialized)
            {
                // First sample snaps immediately so widgets do not animate in from zero.
                currentValue = targetValue;
                isInitialized = true;
            }
        }

        /// <summary>
        /// Call periodically to tick the smoothed value closer to the target.
        /// Returns the new smoothed value.
        /// </summary>
        public float Update(float deltaTime)
        {
            if (!isInitialized) return 0f;

            // Simple Lerp smoothing. Could be replaced with moving average or PID if needed later.
            currentValue = Mathf.Lerp(currentValue, targetValue, smoothSpeed * deltaTime);
            return currentValue;
        }

        public float GetCurrent() => currentValue;
        
        /// <summary>
        /// Instantly snaps the current value to the target.
        /// Useful when the connection is restored after a long dropout.
        /// </summary>
        public void SnapToTarget()
        {
            currentValue = targetValue;
        }
        
        public void Reset()
        {
            // Reset forces the next sample to become the new baseline.
            isInitialized = false;
        }
    }
}
