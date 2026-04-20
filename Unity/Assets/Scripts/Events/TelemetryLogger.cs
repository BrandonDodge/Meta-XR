using UnityEngine;

namespace HudLink.Events
{
    /// <summary>
    /// Telemetry and debug logging for the HudLink system.
    /// Logs performance metrics and connection events.
    /// </summary>
    public class TelemetryLogger : MonoBehaviour
    {
        [Header("Telemetry Settings")]
        public bool LogFrameTiming = false;
        public float MaxAllowedLatency = 3.0f;

        private float _frameTimer;
        private int _frameCount;
        private float _fps;

        private void Update()
        {
            if (!LogFrameTiming) return;

            _frameTimer += Time.unscaledDeltaTime;
            _frameCount++;

            if (_frameTimer >= 1f)
            {
                _fps = _frameCount / _frameTimer;
                if (_fps < 72f)
                {
                    Debug.LogWarning($"[Telemetry] FPS below target: {_fps:F1} (target: 72)");
                }
                _frameTimer = 0f;
                _frameCount = 0;
            }
        }
    }
}
