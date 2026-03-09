using UnityEngine;

namespace HudLink.HUD
{
    /// <summary>
    /// Makes the HUD canvas follow the user's head with smooth lag.
    /// Uses a "tag-along" behavior: the HUD stays in place until the user
    /// turns beyond a threshold, then smoothly catches up. This prevents
    /// the HUD from feeling rigidly head-locked (which causes nausea)
    /// while keeping it accessible.
    /// </summary>
    public class HudFollowHead : MonoBehaviour
    {
        [SerializeField] private Transform headTransform;
        [SerializeField] private float distanceFromHead = 2.0f;
        [SerializeField] private float followSpeed = 3.0f;
        [SerializeField] private float angleThreshold = 15f;

        private Vector3 _targetPosition;
        private Quaternion _targetRotation;
        private bool _needsUpdate = true;

        private void Start()
        {
            if (headTransform == null)
            {
                var cam = Camera.main;
                if (cam != null)
                    headTransform = cam.transform;
            }

            // Snap to initial position
            UpdateTarget();
            transform.position = _targetPosition;
            transform.rotation = _targetRotation;
        }

        private void LateUpdate()
        {
            if (headTransform == null) return;

            float angle = Quaternion.Angle(transform.rotation, GetDesiredRotation());

            if (angle > angleThreshold)
                _needsUpdate = true;

            if (_needsUpdate)
            {
                UpdateTarget();

                transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * followSpeed);
                transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * followSpeed);

                if (angle < 1f)
                    _needsUpdate = false;
            }
        }

        private void UpdateTarget()
        {
            _targetPosition = headTransform.position + headTransform.forward * distanceFromHead;
            _targetRotation = GetDesiredRotation();
        }

        private Quaternion GetDesiredRotation()
        {
            // Face the HUD toward the user — only use Y rotation from head to prevent tilting
            Vector3 forward = headTransform.forward;
            forward.y = 0;
            if (forward.sqrMagnitude < 0.001f)
                forward = Vector3.forward;
            return Quaternion.LookRotation(forward.normalized);
        }
    }
}
