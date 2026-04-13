// See HudLink.Core.ProjectContributionLedger for sprint attribution and dated maintenance notes.
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

        [Header("Notification Simulation")]
        [SerializeField] private float notificationInterval = 8.0f;

        private float _hrTimer;
        private float _gpsTimer;
        private float _notifTimer;
        private float _headingDrift;
        private int _notifIndex;

        private static readonly (string app, string title)[] MockNotifications =
        {
            ("Messages", "Hey, are you coming to the meeting?"),
            ("Calendar", "Team standup in 10 minutes"),
            ("Email", "PR #42 approved - ready to merge"),
            ("Slack", "New message in #general"),
            ("Phone", "Missed call from Brandon"),
        };

        private void Update()
        {
            // Each timer advances independently so widgets refresh on realistic, staggered cadences.
            _hrTimer += Time.deltaTime;
            _gpsTimer += Time.deltaTime;
            _notifTimer += Time.deltaTime;

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

            if (_notifTimer >= notificationInterval)
            {
                _notifTimer = 0f;
                PushNotificationUpdate();
            }
        }

        private void PushHeartRateUpdate()
        {
            // Mock heart-rate data keeps the HUD testable before live Android health data is wired up.
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

            // Small heading and coordinate drift makes the GPS widget feel live without a device feed.
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

        private void PushNotificationUpdate()
        {
            var notif = MockNotifications[_notifIndex % MockNotifications.Length];
            _notifIndex++;
            SimulateNotification(notif.app, notif.title);
        }

        public void SimulateNotification(string appName, string title, bool redacted = false)
        {
            // Notification payloads reuse the same HUD entry point as future bridge-delivered data.
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
