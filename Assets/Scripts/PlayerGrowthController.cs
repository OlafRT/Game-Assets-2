using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarterAssets
{
    public class PlayerGrowthController : MonoBehaviour
    {
        [Header("Growth Settings")]
        [Tooltip("Scale factor when the player grows")]
        public Vector3 growScale = new Vector3(2f, 2f, 2f); // Change this value to set the growth scale
        [Tooltip("Scale factor when the player shrinks (should be the inverse of growScale)")]
        public Vector3 shrinkScale = new Vector3(0.5f, 0.5f, 0.5f); // Change this value to set the shrink scale
        [Tooltip("Speed of growth/shrink")]
        public float growthSpeed = 2f; // Speed of the scaling transition

        [Header("Audio Settings")]
        public AudioClip growSound; // Assign this in the Inspector
        public AudioClip shrinkSound; // Assign this in the Inspector
        private AudioSource audioSource; // AudioSource component

        [Header("References")]
        public FirstPersonController firstPersonController; // Assign this in the Inspector

        private Vector3 _targetScale;
        private bool _isBig = false; // Track if the player is big or small
        private bool _isChangingSize = false; // Track if the size change is in progress

        // Store default values
        private float defaultMoveSpeed;
        private float defaultSprintSpeed;
        private float defaultJumpHeight;
        private float defaultGravity;
        private float defaultGroundedRadius;

        private void Awake()
        {
            // Ensure the FirstPersonController is assigned
            if (firstPersonController == null)
            {
                firstPersonController = GetComponent<FirstPersonController>();
                if (firstPersonController == null)
                {
                    Debug.LogError("FirstPersonController is not assigned and cannot be found on this GameObject.");
                }
            }

            audioSource = gameObject.AddComponent<AudioSource>(); // Add AudioSource component

            _targetScale = transform.localScale; // Initialize target scale

            // Store default values from FirstPersonController
            defaultMoveSpeed = firstPersonController.MoveSpeed;
            defaultSprintSpeed = firstPersonController.SprintSpeed;
            defaultJumpHeight = firstPersonController.JumpHeight;
            defaultGravity = firstPersonController.Gravity;
            defaultGroundedRadius = firstPersonController.GroundedRadius;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G) && !_isChangingSize)
            {
                ToggleSize();
            }

            // Gradually change the scale
            if (_isChangingSize)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, _targetScale, Time.deltaTime * growthSpeed);
                if (Vector3.Distance(transform.localScale, _targetScale) < 0.01f) // Check if close enough to target
                {
                    transform.localScale = _targetScale; // Snap to target scale
                    _isChangingSize = false; // Stop changing size
                }
            }
        }

        private void ToggleSize()
        {
            _isChangingSize = true; // Start changing size
            if (_isBig)
            {
                // Set target scale to shrink
                _targetScale = shrinkScale;

                // Reset FirstPersonController values to default
                firstPersonController.MoveSpeed = defaultMoveSpeed;
                firstPersonController.SprintSpeed = defaultSprintSpeed;
                firstPersonController.JumpHeight = defaultJumpHeight;
                firstPersonController.Gravity = defaultGravity;
                firstPersonController.GroundedRadius = defaultGroundedRadius;

                // Play shrink sound
                if (shrinkSound != null)
                {
                    audioSource.PlayOneShot(shrinkSound);
                }

                Debug.Log("Shrinking: Resetting to default values.");
            }
            else
            {
                // Set target scale to grow
                _targetScale = growScale;

                // Update FirstPersonController values for big size
                firstPersonController.MoveSpeed = 55f;
                firstPersonController.SprintSpeed = 120f;
                firstPersonController.JumpHeight = 20f;
                firstPersonController.Gravity = -120f;
                firstPersonController.GroundedRadius = 4f;

                // Play grow sound
                if (growSound != null)
                {
                    audioSource.PlayOneShot(growSound);
                }

                Debug.Log("Growing: Setting new values.");
            }

            _isBig = !_isBig; // Toggle the state
        }
    }
}