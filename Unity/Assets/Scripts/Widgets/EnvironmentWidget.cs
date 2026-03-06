using UnityEngine;

namespace HUDLink.Widgets
{
    /// <summary>
    /// Consolidates external contextual data (Weather, AQI, UV) into a simple AR badge.
    /// Highly useful for running/cycling scenarios in AR glasses.
    /// </summary>
    public class EnvironmentWidget : BaseWidget
    {
        [Header("Environmental Thresholds")]
        [SerializeField] private Color safeAQIColor = Color.green;
        [SerializeField] private Color warningAQIColor = new Color(1f, 0.5f, 0f); // Orange
        [SerializeField] private Color hazardousAQIColor = Color.red;

        private float tempC;
        private int aqi;
        private int uv;

        public override void Initialize()
        {
            base.Initialize();
            WidgetEventBus.Subscribe<EnvironmentDataEvent>(OnEnvironmentUpdate);
            Debug.Log($"[{WidgetId}] EnvironmentWidget Initialized.");
        }

        public override void DestroyWidget()
        {
            WidgetEventBus.Unsubscribe<EnvironmentDataEvent>(OnEnvironmentUpdate);
            base.DestroyWidget();
        }

        private void OnEnvironmentUpdate(EnvironmentDataEvent envData)
        {
            tempC = envData.TemperatureC;
            aqi = envData.AirQualityIndex;
            uv = envData.UVIndex;
            UpdateEnvironmentalVisuals();
        }

        protected override void RenderWidget(float deltaTime)
        {
            // Passive ambient particle effects (like dust/rain) could be triggered here
            // based on the environmental conditions.
        }

        private void UpdateEnvironmentalVisuals()
        {
            Color currentAQIColor = safeAQIColor;
            if (aqi > 100) currentAQIColor = warningAQIColor;
            if (aqi > 150) currentAQIColor = hazardousAQIColor;

            // In a real implementation we update a Canvas / TextMeshPro element here
            Debug.Log($"[{WidgetId}] Weather: {tempC}°C | UV: {uv} | AQI: {aqi} (Color: {currentAQIColor})");
        }
    }
}
