using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using HudLink.HUD;
using HudLink.Widgets;

namespace HudLink.Data
{
    /// <summary>
    /// Pulls real health data from a Firebase Realtime Database endpoint.
    /// Replaces MockDataProvider for live data integration.
    ///
    /// Setup:
    /// 1. Create a Firebase project at https://console.firebase.google.com
    /// 2. Create a Realtime Database (test mode for development)
    /// 3. Set the firebaseUrl to your database URL
    /// 4. Push data from the companion web app on your iPhone
    /// </summary>
    public class CloudDataProvider : MonoBehaviour
    {
        [SerializeField] private HudController hudController;

        [Header("Firebase Configuration")]
        [SerializeField] private string firebaseUrl = "";
        [SerializeField] private float pollInterval = 2.0f;

        [Header("Fallback")]
        [SerializeField] private bool useMockOnFailure = true;

        private bool _isConnected;
        private float _lastSuccessTime;
        private int _consecutiveFailures;

        private void Start()
        {
            if (string.IsNullOrEmpty(firebaseUrl))
            {
                Debug.LogWarning("[CloudDataProvider] Firebase URL not set. Configure in Inspector.");
                return;
            }

            StartCoroutine(PollFirebase());
        }

        private IEnumerator PollFirebase()
        {
            while (true)
            {
                yield return FetchHeartRate();
                yield return FetchGps();
                yield return FetchNotification();
                yield return new WaitForSeconds(pollInterval);
            }
        }

        private IEnumerator FetchHeartRate()
        {
            string url = $"{firebaseUrl}/health/heartRate.json";

            using var request = UnityWebRequest.Get(url);
            request.timeout = 5;
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success && request.downloadHandler.text != "null")
            {
                var json = request.downloadHandler.text;
                var hrData = JsonUtility.FromJson<FirebaseHeartRate>(json);

                if (hrData != null && hrData.bpm > 0)
                {
                    OnConnectionSuccess();
                    hudController.UpdateWidget("heart_rate", new HeartRateWidgetData
                    {
                        Bpm = hrData.bpm,
                        IsValid = true,
                        TimestampMs = hrData.timestamp,
                        Source = DataSource.Phone
                    });
                }
            }
            else
            {
                OnConnectionFailure("heartRate");
            }
        }

        private IEnumerator FetchGps()
        {
            string url = $"{firebaseUrl}/health/gps.json";

            using var request = UnityWebRequest.Get(url);
            request.timeout = 5;
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success && request.downloadHandler.text != "null")
            {
                var json = request.downloadHandler.text;
                var gpsData = JsonUtility.FromJson<FirebaseGps>(json);

                if (gpsData != null)
                {
                    hudController.UpdateWidget("gps", new GpsWidgetData
                    {
                        SpeedMph = gpsData.speedMph,
                        HeadingDegrees = gpsData.heading,
                        Latitude = gpsData.latitude,
                        Longitude = gpsData.longitude,
                        HasFix = gpsData.hasFix,
                        TimestampMs = gpsData.timestamp,
                        Source = DataSource.Phone
                    });
                }
            }
        }

        private IEnumerator FetchNotification()
        {
            string url = $"{firebaseUrl}/health/notification.json";

            using var request = UnityWebRequest.Get(url);
            request.timeout = 5;
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success && request.downloadHandler.text != "null")
            {
                var json = request.downloadHandler.text;
                var notifData = JsonUtility.FromJson<FirebaseNotification>(json);

                if (notifData != null && !string.IsNullOrEmpty(notifData.title))
                {
                    hudController.UpdateWidget("notifications", new NotificationWidgetData
                    {
                        AppName = notifData.appName,
                        Title = notifData.title,
                        IsRedacted = notifData.redacted,
                        TimestampMs = notifData.timestamp,
                        Source = DataSource.Phone
                    });
                }
            }
        }

        private void OnConnectionSuccess()
        {
            _consecutiveFailures = 0;
            if (!_isConnected)
            {
                _isConnected = true;
                Debug.Log("[CloudDataProvider] Connected to Firebase.");
            }
            _lastSuccessTime = Time.time;
        }

        private void OnConnectionFailure(string endpoint)
        {
            _consecutiveFailures++;
            if (_consecutiveFailures >= 3 && _isConnected)
            {
                _isConnected = false;
                Debug.LogWarning($"[CloudDataProvider] Lost connection to Firebase ({endpoint}).");
            }
        }

        public bool IsConnected => _isConnected;

        // Firebase JSON models
        [System.Serializable]
        private class FirebaseHeartRate
        {
            public int bpm;
            public long timestamp;
        }

        [System.Serializable]
        private class FirebaseGps
        {
            public float speedMph;
            public float heading;
            public double latitude;
            public double longitude;
            public bool hasFix;
            public long timestamp;
        }

        [System.Serializable]
        private class FirebaseNotification
        {
            public string appName;
            public string title;
            public bool redacted;
            public long timestamp;
        }
    }
}
