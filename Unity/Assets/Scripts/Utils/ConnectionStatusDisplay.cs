using UnityEngine;
using TMPro;

namespace HudLink.Utils
{
    /// <summary>
    /// Displays connection status indicator on the HUD.
    /// R4/R11 acceptance criteria: "Connection status is visible (Connected / Degraded / Disconnected)."
    /// For now, always shows "Mock Data" since BLE isn't implemented yet.
    /// </summary>
    public class ConnectionStatusDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI statusText;

        public enum ConnectionState
        {
            Disconnected,
            Connecting,
            Connected,
            Degraded,
            MockData
        }

        private ConnectionState _currentState = ConnectionState.MockData;

        public ConnectionState CurrentState => _currentState;

        private void Start()
        {
            SetState(ConnectionState.MockData);
        }

        public void SetState(ConnectionState state)
        {
            _currentState = state;

            if (statusText == null) return;

            switch (state)
            {
                case ConnectionState.Disconnected:
                    statusText.text = "Disconnected";
                    statusText.color = new Color(1f, 0.4f, 0.4f);
                    break;
                case ConnectionState.Connecting:
                    statusText.text = "Connecting...";
                    statusText.color = new Color(1f, 0.85f, 0.3f);
                    break;
                case ConnectionState.Connected:
                    statusText.text = "Connected";
                    statusText.color = new Color(0.4f, 1f, 0.5f);
                    break;
                case ConnectionState.Degraded:
                    statusText.text = "Degraded";
                    statusText.color = new Color(1f, 0.85f, 0.3f);
                    break;
                case ConnectionState.MockData:
                    statusText.text = "Mock Data";
                    statusText.color = new Color(0.5f, 0.7f, 1f);
                    break;
            }
        }
    }
}
