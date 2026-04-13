// See HudLink.Core.ProjectContributionLedger for sprint attribution and dated maintenance notes.
using System.Collections.Generic;
using UnityEngine;
using HudLink.Widgets;

namespace HudLink.Dashboard
{
    /// <summary>
    /// Manages the layout of HUD widgets inside the XR environment.
    /// Enhances world-space alignment, safe-zone boundaries, and scaling.
    /// Derived from Sprint 3 Requirement FR-3.2.
    /// </summary>
    public class DashboardLayoutManager : MonoBehaviour
    {
        [Header("Layout Settings")]
        [Tooltip("The central transform representing the safe zone boundary.")]
        public Transform CenterSafeZoneAnchor;
        
        [Tooltip("Grid spacing between widgets.")]
        public float Padding = 0.05f;

        private List<IWidget> activeWidgets = new List<IWidget>();
        private Dictionary<IWidget, Transform> widgetTransforms = new Dictionary<IWidget, Transform>();
        private Dictionary<IWidget, Vector3> targetPositions = new Dictionary<IWidget, Vector3>();

        private void Update()
        {
            // Smoothly interpolate widget positions toward their computed slot targets
            foreach (var widget in activeWidgets)
            {
                if (widgetTransforms.TryGetValue(widget, out Transform t) && targetPositions.TryGetValue(widget, out Vector3 target))
                {
                    // FR-4.4 Dashboard Animation: Smooth layout repositioning
                    t.position = Vector3.Lerp(t.position, target, Time.deltaTime * 5f);
                }
            }
        }

        /// <summary>
        /// Registers a new widget to the dashboard layout.
        /// </summary>
        public void RegisterWidget(IWidget widget, Transform widgetTransform)
        {
            if (!activeWidgets.Contains(widget))
            {
                activeWidgets.Add(widget);
                widgetTransforms[widget] = widgetTransform;
                RecomputeLayout();
            }
        }

        /// <summary>
        /// Unregisters a widget from the layout.
        /// </summary>
        public void UnregisterWidget(IWidget widget)
        {
            if (activeWidgets.Contains(widget))
            {
                activeWidgets.Remove(widget);
                widgetTransforms.Remove(widget);
                targetPositions.Remove(widget);
                RecomputeLayout();
            }
        }

        /// <summary>
        /// Recomputes the positioning of all active widgets, ensuring they
        /// respect the safe-zone and do not overlap.
        /// </summary>
        private void RecomputeLayout()
        {
            // Simple slot-based placement logic for Sprint 3.
            // Positions widgets horizontally outside the center safe zone.
            
            if (activeWidgets.Count == 0 || CenterSafeZoneAnchor == null) return;

            float currentX = CenterSafeZoneAnchor.position.x + 0.5f; // Offset from center
            float currentY = CenterSafeZoneAnchor.position.y;
            float currentZ = CenterSafeZoneAnchor.position.z;

            foreach (var widget in activeWidgets)
            {
                Vector2 bounds = widget.GetLayoutBounds();

                // Align target position to world space, ensuring safe zones are respected
                targetPositions[widget] = new Vector3(currentX, currentY, currentZ);
                
                // Advance by the widget's reported width so larger panels do not overlap.
                currentX += bounds.x + Padding;
            }
        }
    }
}
