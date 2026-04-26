/*
 Code Artifact: ConnectionStatusDisplay.cs
 Description: Displays the current data connection state on the HUD using short text and status colors.
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
using TMPro;

namespace HudLink.Utils
{
    /// <summary>
    /// Displays connection status indicator on the HUD.
    /// R4/R11 acceptance criteria: "Connection status is visible (Connected / Degraded / Disconnected)."
    /// For now, always shows "Mock Data" since BLE isn't implemented yet.
    /// </summary>
    public class ConnectionStatusDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI statusText;

        public enum ConnectionState
        {
            Disconnected,
            Connecting,
            Connected,
            Degraded,
            MockData
        }

        private ConnectionState _currentState = ConnectionState.MockData;

        public ConnectionState CurrentState => _currentState;

        private void Start()
        {
            SetState(ConnectionState.MockData);
        }

        public void SetState(ConnectionState state)
        {
            _currentState = state;

            if (statusText == null) return;

            switch (state)
            {
                case ConnectionState.Disconnected:
                    statusText.text = "Disconnected";
                    statusText.color = new Color(1f, 0.4f, 0.4f);
                    break;
                case ConnectionState.Connecting:
                    statusText.text = "Connecting...";
                    statusText.color = new Color(1f, 0.85f, 0.3f);
                    break;
                case ConnectionState.Connected:
                    statusText.text = "Connected";
                    statusText.color = new Color(0.4f, 1f, 0.5f);
                    break;
                case ConnectionState.Degraded:
                    statusText.text = "Degraded";
                    statusText.color = new Color(1f, 0.85f, 0.3f);
                    break;
                case ConnectionState.MockData:
                    statusText.text = "Mock Data";
                    statusText.color = new Color(0.5f, 0.7f, 1f);
                    break;
            }
        }
    }
}
