using UnityEngine;

namespace HudLink.Utils
{
    /// <summary>
    /// Handles buffering and smoothing of incoming data.
    /// Reduces abrupt value jumps and prevents noisy UI behavior.
    /// Meets FR-4.2 (Latency & Smoothing).
    /// </summary>
    public class DataSmoother
    {
        private float currentValue;
        private float targetValue;
        private float smoothSpeed;
        private bool isInitialized = false;

        public DataSmoother(float initialSmoothSpeed = 5f)
        {
            smoothSpeed = initialSmoothSpeed;
        }

        /// <summary>
        /// Updates the target destination for the smoothing function.
        /// </summary>
        public void SetTarget(float newTarget)
        {
            targetValue = newTarget;
            if (!isInitialized)
            {
                currentValue = targetValue;
                isInitialized = true;
            }
        }

        /// <summary>
        /// Call periodically to tick the smoothed value closer to the target.
        /// Returns the new smoothed value.
        /// </summary>
        public float Update(float deltaTime)
        {
            if (!isInitialized) return 0f;

            // Simple Lerp smoothing. Could be replaced with moving average or PID if needed later.
            currentValue = Mathf.Lerp(currentValue, targetValue, smoothSpeed * deltaTime);
            return currentValue;
        }

        public float GetCurrent() => currentValue;
        
        /// <summary>
        /// Instantly snaps the current value to the target.
        /// Useful when the connection is restored after a long dropout.
        /// </summary>
        public void SnapToTarget()
        {
            currentValue = targetValue;
        }
        
        public void Reset()
        {
            isInitialized = false;
        }
    }
}
