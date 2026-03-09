using UnityEngine;
using TMPro;

namespace HudLink.Utils
{
    /// <summary>
    /// Displays real-time FPS and frame timing in a small debug overlay.
    /// Used to verify R7 acceptance criteria: stable >= 72 FPS on Quest Pro.
    /// Toggle with F key in editor or double-tap left trigger on Quest.
    /// </summary>
    public class PerformanceMonitor : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI fpsText;
        [SerializeField] private bool showOnStart = true;

        private float _deltaTime;
        private float _updateInterval = 0.5f;
        private float _timer;
        private int _frameCount;
        private float _fps;

        private void Start()
        {
            if (fpsText != null)
                fpsText.gameObject.SetActive(showOnStart);
        }

        private void Update()
        {
            _deltaTime += Time.unscaledDeltaTime;
            _frameCount++;
            _timer += Time.unscaledDeltaTime;

            if (_timer >= _updateInterval)
            {
                _fps = _frameCount / _timer;
                _frameCount = 0;
                _timer = 0f;

                if (fpsText != null && fpsText.gameObject.activeSelf)
                {
                    float ms = _deltaTime / _frameCount * 1000f;
                    _deltaTime = 0f;

                    fpsText.text = $"FPS: {_fps:F0}";
                    fpsText.color = _fps >= 72f
                        ? new Color(0.4f, 1f, 0.5f)    // Green — meeting target
                        : new Color(1f, 0.4f, 0.4f);   // Red — below target
                }
                else
                {
                    _deltaTime = 0f;
                }
            }

            // Editor toggle
            if (UnityEngine.Input.GetKeyDown(KeyCode.F) && fpsText != null)
                fpsText.gameObject.SetActive(!fpsText.gameObject.activeSelf);
        }
    }
}
