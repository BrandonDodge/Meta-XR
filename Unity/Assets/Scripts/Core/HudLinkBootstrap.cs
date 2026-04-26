/*
 Code Artifact: HudLinkBootstrap.cs
 Description: Starts the Unity HUD scene, prepares the camera for passthrough, creates default widgets, and registers them in grid slots.
 Programmer: HudLink development team (Brandon Dodge, Zach Sevart, Asa Maker, Jonathan Gott, Josh Dwoskin)
 Date Created: 2026-03-09
 Revision History:
 - 2026-04-13 - Brandon.Dodge - Update contribution ledger and Unity scripts
 - 2026-04-26 - HudLink development team - Added release prologue comments and tightened sprint-review documentation.
 - 2026-04-26 - HudLink development team - Started the virtual demo and demo locomotion helpers at runtime.
 - 2026-04-26 - HudLink development team - Hid legacy head-follow widgets when the virtual demo is active.
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
using HudLink.HUD;
using HudLink.Input;
using HudLink.Widgets;

namespace HudLink.Core
{
    /// <summary>
    /// Scene entry point. Initializes the HUD, creates widgets, and assigns them to grid slots.
    /// Attach this to an empty GameObject in the scene root.
    /// </summary>
    public class HudLinkBootstrap : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private HudController hudController;
        [SerializeField] private HudGridLayout gridLayout;

        [Header("Widget Prefabs")]
        [SerializeField] private GameObject heartRateWidgetPrefab;
        [SerializeField] private GameObject gpsWidgetPrefab;
        [SerializeField] private GameObject notificationWidgetPrefab;

        [Header("Default Widget Placement")]
        [SerializeField]
        private HudGridLayout.SlotPosition heartRateSlot = HudGridLayout.SlotPosition.TopLeft;
        [SerializeField]
        private HudGridLayout.SlotPosition gpsSlot = HudGridLayout.SlotPosition.BottomRight;
        [SerializeField]
        private HudGridLayout.SlotPosition notificationSlot = HudGridLayout.SlotPosition.TopRight;

        [Header("Virtual Demo")]
        [SerializeField] private bool disableVirtualDemo;
        [SerializeField] private bool showDefaultWidgetsInVirtualDemo;
        [SerializeField] private bool enableDemoLocomotion = true;
        [SerializeField] private VirtualDemoManager virtualDemoManager;
        [SerializeField] private DemoLocomotionController demoLocomotionController;

        private void Start()
        {
            // Apply camera passthrough first so the HUD initializes against the final scene view.
            EnablePassthrough();
            if (disableVirtualDemo || showDefaultWidgetsInVirtualDemo)
            {
                SetupDefaultWidgets();
            }
            else if (hudController != null)
            {
                hudController.SetHudVisible(false);
            }

            SetupVirtualDemo();
            SetupDemoLocomotion();
        }

        private void EnablePassthrough()
        {
            // Set camera to render nothing so passthrough shows the real world
            var cam = Camera.main;
            if (cam != null)
            {
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = Color.clear;
            }
        }

        private void SetupDefaultWidgets()
        {
            // Register only the default widgets that are wired in the scene.
            if (heartRateWidgetPrefab != null)
            {
                var hrWidget = CreateWidget<HeartRateWidget>(heartRateWidgetPrefab, "heart_rate");
                if (hrWidget != null)
                {
                    hudController.RegisterWidget(hrWidget, heartRateSlot);
                }
            }

            if (gpsWidgetPrefab != null)
            {
                var gpsWidget = CreateWidget<GPSWidget>(gpsWidgetPrefab, "gps");
                if (gpsWidget != null)
                {
                    hudController.RegisterWidget(gpsWidget, gpsSlot);
                }
            }

            if (notificationWidgetPrefab != null)
            {
                var notifWidget = CreateWidget<NotificationWidget>(notificationWidgetPrefab, "notifications");
                if (notifWidget != null)
                {
                    hudController.RegisterWidget(notifWidget, notificationSlot);
                }
            }
        }

        private T CreateWidget<T>(GameObject prefab, string widgetId) where T : BaseWidget
        {
            var go = Instantiate(prefab);
            go.name = $"Widget_{widgetId}";
            // Prefabs are expected to already carry the requested widget component.
            var widget = go.GetComponent<T>();
            if (widget == null)
            {
                Debug.LogError($"[HudLink] Prefab for '{widgetId}' is missing {typeof(T).Name}.");
                Destroy(go);
            }
            return widget;
        }

        private void SetupVirtualDemo()
        {
            if (disableVirtualDemo)
            {
                return;
            }

            // Add the virtual demo at runtime so the current scene remains the single build scene.
            if (virtualDemoManager == null)
            {
                virtualDemoManager = GetComponent<VirtualDemoManager>();
            }

            if (virtualDemoManager == null)
            {
                virtualDemoManager = gameObject.AddComponent<VirtualDemoManager>();
            }

            virtualDemoManager.Configure(hudController);
        }

        private void SetupDemoLocomotion()
        {
            if (!enableDemoLocomotion)
            {
                return;
            }

            // The movement helper is runtime-only so the OVRCameraRig prefab stays untouched.
            if (demoLocomotionController == null)
            {
                demoLocomotionController = FindFirstObjectByType<DemoLocomotionController>();
            }

            if (demoLocomotionController == null)
            {
                demoLocomotionController = gameObject.AddComponent<DemoLocomotionController>();
            }

            Camera mainCamera = Camera.main;
            demoLocomotionController.Configure(mainCamera != null ? mainCamera.transform : null);
        }
    }
}
