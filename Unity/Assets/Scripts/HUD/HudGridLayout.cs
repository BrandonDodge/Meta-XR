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
        [SerializeField] private float cellWidth = 200f;
        [SerializeField] private float cellHeight = 140f;
        [SerializeField] private float spacing = 20f;

        private RectTransform[] _slots;

        public enum SlotPosition
        {
            TopLeft = 0,
            TopCenter = 1,
            TopRight = 2,
            MidLeft = 3,
            SafeZone = 4,  // Reserved — do not place widgets here
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

                int col = i % 3;
                int row = i / 3;

                // Center the grid: col 0 = left, col 1 = center, col 2 = right
                float x = (col - 1) * (cellWidth + spacing);
                // Row 0 = top, row 1 = middle, row 2 = bottom
                float y = (1 - row) * (cellHeight + spacing);

                rect.anchoredPosition = new Vector2(x, y);

                _slots[i] = rect;

                // Mark safe zone as inactive by default
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
