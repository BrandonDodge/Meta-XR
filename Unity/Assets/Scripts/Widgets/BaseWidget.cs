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

        public virtual void Initialize(RectTransform slot)
        {
            SlotTransform = slot;
            transform.SetParent(slot, false);

            var rect = GetComponent<RectTransform>();
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
