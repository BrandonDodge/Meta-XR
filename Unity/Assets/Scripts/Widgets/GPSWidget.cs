using UnityEngine;

namespace HUDLink.Widgets
{
    /// <summary>
    /// Event payload specifically for GPS/Heading contextual drops.
    /// </summary>
    public class HeadingEvent : WidgetEvent
    {
        public float Azimuth { get; private set; }
        public float Pitch { get; private set; }

        public HeadingEvent(int version, float azimuth, float pitch) : base(version)
        {
            Azimuth = azimuth;
            Pitch = pitch;
        }
    }

    /// <summary>
    /// Implements directional compass or contextual location metrics on the AR display.
    /// Subscribes to HeadingEvent via the WidgetEventBus.
    /// </summary>
    public class GPSWidget : BaseWidget
    {
        [Header("GPS UI Elements")]
        [Tooltip("Transform to rotate to act as an AR compass arrow.")]
        [SerializeField] private Transform compassNeedle;

        private float currentAzimuth = 0f;

        public override void Initialize()
        {
            base.Initialize();
            WidgetEventBus.Subscribe<HeadingEvent>(OnHeadingUpdate);
            Debug.Log($"[{WidgetId}] GPSWidget Initialized and Subscribed.");
        }

        public override void DestroyWidget()
        {
            WidgetEventBus.Unsubscribe<HeadingEvent>(OnHeadingUpdate);
            base.DestroyWidget();
        }

        private void OnHeadingUpdate(HeadingEvent headingEvent)
        {
            currentAzimuth = headingEvent.Azimuth;
            UpdateCompassVisual();
        }

        protected override void RenderWidget(float deltaTime)
        {
            // Can smoothly interpolate needle rotation over time here,
            // rather than instantly snapping on data receipt.
        }

        private void UpdateCompassVisual()
        {
            if (compassNeedle != null)
            {
                // Simple 2D rotation for an AR overlay compass relative to user frame
                compassNeedle.localRotation = Quaternion.Euler(0, 0, -currentAzimuth);
            }
            Debug.Log($"[{WidgetId}] GPS Visual Updated: {currentAzimuth}°");
        }
    }
}
