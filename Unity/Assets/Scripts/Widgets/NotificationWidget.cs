using UnityEngine;

namespace HUDLink.Widgets
{
    public class NotificationDataEvent : WidgetEvent
    {
        public string Message { get; private set; }
        public int PriorityLevel { get; private set; }

        public NotificationDataEvent(int version, string msg, int priority) : base(version)
        {
            Message = msg;
            PriorityLevel = priority;
        }
    }

    /// <summary>
    /// Handles asynchronous general alerts (messages, phone status changes).
    /// Drops into view only when triggered.
    /// </summary>
    public class NotificationWidget : BaseWidget
    {
        private float displayTimer = 0f;
        private const float MAX_DISPLAY_TIME = 5f;

        public override void Initialize()
        {
            base.Initialize();
            WidgetEventBus.Subscribe<NotificationDataEvent>(OnNotificationReceived);
            // Notifications are hidden by default until an event arrives
            Suspend(); 
        }

        public override void DestroyWidget()
        {
            WidgetEventBus.Unsubscribe<NotificationDataEvent>(OnNotificationReceived);
            base.DestroyWidget();
        }

        private void OnNotificationReceived(NotificationDataEvent notifEvent)
        {
            // Activate the widget UI panel
            Resume();
            displayTimer = MAX_DISPLAY_TIME;
            
            Debug.Log($"[{WidgetId}] Notification Alert triggered: {notifEvent.Message}");
        }

        protected override void RenderWidget(float deltaTime)
        {
            if (displayTimer > 0)
            {
                displayTimer -= deltaTime;
                if (displayTimer <= 0)
                {
                    // Auto-hide when duration expires
                    Suspend();
                }
            }
        }
    }
}
