/*
 Code Artifact: PerformanceMonitor.cs
 Description: Shows FPS and frame timing so the team can check the Quest performance target during demos.
 Programmer: HudLink development team (Brandon Dodge, Zach Sevart, Asa Maker, Jonathan Gott, Josh Dwoskin)
 Date Created: 2026-03-09
 Revision History:
 - 2026-03-09 - Zach Sevart - Add HUD widget framework and scene
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
                int sampledFrameCount = _frameCount;
                float sampledTime = _timer;
                _fps = sampledFrameCount / sampledTime;
                _frameCount = 0;
                _timer = 0f;

                if (fpsText != null && fpsText.gameObject.activeSelf)
                {
                    float ms = _deltaTime / sampledFrameCount * 1000f;
                    _deltaTime = 0f;

                    fpsText.text = $"FPS: {_fps:F0} ({ms:F1} ms)";
                    fpsText.color = _fps >= 72f
                        ? new Color(0.4f, 1f, 0.5f)    // Green means the frame rate is meeting target.
                        : new Color(1f, 0.4f, 0.4f);   // Red means the frame rate is below target.
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
