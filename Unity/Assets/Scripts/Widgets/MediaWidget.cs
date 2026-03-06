using UnityEngine;
using UnityEngine.UI;

namespace HUDLink.Widgets
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

        public override void Initialize()
        {
            base.Initialize();
            WidgetEventBus.Subscribe<MediaPlaybackEvent>(OnMediaUpdate);
            Suspend(); // Hide completely when media is stopped/disconnected
            Debug.Log($"[{WidgetId}] MediaWidget Initialized.");
        }

        public override void DestroyWidget()
        {
            WidgetEventBus.Unsubscribe<MediaPlaybackEvent>(OnMediaUpdate);
            base.DestroyWidget();
        }

        private void OnMediaUpdate(MediaPlaybackEvent mediaEvent)
        {
            trackTitle = mediaEvent.TrackName;
            artistTitle = mediaEvent.ArtistName;
            isPlaying = mediaEvent.IsPlaying;
            estimatedProgress = mediaEvent.PlaybackProgress;

            if (isPlaying && isSuspended)
            {
                Resume(); // Unhide when music starts
            }
            else if (!isPlaying && !isSuspended)
            {
                Suspend(); // Hide to prevent cluttering the HUD safe zone
            }
        }

        protected override void RenderWidget(float deltaTime)
        {
            if (!isPlaying) return;

            // Simulate progress locally between BLE events to keep the bar smooth
            estimatedProgress += deltaTime * 0.01f; // Fake time advancement rate
            if (estimatedProgress > 1f) estimatedProgress = 0f;

            if (playbackProgressBar != null)
            {
                playbackProgressBar.value = estimatedProgress;
            }
        }
    }
}
