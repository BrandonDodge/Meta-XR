using UnityEngine;

namespace HUDLink.Widgets
{
    /// <summary>
    /// Minimalist dock widget rendering battery life of the phone and headset,
    /// and the stability of the active BLE connection.
    /// </summary>
    public class SystemStatusWidget : BaseWidget
    {
        private float headsetCharge = 1.0f;
        private float phoneCharge = 1.0f;
        private int bleSignal = 100;

        public override void Initialize()
        {
            base.Initialize();
            WidgetEventBus.Subscribe<SystemStateEvent>(OnSystemUpdate);
            Debug.Log($"[{WidgetId}] SystemStatusWidget Initialized.");
        }

        public override void DestroyWidget()
        {
            WidgetEventBus.Unsubscribe<SystemStateEvent>(OnSystemUpdate);
            base.DestroyWidget();
        }

        private void OnSystemUpdate(SystemStateEvent stateEvent)
        {
            headsetCharge = stateEvent.HeadsetBatteryLevel;
            phoneCharge = stateEvent.PhoneBatteryLevel;
            bleSignal = stateEvent.SignalStrengthPercent;

            UpdateBatteryIcons();
        }

        protected override void RenderWidget(float deltaTime)
        {
            // Low battery flash animation can loop here using Mathf.Sin
        }

        private void UpdateBatteryIcons()
        {
            // UI code bridging goes here
            // Changes icon to orange if < 20%, red if < 10%
            Debug.Log($"[{WidgetId}] Sys Update -> AR: {headsetCharge*100}% | Mobile: {phoneCharge*100}% | BLE: {bleSignal}%");
        }
    }
}
