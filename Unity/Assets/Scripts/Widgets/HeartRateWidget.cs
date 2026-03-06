using UnityEngine;

namespace HUDLink.Widgets
{
    /// <summary>
    /// Event payload specifically for Heart Rate data drops from the Android transport.
    /// </summary>
    public class HeartRateEvent : WidgetEvent
    {
        public int BPM { get; private set; }
        public float Confidence { get; private set; }

        public HeartRateEvent(int version, int bpm, float confidence) : base(version)
        {
            BPM = bpm;
            Confidence = confidence;
        }
    }

    /// <summary>
    /// Implements visual representation of user heart rate on the AR display.
    /// Subscribes to HeartRateEvent via the WidgetEventBus.
    /// Derived from Sprint 4 Requirement R18 (Heart rate widget rendering).
    /// </summary>
    public class HeartRateWidget : BaseWidget
    {
        [Header("Heart Rate UI")]
        // [SerializeField] private TMPro.TextMeshProUGUI bpmText;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color elevatedColor = Color.red;
        
        private int currentBpm = 0;
        private float currentConfidence = 0;

        public override void Initialize()
        {
            base.Initialize();
            // Subscribe to the global data pipeline for heart rate events
            WidgetEventBus.Subscribe<HeartRateEvent>(OnHeartRateUpdate);
            Debug.Log($"[{WidgetId}] HeartRateWidget Initialized and Subscribed.");
        }

        public override void DestroyWidget()
        {
            // Always unsubscribe to prevent memory leaks in the event bus
            WidgetEventBus.Unsubscribe<HeartRateEvent>(OnHeartRateUpdate);
            base.DestroyWidget();
        }

        private void OnHeartRateUpdate(HeartRateEvent hrEvent)
        {
            currentBpm = hrEvent.BPM;
            currentConfidence = hrEvent.Confidence;
            // Update UI visuals immediately upon data receipt
            UpdateVisuals();
        }

        protected override void RenderWidget(float deltaTime)
        {
            // Passive animations or visual pulses based on BPM could happen here
            // It operates within the maxDrawCalls budget defined in BaseWidget.
        }

        private void UpdateVisuals()
        {
            // Example AR text update:
            // if (bpmText != null) bpmText.text = $"{currentBpm} BPM";
            // if (bpmText != null) bpmText.color = currentBpm > 120 ? elevatedColor : normalColor;
            
            Debug.Log($"[{WidgetId}] Heart Rate Visual Updated: {currentBpm} BPM (Conf: {currentConfidence})");
        }
    }
}
