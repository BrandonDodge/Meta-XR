/*
 Code Artifact: DemoLocomotionController.cs
 Description: Adds simple keyboard and Quest thumbstick movement for the HUD-Link virtual demo scene.
 Programmer: HudLink development team (Brandon Dodge, Zach Sevart, Asa Maker, Jonathan Gott, Josh Dwoskin)
 Date Created: 2026-04-26
 Revision History:
 - 2026-04-26 - HudLink development team - Added demo locomotion for editor and Quest presentation walkthroughs.
 Preconditions: The scene has a main camera, usually inside an OVRCameraRig, before normal frame updates begin.
 Acceptable Inputs: WASD or arrow keys, Q/E turn keys, left Quest thumbstick movement, and right Quest thumbstick snap turn.
 Unacceptable Inputs: Missing camera references, invalid rig transforms, or controller axes that do not report valid 2D values.
 Postconditions: The camera rig moves on the horizontal plane so the presenter can walk around the generated demo scene.
 Return Values: Unity lifecycle methods return void, and helper methods return vectors, floats, booleans, or transforms as named.
 Error and Exception Conditions: If no camera can be found, the script waits quietly until one exists instead of moving anything.
 Side Effects: Moves and rotates the active camera rig transform during play mode.
 Invariants: Movement is yaw-relative to the viewer, avoids vertical drift, and keeps snap turning on a short cooldown.
 Known Faults: This is demo locomotion only; it does not perform comfort vignettes, collision checks, or room-boundary enforcement.
 Major Blocks: Setup resolves the rig, input reading combines editor and Quest controls, and movement applies frame deltas.
 Line Comments: Important control mappings and rig choices are commented so the presenter can tune them quickly.
*/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using XRInputDevice = UnityEngine.XR.InputDevice;
using XRInputDevices = UnityEngine.XR.InputDevices;
using XRInputDeviceCharacteristics = UnityEngine.XR.InputDeviceCharacteristics;

namespace HudLink.Input
{
    /// <summary>
    /// Lightweight movement helper for sprint demos.
    /// It is intentionally small so it can be added at runtime without rebuilding the camera rig prefab.
    /// </summary>
    public sealed class DemoLocomotionController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform rigRoot;
        [SerializeField] private Transform headTransform;

        [Header("Movement")]
        [SerializeField] private float moveSpeedMetersPerSecond = 1.35f;
        [SerializeField] private float sprintMultiplier = 1.8f;
        [SerializeField] private float keyboardTurnDegreesPerSecond = 80f;
        [SerializeField] private float snapTurnDegrees = 30f;
        [SerializeField] private float snapTurnCooldownSeconds = 0.35f;
        [SerializeField] private float thumbstickDeadZone = 0.18f;

        private readonly List<XRInputDevice> _leftControllers = new List<XRInputDevice>();
        private readonly List<XRInputDevice> _rightControllers = new List<XRInputDevice>();
        private float _nextSnapTurnTime;

        public void Configure(Transform head)
        {
            // Bootstrap passes Camera.main when it is available.
            if (head != null)
            {
                headTransform = head;
            }

            ResolveRigReferences();
        }

        private void Awake()
        {
            ResolveRigReferences();
        }

        private void Update()
        {
            ResolveRigReferences();
            if (rigRoot == null || headTransform == null)
            {
                return;
            }

            Vector2 moveInput = Vector2.ClampMagnitude(ReadKeyboardMove() + ReadQuestMove(), 1f);
            ApplyHorizontalMove(moveInput);
            ApplyKeyboardTurn(ReadKeyboardTurn());
            ApplyQuestSnapTurn(ReadQuestTurn());
        }

        private void ResolveRigReferences()
        {
            if (headTransform == null)
            {
                Camera mainCamera = Camera.main;
                if (mainCamera == null)
                {
                    mainCamera = FindFirstObjectByType<Camera>();
                }

                if (mainCamera != null)
                {
                    headTransform = mainCamera.transform;
                }
            }

            if (rigRoot == null && headTransform != null)
            {
                rigRoot = FindRigRoot(headTransform);
            }
        }

