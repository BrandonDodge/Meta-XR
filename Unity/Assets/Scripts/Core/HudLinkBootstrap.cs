// See HudLink.Core.ProjectContributionLedger for sprint attribution and dated maintenance notes.
using UnityEngine;
using HudLink.HUD;
using HudLink.Widgets;

namespace HudLink.Core
{
    /// <summary>
    /// Scene entry point. Initializes the HUD, creates widgets, and assigns them to grid slots.
    /// Attach this to an empty GameObject in the scene root.
    /// </summary>
    public class HudLinkBootstrap : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private HudController hudController;
        [SerializeField] private HudGridLayout gridLayout;

        [Header("Widget Prefabs")]
        [SerializeField] private GameObject heartRateWidgetPrefab;
        [SerializeField] private GameObject gpsWidgetPrefab;
        [SerializeField] private GameObject notificationWidgetPrefab;

        [Header("Default Widget Placement")]
        [SerializeField]
        private HudGridLayout.SlotPosition heartRateSlot = HudGridLayout.SlotPosition.TopLeft;
        [SerializeField]
        private HudGridLayout.SlotPosition gpsSlot = HudGridLayout.SlotPosition.BottomCenter;
        [SerializeField]
        private HudGridLayout.SlotPosition notificationSlot = HudGridLayout.SlotPosition.TopRight;

        private void Start()
        {
            // Apply camera passthrough first so the HUD initializes against the final scene view.
            EnablePassthrough();
            SetupDefaultWidgets();
        }

        private void EnablePassthrough()
        {
            // Set camera to render nothing so passthrough shows the real world
            var cam = Camera.main;
            if (cam != null)
            {
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = Color.clear;
            }
        }

        private void SetupDefaultWidgets()
        {
            // Register only the default widgets that are wired in the scene.
            if (heartRateWidgetPrefab != null)
            {
                var hrWidget = CreateWidget<HeartRateWidget>(heartRateWidgetPrefab, "heart_rate");
                hudController.RegisterWidget(hrWidget, heartRateSlot);
            }

            if (gpsWidgetPrefab != null)
            {
                var gpsWidget = CreateWidget<GPSWidget>(gpsWidgetPrefab, "gps");
                hudController.RegisterWidget(gpsWidget, gpsSlot);
            }

            if (notificationWidgetPrefab != null)
            {
                var notifWidget = CreateWidget<NotificationWidget>(notificationWidgetPrefab, "notifications");
                hudController.RegisterWidget(notifWidget, notificationSlot);
            }
        }

        private T CreateWidget<T>(GameObject prefab, string widgetId) where T : BaseWidget
        {
            var go = Instantiate(prefab);
            go.name = $"Widget_{widgetId}";
            // Prefabs are expected to already carry the requested widget component.
            var widget = go.GetComponent<T>();
            return widget;
        }
    }
}
