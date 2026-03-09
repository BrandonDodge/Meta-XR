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
