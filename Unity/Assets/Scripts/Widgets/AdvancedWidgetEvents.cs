/*
 Code Artifact: AdvancedWidgetEvents.cs
 Description: Defines the typed event payloads used by the advanced HUD widgets.
 Programmer: HudLink development team (Brandon Dodge, Zach Sevart, Asa Maker, Jonathan Gott, Josh Dwoskin)
 Date Created: 2026-03-06
 Revision History:
 - 2026-04-12 - Zach Sevart - Merge test branch: add event bus, new widgets, data layer enhancements
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

namespace HudLink.Widgets
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
            Steps = steps;
            Calories = cals;
            ActiveMinutes = mins;
            DailyGoalPercentage = pct;
        }
    }

    public class EnvironmentDataEvent : WidgetEvent
    {
        public float TemperatureC { get; private set; }
        public int UVIndex { get; private set; }
        public int AirQualityIndex { get; private set; }

        public EnvironmentDataEvent(int v, float temp, int uv, int aqi) : base(v)
        {
            TemperatureC = temp;
            UVIndex = uv;
            AirQualityIndex = aqi;
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
            TrackName = track;
            ArtistName = artist;
            PlaybackProgress = prog;
            IsPlaying = playing;
        }
    }

    public class ProximityAlertEvent : WidgetEvent
    {
        public string TargetId { get; private set; }
        public float DistanceMeters { get; private set; }
        public Vector2 RelativeBearing { get; private set; } // X,Y polar coords

        public ProximityAlertEvent(int v, string target, float dist, Vector2 bearing) : base(v)
        {
            TargetId = target;
            DistanceMeters = dist;
            RelativeBearing = bearing;
        }
    }

    public class HRVStressEvent : WidgetEvent
    {
        public float StressLevel { get; private set; } // 0.0 to 1.0
        public float HeartRateVariabilityMs { get; private set; }

        public HRVStressEvent(int v, float stress, float hrv) : base(v)
        {
            StressLevel = stress;
            HeartRateVariabilityMs = hrv;
        }
    }

    public class SystemStateEvent : WidgetEvent
    {
        public float PhoneBatteryLevel { get; private set; }
        public float HeadsetBatteryLevel { get; private set; }
        public int SignalStrengthPercent { get; private set; }

        public SystemStateEvent(int v, float phoneBat, float headBat, int sig) : base(v)
        {
            PhoneBatteryLevel = phoneBat;
            HeadsetBatteryLevel = headBat;
            SignalStrengthPercent = sig;
        }
    }
}
