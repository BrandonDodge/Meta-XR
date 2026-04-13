using UnityEngine;

namespace HudLink.Widgets
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

        public override void Initialize(RectTransform slot)
        {
            base.Initialize(slot);
            WidgetEventBus.Subscribe<SystemStateEvent>(OnSystemUpdate);
            Debug.Log($"[{WidgetId}] SystemStatusWidget Initialized.");
        }

        public override void UpdateData(WidgetData data)
        {
            // This widget currently receives updates via WidgetEventBus.
        }

        public override void Dispose()
        {
            WidgetEventBus.Unsubscribe<SystemStateEvent>(OnSystemUpdate);
            base.Dispose();
        }

        private void OnSystemUpdate(SystemStateEvent stateEvent)
        {
            headsetCharge = stateEvent.HeadsetBatteryLevel;
            phoneCharge = stateEvent.PhoneBatteryLevel;
            bleSignal = stateEvent.SignalStrengthPercent;

            UpdateBatteryIcons();
        }

        private void UpdateBatteryIcons()
        {
            // UI code bridging goes here
            // Changes icon to orange if < 20%, red if < 10%
            Debug.Log($"[{WidgetId}] Sys Update -> AR: {headsetCharge*100}% | Mobile: {phoneCharge*100}% | BLE: {bleSignal}%");
        }
    }
}
