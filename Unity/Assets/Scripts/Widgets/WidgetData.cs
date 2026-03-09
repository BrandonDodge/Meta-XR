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
        public int Bpm { get; set; }
        public bool IsValid { get; set; }
    }

    public class GpsWidgetData : WidgetData
    {
        public float SpeedMph { get; set; }
        public float HeadingDegrees { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool HasFix { get; set; }
    }

    public class NotificationWidgetData : WidgetData
    {
        public string AppName { get; set; }
        public string Title { get; set; }
        public bool IsRedacted { get; set; }
    }
}
