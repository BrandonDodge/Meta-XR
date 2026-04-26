/*
 Code Artifact: BaseWidget.cs
 Description: Implements the shared widget lifecycle, slot parenting, layout bounds, visibility, and text-label helpers.
 Programmer: HudLink development team (Brandon Dodge, Zach Sevart, Asa Maker, Jonathan Gott, Josh Dwoskin)
 Date Created: 2026-03-06
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
using UnityEngine;
using TMPro;

namespace HudLink.Widgets
{
    /// <summary>
    /// Abstract base class for all widgets. Handles common lifecycle and slot management.
    /// Matches BaseWidget from the architecture UML.
    /// </summary>
    public abstract class BaseWidget : MonoBehaviour, IWidget
    {
        [SerializeField] private string widgetId;
        [SerializeField] private string displayName;

        protected RectTransform SlotTransform { get; private set; }
        protected bool Initialized { get; private set; }

        public string WidgetId => widgetId;
        public string DisplayName => displayName;
        public bool IsVisible => gameObject.activeSelf;

        public virtual Vector2 GetLayoutBounds()
        {
            var rect = GetComponent<RectTransform>();
            // Layout managers use the widget rect as the default footprint.
            return rect != null ? rect.rect.size : Vector2.zero;
        }

        public virtual void Initialize(RectTransform slot)
        {
            SlotTransform = slot;
            transform.SetParent(slot, false);

            var rect = GetComponent<RectTransform>();
            if (rect == null)
            {
                rect = gameObject.AddComponent<RectTransform>();
            }
            // Stretch the widget to fully occupy its assigned grid slot.
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Initialized = true;
        }

        public abstract void UpdateData(WidgetData data);

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void Dispose()
        {
            if (gameObject != null)
                // Widgets own their instantiated GameObject lifetime.
                Destroy(gameObject);
        }

        protected TextMeshProUGUI CreateLabel(string objectName, int fontSize, TextAlignmentOptions alignment)
        {
            var go = new GameObject(objectName);
            go.transform.SetParent(transform, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.fontSize = fontSize;
            tmp.alignment = alignment;
            tmp.color = Color.white;

            return tmp;
        }
    }
}
