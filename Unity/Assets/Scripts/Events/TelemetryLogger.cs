// See HudLink.Core.ProjectContributionLedger for sprint attribution and dated maintenance notes.
using UnityEngine;

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
            // Widget telemetry can be reconnected here if the data pipeline emits typed timing events again.
        }

        private void OnDisable()
        {
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
            // Connection transitions are still logged so headset-side testing keeps observability.
            Debug.Log($"[Telemetry] Connection event triggered. Connected: {ev.IsConnected}. Time: {ev.Timestamp}");
        }
    }
}
