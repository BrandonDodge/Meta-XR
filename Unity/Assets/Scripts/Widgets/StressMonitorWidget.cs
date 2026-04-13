using UnityEngine;

namespace HudLink.Widgets
{
    /// <summary>
    /// Tracks user stress parameters using derived Heart Rate Variability (HRV).
    /// Warns the user or tints the screen edge if stress climbs into a 'fight or flight' zone.
    /// </summary>
    public class StressMonitorWidget : BaseWidget
    {
        [Header("Biofeedback Settings")]
        [Range(0f, 1f)] public float StressThreshold = 0.7f;
        [SerializeField] private CanvasGroup warningVignette;

        private float currentStress = 0f;

        public override void Initialize(RectTransform slot)
        {
            base.Initialize(slot);
            WidgetEventBus.Subscribe<HRVStressEvent>(OnHRVUpdate);
            if (warningVignette != null) warningVignette.alpha = 0f;
            Debug.Log($"[{WidgetId}] StressMonitorWidget Initialized.");
        }

        public override void UpdateData(WidgetData data)
        {
            // This widget currently receives updates via WidgetEventBus.
        }

        public override void Dispose()
        {
            WidgetEventBus.Unsubscribe<HRVStressEvent>(OnHRVUpdate);
            base.Dispose();
        }

        private void OnHRVUpdate(HRVStressEvent stressEvent)
        {
            currentStress = stressEvent.StressLevel;
        }

        private void Update()
        {
            if (!Initialized || warningVignette == null) return;

            // Interpolate a subtle red/orange vignette on the AR display 
            // if the user's autonomic system indicates elevated stress.
            float targetAlpha = currentStress > StressThreshold ? (currentStress - StressThreshold) * 2f : 0f;
            warningVignette.alpha = Mathf.Lerp(warningVignette.alpha, targetAlpha, Time.deltaTime);
        }
    }
}
