using System.Collections.Generic;
using UnityEngine;
using HudLink.Widgets;

namespace HudLink.HUD
{
    /// <summary>
    /// Central controller for the HUD. Manages widget registration, slot assignment,
    /// visibility toggling, and data routing. This is the main entry point for the HUD system.
    /// </summary>
    public class HudController : MonoBehaviour
    {
        [SerializeField] private GameObject hudRoot;
        [SerializeField] private HudGridLayout gridLayout;

        private readonly Dictionary<string, BaseWidget> _activeWidgets = new();
        private bool _hudVisible = true;

        private void Awake()
        {
            if (gridLayout != null)
                gridLayout.Initialize();
        }

        public void ToggleHud()
        {
            _hudVisible = !_hudVisible;
            hudRoot.SetActive(_hudVisible);
        }

        public void SetHudVisible(bool visible)
        {
            _hudVisible = visible;
            hudRoot.SetActive(_hudVisible);
        }

        public bool IsHudVisible => _hudVisible;

        public void RegisterWidget(BaseWidget widget, HudGridLayout.SlotPosition position)
        {
            if (position == HudGridLayout.SlotPosition.SafeZone)
            {
                Debug.LogWarning("[HudLink] Cannot place widget in SafeZone slot.");
                return;
            }

            var slot = gridLayout.GetSlot(position);
            if (slot == null)
            {
                Debug.LogError($"[HudLink] Slot {position} not found.");
                return;
            }

            // Remove existing widget in this slot
            if (_activeWidgets.ContainsKey(widget.WidgetId))
            {
                _activeWidgets[widget.WidgetId].Dispose();
                _activeWidgets.Remove(widget.WidgetId);
            }

            widget.Initialize(slot);
            widget.Show();
            _activeWidgets[widget.WidgetId] = widget;
        }

        public void RemoveWidget(string widgetId)
        {
            if (_activeWidgets.TryGetValue(widgetId, out var widget))
            {
                widget.Dispose();
                _activeWidgets.Remove(widgetId);
            }
        }

        public void HideWidget(string widgetId)
        {
            if (_activeWidgets.TryGetValue(widgetId, out var widget))
                widget.Hide();
        }

        public void ShowWidget(string widgetId)
        {
            if (_activeWidgets.TryGetValue(widgetId, out var widget))
                widget.Show();
        }

        public void UpdateWidget(string widgetId, WidgetData data)
        {
            if (_activeWidgets.TryGetValue(widgetId, out var widget))
                widget.UpdateData(data);
        }

        public BaseWidget GetWidget(string widgetId)
        {
            _activeWidgets.TryGetValue(widgetId, out var widget);
            return widget;
        }
    }
}
