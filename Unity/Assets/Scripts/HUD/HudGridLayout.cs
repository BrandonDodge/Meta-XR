/*
 Code Artifact: HudGridLayout.cs
 Description: Builds the 3x3 HUD slot grid and reserves the center safe zone for situational awareness.
 Programmer: HudLink development team (Brandon Dodge, Zach Sevart, Asa Maker, Jonathan Gott, Josh Dwoskin)
 Date Created: 2026-03-09
 Revision History:
 - 2026-04-13 - Brandon.Dodge - Update contribution ledger and Unity scripts
 - 2026-04-26 - HudLink development team - Added release prologue comments and tightened sprint-review documentation.
 - 2026-04-26 - HudLink development team - Matched headset grid slots to the virtual demo edge HUD layout.
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

namespace HudLink.HUD
{
    /// <summary>
    /// Manages the 3x3 grid layout for the HUD. The center cell is the "safe zone"
    /// that remains clear for situational awareness (per architecture wireframe Figure 4).
    ///
    /// Grid positions:
    ///   TopLeft(0)    TopCenter(1)    TopRight(2)
    ///   MidLeft(3)    [SafeZone](4)   MidRight(5)
    ///   BottomLeft(6) BottomCenter(7) BottomRight(8)
    /// </summary>
    public class HudGridLayout : MonoBehaviour
    {
        [SerializeField] private float cellWidth = 330f;
        [SerializeField] private float cellHeight = 158f;
        [SerializeField] private float spacing = 20f;
        [SerializeField] private float horizontalSpacing = 130f;
        [SerializeField] private float verticalSpacing = 12f;
        [SerializeField] private float verticalOffset = 52f;

        private RectTransform[] _slots;

        public enum SlotPosition
        {
            TopLeft = 0,
            TopCenter = 1,
            TopRight = 2,
            MidLeft = 3,
            SafeZone = 4,  // Reserved; do not place widgets here.
            MidRight = 5,
            BottomLeft = 6,
            BottomCenter = 7,
            BottomRight = 8
        }

        public void Initialize()
        {
            _slots = new RectTransform[9];

            for (int i = 0; i < 9; i++)
            {
                var slotGo = new GameObject($"Slot_{(SlotPosition)i}");
                slotGo.transform.SetParent(transform, false);

                var rect = slotGo.AddComponent<RectTransform>();
                rect.sizeDelta = new Vector2(cellWidth, cellHeight);

                // Convert the flat slot index into grid coordinates.
                int col = i % 3;
                int row = i / 3;

                // Center the grid: col 0 = left, col 1 = center, col 2 = right
                float x = (col - 1) * (cellWidth + horizontalSpacing);
                // Row 0 = top, row 1 = middle, row 2 = bottom
                float y = verticalOffset + (1 - row) * (cellHeight + verticalSpacing);

                rect.anchoredPosition = new Vector2(x, y);

                _slots[i] = rect;

                // Keep the center slot empty so it stays available for situational awareness.
                if (i == (int)SlotPosition.SafeZone)
                    slotGo.SetActive(false);
            }
        }

        public RectTransform GetSlot(SlotPosition position)
        {
            int index = (int)position;
            if (_slots == null || index < 0 || index >= _slots.Length)
                return null;
            return _slots[index];
        }

        public RectTransform GetSlot(int index)
        {
            if (_slots == null || index < 0 || index >= _slots.Length)
                return null;
            return _slots[index];
        }
    }
}
