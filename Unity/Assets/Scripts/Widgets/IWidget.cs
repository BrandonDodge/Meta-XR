// See HudLink.Core.ProjectContributionLedger for sprint attribution and dated maintenance notes.
using UnityEngine;

namespace HudLink.Widgets
{
    /// <summary>
    /// Interface for all HUD widgets. Matches the IWidget interface from the architecture UML.
    /// </summary>
    public interface IWidget
    {
        string WidgetId { get; }
        string DisplayName { get; }
        bool IsVisible { get; }

        // Layout managers use this to space widgets without hard-coding prefab sizes.
        Vector2 GetLayoutBounds();
        void Initialize(RectTransform slot);
        void UpdateData(WidgetData data);
        void Show();
        void Hide();
        void Dispose();
    }
}
