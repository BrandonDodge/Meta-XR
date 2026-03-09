using UnityEngine;
using HudLink.HUD;
using HudLink.Widgets;

namespace HudLink.Data
{
    /// <summary>
    /// Generates mock health/location/notification data to drive widgets
    /// until the BLE bridge is implemented. Simulates realistic data patterns.
    /// </summary>
    public class MockDataProvider : MonoBehaviour
    {
        [SerializeField] private HudController hudController;
        [SerializeField] private float heartRateUpdateInterval = 1.0f;
        [SerializeField] private float gpsUpdateInterval = 2.0f;

        [Header("Heart Rate Simulation")]
        [SerializeField] private int baseHeartRate = 72;
        [SerializeField] private int heartRateVariance = 8;

        [Header("GPS Simulation")]
        [SerializeField] private float baseSpeedMph = 3.5f;
        [SerializeField] private float speedVariance = 1.0f;

        private float _hrTimer;
        private float _gpsTimer;
        private float _headingDrift;

        private void Update()
        {
            _hrTimer += Time.deltaTime;
            _gpsTimer += Time.deltaTime;

            if (_hrTimer >= heartRateUpdateInterval)
            {
                _hrTimer = 0f;
                PushHeartRateUpdate();
            }

            if (_gpsTimer >= gpsUpdateInterval)
            {
                _gpsTimer = 0f;
                PushGpsUpdate();
            }
        }

        private void PushHeartRateUpdate()
        {
            var data = new HeartRateWidgetData
            {
                Bpm = baseHeartRate + Random.Range(-heartRateVariance, heartRateVariance + 1),
                IsValid = true,
                TimestampMs = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Source = DataSource.Mock
            };

            hudController.UpdateWidget("heart_rate", data);
        }

        private void PushGpsUpdate()
        {
            _headingDrift += Random.Range(-10f, 10f);
            float heading = Mathf.Repeat(_headingDrift, 360f);

            var data = new GpsWidgetData
            {
                SpeedMph = Mathf.Max(0, baseSpeedMph + Random.Range(-speedVariance, speedVariance)),
                HeadingDegrees = heading,
                Latitude = 38.9543f + Random.Range(-0.0001f, 0.0001f),  // Lawrence, KS area
                Longitude = -95.2558f + Random.Range(-0.0001f, 0.0001f),
                HasFix = true,
                TimestampMs = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Source = DataSource.Mock
            };

            hudController.UpdateWidget("gps", data);
        }

        /// <summary>
        /// Call this to simulate a notification arriving.
        /// </summary>
        public void SimulateNotification(string appName, string title, bool redacted = false)
        {
            var data = new NotificationWidgetData
            {
                AppName = appName,
                Title = title,
                IsRedacted = redacted,
                TimestampMs = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Source = DataSource.Mock
            };

            hudController.UpdateWidget("notifications", data);
        }
    }
}
