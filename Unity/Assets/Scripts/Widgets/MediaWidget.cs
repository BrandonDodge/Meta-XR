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
