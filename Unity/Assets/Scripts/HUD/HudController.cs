/*
 Code Artifact: HudController.cs
 Description: Owns HUD visibility, widget registration, slot assignment, widget removal, and widget data routing.
 Programmer: HudLink development team (Brandon Dodge, Zach Sevart, Asa Maker, Jonathan Gott, Josh Dwoskin)
 Date Created: 2026-03-09
 Revision History:
 - 2026-04-13 - Brandon.Dodge - Update contribution ledger and Unity scripts
 - 2026-04-26 - HudLink development team - Added release prologue comments and tightened sprint-review documentation.
 Preconditions: Unity calls this artifact on the main thread; required scene references and serialized fields are assigned before runtime use.
 Acceptable Inputs: Valid Unity objects, event payloads, widget data, enum values, and inspector settings documented by the fields below.
 Unacceptable Inputs: Missing required scene references, null widget components, invalid slot choices, or sensor values outside the documented model ranges.
 Postconditions: HUD state, widget UI, event subscriptions, or diagnostic output is updated according to the public method that was called.
 Return Values: Unity lifecycle methods return void; helper methods return the type named in the method signature, or null only where the method documents a missing reference.
 Error and Exception Conditions: Unity may log errors or warnings for missing references, wrong prefab setup, invalid data, or unsupported slot assignments.
 Side Effects: May create, parent, destroy, activate, deactivate, or recolor Unity GameObjects and may publish or subscribe to HUD events.
 Invariants: Widget IDs stay stable for routing, the center safe zone remains clear, and UI updates run on Unity's main thread.
 Known Faults: Live Android bridge input is not complete yet, so several demo paths still use mock data or simulated events.
 Major Blocks: The inline comments below mark lifecycle hooks, data routing, validation, persistence, and UI update blocks.
 Line Comments: Important statements and branches carry short notes where a teammate would otherwise need extra context.
*/
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
            {
                gridLayout.Initialize();
            }
            else
            {
                Debug.LogError("[HudLink] HudController needs a HudGridLayout reference.");
            }
        }

        public void ToggleHud()
        {
            _hudVisible = !_hudVisible;
            SetHudRootActive();
        }

        public void SetHudVisible(bool visible)
        {
            _hudVisible = visible;
            SetHudRootActive();
        }

        public bool IsHudVisible => _hudVisible;

        public void RegisterWidget(BaseWidget widget, HudGridLayout.SlotPosition position)
        {
            if (widget == null)
            {
                Debug.LogError("[HudLink] Cannot register a missing widget component.");
                return;
            }

            if (gridLayout == null)
            {
                Debug.LogError("[HudLink] Cannot register widgets before the grid layout is assigned.");
                return;
            }

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

            // Re-registering the same widget id replaces the existing live instance.
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
            // Data routing stays string-based so the bridge layer does not need direct widget references.
            if (_activeWidgets.TryGetValue(widgetId, out var widget))
                widget.UpdateData(data);
        }

        public BaseWidget GetWidget(string widgetId)
        {
            _activeWidgets.TryGetValue(widgetId, out var widget);
            return widget;
        }

        private void SetHudRootActive()
        {
            if (hudRoot != null)
            {
                hudRoot.SetActive(_hudVisible);
            }
            else
            {
                Debug.LogWarning("[HudLink] Hud root is not assigned; visibility state was cached only.");
            }
        }
    }
}