        private static Transform FindRigRoot(Transform start)
        {
            Transform current = start;
            Transform highestParent = start;

            // Prefer the OVR camera rig root when the scene uses the Meta prefab.
            while (current != null)
            {
                if (current.name.Contains("OVRCameraRig"))
                {
                    return current;
                }

                highestParent = current;
                current = current.parent;
            }

            return highestParent;
        }

        private Vector2 ReadKeyboardMove()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return Vector2.zero;
            }

            float x = 0f;
            float y = 0f;

            // WASD and arrows both work so the demo can be driven from most keyboards.
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            {
                x -= 1f;
            }

            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            {
                x += 1f;
            }

            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
            {
                y += 1f;
            }

            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
            {
                y -= 1f;
            }

            return Vector2.ClampMagnitude(new Vector2(x, y), 1f);
        }

        private Vector2 ReadQuestMove()
        {
            return ReadThumbstick(
                XRInputDeviceCharacteristics.Controller | XRInputDeviceCharacteristics.HeldInHand | XRInputDeviceCharacteristics.Left,
                _leftControllers);
        }

        private float ReadKeyboardTurn()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return 0f;
            }

            float turn = 0f;
            if (keyboard.qKey.isPressed)
            {
                turn -= 1f;
            }

            if (keyboard.eKey.isPressed)
            {
                turn += 1f;
            }

            return turn;
        }

        private float ReadQuestTurn()
        {
            Vector2 axis = ReadThumbstick(
                XRInputDeviceCharacteristics.Controller | XRInputDeviceCharacteristics.HeldInHand | XRInputDeviceCharacteristics.Right,
                _rightControllers);
            return axis.x;
        }

        private Vector2 ReadThumbstick(XRInputDeviceCharacteristics characteristics, List<XRInputDevice> devices)
        {
            devices.Clear();
            XRInputDevices.GetDevicesWithCharacteristics(characteristics, devices);

            for (int i = 0; i < devices.Count; i++)
            {
                if (devices[i].TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out Vector2 axis)
                    && axis.magnitude >= thumbstickDeadZone)
                {
                    return axis;
                }
            }

            return Vector2.zero;
        }

        private void ApplyHorizontalMove(Vector2 input)
        {
            if (input.sqrMagnitude < 0.001f)
            {
                return;
            }

            Vector3 forward = headTransform.forward;
            Vector3 right = headTransform.right;
            forward.y = 0f;
            right.y = 0f;

            if (forward.sqrMagnitude < 0.001f)
            {
                forward = rigRoot.forward;
                forward.y = 0f;
            }

            forward.Normalize();
            right.Normalize();

            float speed = moveSpeedMetersPerSecond * GetKeyboardSpeedMultiplier();
            Vector3 worldMove = (forward * input.y + right * input.x) * speed * Time.deltaTime;
            rigRoot.position += worldMove;
        }

        private float GetKeyboardSpeedMultiplier()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && (keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed))
            {
                return sprintMultiplier;
            }

            return 1f;
        }

        private void ApplyKeyboardTurn(float turnInput)
        {
            if (Mathf.Abs(turnInput) < 0.001f)
            {
                return;
            }

            RotateAroundHead(turnInput * keyboardTurnDegreesPerSecond * Time.deltaTime);
        }

        private void ApplyQuestSnapTurn(float turnInput)
        {
            if (Time.time < _nextSnapTurnTime || Mathf.Abs(turnInput) < 0.72f)
            {
                return;
            }

            RotateAroundHead(Mathf.Sign(turnInput) * snapTurnDegrees);
            _nextSnapTurnTime = Time.time + snapTurnCooldownSeconds;
        }

        private void RotateAroundHead(float degrees)
        {
            Vector3 pivot = headTransform != null ? headTransform.position : rigRoot.position;
            rigRoot.RotateAround(pivot, Vector3.up, degrees);
        }
    }
}
