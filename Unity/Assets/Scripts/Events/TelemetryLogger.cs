using System;
using System.Collections.Generic;
using UnityEngine;
using HudLink.Widgets;

namespace HudLink.Events
{
    /// <summary>
    /// Telemetry & Debug Logging for the HUDLink system.
    /// Implements FR-4.3 for Sprint 4.
    /// </summary>
    public class TelemetryLogger : MonoBehaviour
    {
        [Header("Telemetry Settings")]
        [Tooltip("Enable detailed message timing logs")]
        public bool LogMessageTiming = false;

        [Tooltip("Max latency allowed before a warning is logged (seconds)")]
        public float MaxAllowedLatency = 3.0f;

        private void OnEnable()
        {
            // Using generic subscription model, we could in theory subscribe to all, 
            // but for Sprint 4 we focus on the important system flow events.
            WidgetEventBus.Subscribe<HeartRateEvent>(OnHeartRateReceived);
            // More event subscriptions could be added here
        }

        private void OnDisable()
        {
            WidgetEventBus.Unsubscribe<HeartRateEvent>(OnHeartRateReceived);
        }

        private void OnHeartRateReceived(HeartRateEvent hrEvent)
        {
            float latency = Time.time - hrEvent.Timestamp;
            
            if (LogMessageTiming)
            {
                Debug.Log($"[Telemetry] HeartRateEvent received. Ver: {hrEvent.Version}. UI Latency: {latency:F3}s.");
            }

            if (latency > MaxAllowedLatency)
            {
                Debug.LogWarning($"[Telemetry] High latency detected on HR: {latency:F3}s > {MaxAllowedLatency}s!");
            }

            // Detect invalid payloads
            if (hrEvent.BPM <= 0 || hrEvent.BPM >= 300)
            {
                Debug.LogError($"[Telemetry] Invalid payload detected: BPM is OUT OF RANGE ({hrEvent.BPM})");
            }
        }
        
        // Listen to connection drops
        private void Awake()
        {
            GlobalEventBus.Subscribe<HudLink.Network.ConnectionStatusEvent>(OnConnectionUpdate);
        }
        
        private void OnDestroy()
        {
            GlobalEventBus.Unsubscribe<HudLink.Network.ConnectionStatusEvent>(OnConnectionUpdate);
        }

        private void OnConnectionUpdate(HudLink.Network.ConnectionStatusEvent ev)
        {
            Debug.Log($"[Telemetry] Connection event triggered. Connected: {ev.IsConnected}. Time: {ev.Timestamp}");
        }
    }
}
