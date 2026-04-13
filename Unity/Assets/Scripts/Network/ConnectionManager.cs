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
                // Simple simulated retry logic
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
                
                // Broadcast to HUD widgets
                GlobalEventBus.Publish(new ConnectionStatusEvent(1, wasConnected));
            }
        }
    }
}
