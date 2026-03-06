using UnityEngine;

namespace HUDLink.Widgets
{
    /// <summary>
    /// High-order experimental tracking widget (Sprint 6 target).
    /// Tracks friends or points of interest relative to user gaze/heading in a mini-map radar layout.
    /// </summary>
    public class ProximityRadarWidget : BaseWidget
    {
        [Header("Radar Settings")]
        [SerializeField] private float maxRadarRangeMeters = 50f;
        [SerializeField] private GameObject blipPrefab;
        [SerializeField] private Transform radarCenter;
        
        // Active tracking
        private string trackingId;
        private Vector2 bearingCoordinates; // Map user directly

        public override void Initialize()
        {
            base.Initialize();
            WidgetEventBus.Subscribe<ProximityAlertEvent>(OnProximityUpdate);
            Debug.Log($"[{WidgetId}] ProximityRadarWidget Initialized.");
        }

        public override void DestroyWidget()
        {
            WidgetEventBus.Unsubscribe<ProximityAlertEvent>(OnProximityUpdate);
            base.DestroyWidget();
        }

        private void OnProximityUpdate(ProximityAlertEvent proxEvent)
        {
            // In a real application, we would manage a pool of targets.
            // For this impressive demo, we track the loudest proximity alert.
            trackingId = proxEvent.TargetId;
            bearingCoordinates = proxEvent.RelativeBearing;
            
            float dist = proxEvent.DistanceMeters;
            float scaleFactor = Mathf.Clamp01(1.0f - (dist / maxRadarRangeMeters));
            
            Debug.Log($"[{WidgetId}] Radar Target '{trackingId}' locked. Dist: {dist}m, Scale Output: {scaleFactor}");
            
            UpdateRadarVisuals(scaleFactor);
        }

        protected override void RenderWidget(float deltaTime)
        {
            // Radar sweeping visual effect or pulsing ring effect goes here
        }

        private void UpdateRadarVisuals(float scale)
        {
            if (blipPrefab != null && radarCenter != null)
            {
                // Position the blip relative to bearing polar coordinates
                blipPrefab.transform.localPosition = new Vector3(bearingCoordinates.x, bearingCoordinates.y, 0);
                
                // Scale it up if they are closer to you
                blipPrefab.transform.localScale = Vector3.one * (scale * 2f);
            }
        }
    }
}
