/*
 Code Artifact: MediaWidget.cs
 Description: Displays phone media playback state and keeps a progress bar moving between data updates.
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
using UnityEngine.UI;

namespace HudLink.Widgets
{
    /// <summary>
    /// Displays currently playing media from the smartphone.
    /// Incorporates dynamic scrubbing/progress visualization mapped to Unity sliders.
    /// </summary>
    public class MediaWidget : BaseWidget
    {
        [Header("Media Layout")]
        [Tooltip("Maps to an AR UI Slider element")]
        [SerializeField] private Slider playbackProgressBar;
        
        // Private state
        private string trackTitle;
        private string artistTitle;
        private bool isPlaying = false;
        private float estimatedProgress = 0f;

        public override void Initialize(RectTransform slot)
        {
            base.Initialize(slot);
            WidgetEventBus.Subscribe<MediaPlaybackEvent>(OnMediaUpdate);
            Hide();
            Debug.Log($"[{WidgetId}] MediaWidget Initialized.");
        }

        public override void UpdateData(WidgetData data)
        {
            // This widget currently receives updates via WidgetEventBus.
        }

        public override void Dispose()
        {
            WidgetEventBus.Unsubscribe<MediaPlaybackEvent>(OnMediaUpdate);
            base.Dispose();
        }

        private void OnMediaUpdate(MediaPlaybackEvent mediaEvent)
        {
            trackTitle = mediaEvent.TrackName;
            artistTitle = mediaEvent.ArtistName;
            isPlaying = mediaEvent.IsPlaying;
            estimatedProgress = mediaEvent.PlaybackProgress;

            if (isPlaying && !IsVisible)
            {
                Show();
            }
            else if (!isPlaying && IsVisible)
            {
                Hide();
            }
        }

        private void Update()
        {
            if (!Initialized || !isPlaying) return;

            // Simulate progress locally between BLE events to keep the bar smooth
            estimatedProgress += Time.deltaTime * 0.01f; // Fake time advancement rate
            if (estimatedProgress > 1f) estimatedProgress = 0f;

            if (playbackProgressBar != null)
            {
                playbackProgressBar.value = estimatedProgress;
            }
        }
    }
}
