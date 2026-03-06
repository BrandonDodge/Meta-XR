using UnityEngine;

namespace HUDLink.Widgets
{
    // --- Advanced Event Payloads ---

    public class ActivityEvent : WidgetEvent
    {
        public int Steps { get; private set; }
        public int Calories { get; private set; }
        public int ActiveMinutes { get; private set; }
        public float DailyGoalPercentage { get; private set; }

        public ActivityEvent(int v, int steps, int cals, int mins, float pct) : base(v)
        {
            Steps = steps; Calories = cals; ActiveMinutes = mins; DailyGoalPercentage = pct;
        }
    }

    public class EnvironmentDataEvent : WidgetEvent
    {
        public float TemperatureC { get; private set; }
        public int UVIndex { get; private set; }
        public int AirQualityIndex { get; private set; }

        public EnvironmentDataEvent(int v, float temp, int uv, int aqi) : base(v)
        {
            TemperatureC = temp; UVIndex = uv; AirQualityIndex = aqi;
        }
    }

    public class MediaPlaybackEvent : WidgetEvent
    {
        public string TrackName { get; private set; }
        public string ArtistName { get; private set; }
        public float PlaybackProgress { get; private set; }
        public bool IsPlaying { get; private set; }

        public MediaPlaybackEvent(int v, string track, string artist, float prog, bool playing) : base(v)
        {
            TrackName = track; ArtistName = artist; PlaybackProgress = prog; IsPlaying = playing;
        }
    }

    public class ProximityAlertEvent : WidgetEvent
    {
        public string TargetId { get; private set; }
        public float DistanceMeters { get; private set; }
        public Vector2 RelativeBearing { get; private set; } // X,Y polar coords

        public ProximityAlertEvent(int v, string target, float dist, Vector2 bearing) : base(v)
        {
            TargetId = target; DistanceMeters = dist; RelativeBearing = bearing;
        }
    }

    public class HRVStressEvent : WidgetEvent
    {
        public float StressLevel { get; private set; } // 0.0 to 1.0
        public float HeartRateVariabilityMs { get; private set; }

        public HRVStressEvent(int v, float stress, float hrv) : base(v)
        {
            StressLevel = stress; HeartRateVariabilityMs = hrv;
        }
    }

    public class SystemStateEvent : WidgetEvent
    {
        public float PhoneBatteryLevel { get; private set; }
        public float HeadsetBatteryLevel { get; private set; }
        public int SignalStrengthPercent { get; private set; }

        public SystemStateEvent(int v, float phoneBat, float headBat, int sig) : base(v)
        {
            PhoneBatteryLevel = phoneBat; HeadsetBatteryLevel = headBat; SignalStrengthPercent = sig;
        }
    }
}
