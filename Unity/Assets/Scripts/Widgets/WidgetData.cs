/*
 Code Artifact: WidgetData.cs
 Description: Defines the simple data objects passed from providers and bridge code into HUD widgets.
 Programmer: HudLink development team (Brandon Dodge, Zach Sevart, Asa Maker, Jonathan Gott, Josh Dwoskin)
 Date Created: 2026-03-09
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
namespace HudLink.Widgets
{
    /// <summary>
    /// Base class for data passed into widgets. Each widget type defines its own subclass.
    /// </summary>
    public abstract class WidgetData
    {
        public long TimestampMs { get; set; }
        public DataSource Source { get; set; }
    }

    public enum DataSource
    {
        Mock,
        Phone,
        Watch
    }

    public class HeartRateWidgetData : WidgetData
    {
        // Heart rate uses an explicit validity flag so the UI can show sensor-loss states cleanly.
        public int Bpm { get; set; }
        public bool IsValid { get; set; }
    }

    public class GpsWidgetData : WidgetData
    {
        // GPS data carries both motion context and a fix-state flag for fallback rendering.
        public float SpeedMph { get; set; }
        public float HeadingDegrees { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool HasFix { get; set; }
    }

    public class NotificationWidgetData : WidgetData
    {
        // Notifications can be redacted while still preserving the source-app context.
        public string AppName { get; set; }
        public string Title { get; set; }
        public bool IsRedacted { get; set; }
    }
}
