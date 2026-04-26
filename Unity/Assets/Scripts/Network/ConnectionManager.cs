/*
 Code Artifact: ConnectionManager.cs
 Description: Simulates Android bridge connection state changes, retries, and event publication for reliability testing.
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
using System;
using UnityEngine;
using HudLink.Events;
using HudLink.Widgets;

namespace HudLink.Network
{
    /// <summary>
    /// Represents the overall status of the Android to Unity data pipeline.
    /// Broadcast to all widgets so they can enter fallback or connected states.
    /// FR-4.1 Connection Reliability.
    /// </summary>
    public class ConnectionStatusEvent : HUDEvent
    {
        public bool IsConnected { get; private set; }
        public string SourceName { get; private set; }

        public ConnectionStatusEvent(int v, bool connected, string sourceName = "AndroidBridge") 
        {
            Version = v;
            IsConnected = connected;
            SourceName = sourceName;
        }
    }

    /// <summary>
    /// Simulates/Handles connection states, retries, and data bridging.
    /// Meets FR-4.1 (Connection loss detection, retry, safe states).
    /// </summary>
    public class ConnectionManager : MonoBehaviour
    {
        [Header("Connection State")]
        public bool IsSimulatedConnected = true;
        
        [Header("Testing Controls")]
        [Tooltip("Check this box in the inspector to simulate a network drop")]
        public bool SimulateDisconnect = false;

        private bool wasConnected = false;
        private float disconnectTimer = 0f;
        private float retryInterval = 5f; // Attempts to reconnect every 5s if disconnected

        private void Start()
        {
            wasConnected = !IsSimulatedConnected; // force first update
            CheckConnectionState();
        }

        private void Update()
        {
            // For Sprint 4, we use the inspector toggle to simulate drops
            if (SimulateDisconnect && IsSimulatedConnected)
            {
                IsSimulatedConnected = false;
            }
            else if (!SimulateDisconnect && !IsSimulatedConnected)
            {
                // Retry locally so disconnected-state UX can be tested without the Android bridge online.
                disconnectTimer += Time.deltaTime;
                if (disconnectTimer >= retryInterval)
                {
                    Debug.Log("[ConnectionManager] Retrying connection...");
                    IsSimulatedConnected = true; // Simulating successful reconnect
                    disconnectTimer = 0f;
                }
            }

            CheckConnectionState();
        }

        private void CheckConnectionState()
        {
            if (IsSimulatedConnected != wasConnected)
            {
                wasConnected = IsSimulatedConnected;
                Debug.Log($"[ConnectionManager] Status changed: IsConnected = {wasConnected}");
                
                // Publish once per state edge so widgets can swap between live and fallback states.
                GlobalEventBus.Publish(new ConnectionStatusEvent(1, wasConnected));
            }
        }
    }
}
