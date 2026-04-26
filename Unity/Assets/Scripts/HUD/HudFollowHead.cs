/*
 Code Artifact: HudFollowHead.cs
 Description: Keeps the world-space HUD near the user's view with smooth tag-along movement.
 Programmer: HudLink development team (Brandon Dodge, Zach Sevart, Asa Maker, Jonathan Gott, Josh Dwoskin)
 Date Created: 2026-03-09
 Revision History:
 - 2026-03-09 - Zach Sevart - Add HUD widget framework and scene
 - 2026-04-26 - HudLink development team - Added release prologue comments and tightened sprint-review documentation.
 - 2026-04-26 - HudLink development team - Tuned headset HUD distance and follow feel to match the virtual demo scale.
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

namespace HudLink.HUD
{
    /// <summary>
    /// Makes the HUD canvas follow the user's head with smooth lag.
    /// Uses a "tag-along" behavior: the HUD stays in place until the user
    /// turns beyond a threshold, then smoothly catches up. This prevents
    /// the HUD from feeling rigidly head-locked (which causes nausea)
    /// while keeping it accessible.
    /// </summary>
    public class HudFollowHead : MonoBehaviour
    {
        [SerializeField] private Transform headTransform;
        [SerializeField] private float distanceFromHead = 1.15f;
        [SerializeField] private float followSpeed = 4.0f;
        [SerializeField] private float angleThreshold = 12f;

        private Vector3 _targetPosition;
        private Quaternion _targetRotation;
        private bool _needsUpdate = true;

        private void Start()
        {
            if (headTransform == null)
            {
                var cam = Camera.main;
                if (cam != null)
                    headTransform = cam.transform;
            }

            // Snap to initial position
            UpdateTarget();
            transform.position = _targetPosition;
            transform.rotation = _targetRotation;
        }

        private void LateUpdate()
        {
            if (headTransform == null) return;

            float angle = Quaternion.Angle(transform.rotation, GetDesiredRotation());

            if (angle > angleThreshold)
                _needsUpdate = true;

            if (_needsUpdate)
            {
                UpdateTarget();

                transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * followSpeed);
                transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * followSpeed);

                if (angle < 1f)
                    _needsUpdate = false;
            }
        }

        private void UpdateTarget()
        {
            _targetPosition = headTransform.position + headTransform.forward * distanceFromHead;
            _targetRotation = GetDesiredRotation();
        }

        private Quaternion GetDesiredRotation()
        {
            // Face the HUD toward the user; only use Y rotation from head to prevent tilting.
            Vector3 forward = headTransform.forward;
            forward.y = 0;
            if (forward.sqrMagnitude < 0.001f)
                forward = Vector3.forward;
            return Quaternion.LookRotation(forward.normalized);
        }
    }
}
