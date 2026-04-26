/*
 Code Artifact: HudToggleInput.cs
 Description: Connects controller and editor keyboard input to the HUD visibility toggle.
 Programmer: HudLink development team (Brandon Dodge, Zach Sevart, Asa Maker, Jonathan Gott, Josh Dwoskin)
 Date Created: 2026-03-09
 Revision History:
 - 2026-03-09 - Zach Sevart - Add HUD widget framework and scene
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
using UnityEngine.InputSystem;
using HudLink.HUD;

namespace HudLink.Input
{
    /// <summary>
    /// Handles controller input to toggle the HUD on/off.
    /// Maps to a configurable InputActionReference (default: menu button).
    /// R5 acceptance criteria: "HUD can be enabled/disabled via a controller gesture/menu toggle."
    /// </summary>
    public class HudToggleInput : MonoBehaviour
    {
        [SerializeField] private HudController hudController;
        [SerializeField] private InputActionReference toggleAction;

        private void OnEnable()
        {
            if (toggleAction != null && toggleAction.action != null)
            {
                toggleAction.action.Enable();
                toggleAction.action.performed += OnTogglePerformed;
            }
        }

        private void OnDisable()
        {
            if (toggleAction != null && toggleAction.action != null)
            {
                toggleAction.action.performed -= OnTogglePerformed;
                toggleAction.action.Disable();
            }
        }

        private void OnTogglePerformed(InputAction.CallbackContext ctx)
        {
            hudController.ToggleHud();
        }

        // Fallback: keyboard toggle for editor testing
        private InputAction _keyboardToggle;

        private void Start()
        {
            _keyboardToggle = new InputAction("HudToggleKey", binding: "<Keyboard>/h");
            _keyboardToggle.performed += _ => hudController.ToggleHud();
            _keyboardToggle.Enable();
        }

        private void OnDestroy()
        {
            _keyboardToggle?.Dispose();
        }
    }
}
