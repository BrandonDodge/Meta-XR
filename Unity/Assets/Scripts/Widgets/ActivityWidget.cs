using UnityEngine;
using UnityEngine.UI;

namespace HudLink.Widgets
{
    /// <summary>
    /// Advanced visualization for daily fitness metrics (Steps, Calories, Minutes).
    /// Features smooth interpolation of circular fill amounts (like Activity Rings).
    /// </summary>
    public class ActivityWidget : BaseWidget
    {
        [Header("Activity Rings UI")]
        [Tooltip("The circular image representing daily goal progress.")]
        [SerializeField] private Image progressRingFill;
        [SerializeField] private float animationSpeed = 2f;

        private float targetFillAmount = 0f;
        private float currentFillAmount = 0f;

        public override void Initialize(RectTransform slot)
        {
            base.Initialize(slot);
            WidgetEventBus.Subscribe<ActivityEvent>(OnActivityUpdate);
            if (progressRingFill != null) progressRingFill.fillAmount = 0;
            Debug.Log($"[{WidgetId}] ActivityWidget Initialized.");
        }

        public override void UpdateData(WidgetData data)
        {
            // This widget currently receives updates via WidgetEventBus.
        }

        public override void Dispose()
        {
            WidgetEventBus.Unsubscribe<ActivityEvent>(OnActivityUpdate);
            base.Dispose();
        }

        private void OnActivityUpdate(ActivityEvent activityData)
        {
            // Clamp percentage and cache the target for smooth animation over time
            targetFillAmount = Mathf.Clamp01(activityData.DailyGoalPercentage);
            Debug.Log($"[{WidgetId}] Activity Goal updated to {activityData.DailyGoalPercentage * 100}%");
        }

        private void Update()
        {
            if (!Initialized) return;

            // Smoothly animate the ring fill based on the target value
            if (Mathf.Abs(currentFillAmount - targetFillAmount) > 0.001f)
            {
                currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, Time.deltaTime * animationSpeed);
                if (progressRingFill != null)
                {
                    progressRingFill.fillAmount = currentFillAmount;
                    
                    // Dynamic color switching: Green if complete, cyan if in progress
                    progressRingFill.color = currentFillAmount >= 0.99f ? Color.green : Color.cyan;
                }
            }
        }
    }
}
