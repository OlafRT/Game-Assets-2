using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
	[RequireComponent(typeof(PlayerInput))]
#endif


	public class FirstPersonController : MonoBehaviour
	{
		[Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 4.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 6.0f;
		[Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 1.0f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;

		[Space(10)]
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.2f;
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -15.0f;

		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.1f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.5f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;

		[Header("Animation")]
		[Tooltip("Animator component controlling the character animations")]
		public Animator animatorComponent;
		private float _turnDirection = 0f; // for turning left or right
		private float _speed = 0f; // for walk/idle blend tree
		public Transform neckJoint; // Reference to the neck joint of the character rig

		// cinemachine
		private float _cinemachineTargetPitch;

		// player
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

#if ENABLE_INPUT_SYSTEM
		private PlayerInput _playerInput;
#endif
		private CharacterController _controller;
		private StarterAssetsInputs _input;
		private GameObject _mainCamera;

		private const float _threshold = 0.01f;

		private bool IsCurrentDeviceMouse
		{
			get
			{
#if ENABLE_INPUT_SYSTEM
				return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
			}
		}

		private void Awake()
		{
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
		}

		private void Start()
		{
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();

		#if ENABLE_INPUT_SYSTEM
			_playerInput = GetComponent<PlayerInput>();
		#else
			Debug.LogError("Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
		#endif

			// Reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;

			// Use the public Animator reference instead of the local one
			if (animatorComponent == null)
			{
				animatorComponent = GetComponentInChildren<Animator>();  // Get the Animator from the child object if not assigned
			}
			
			// Ensure the Animator is assigned
			if (animatorComponent == null)
			{
				Debug.LogError("Animator is not assigned!");
			}
		}

		private void Update()
		{
			GroundedCheck(); // Check grounded state first
			JumpAndGravity();
			Move();
			UpdateAnimations();
		}

		private void LateUpdate()
		{
			CameraRotation(); // Handle camera rotation
			UpdateNeckJointRotation(); // Update neck joint rotation
		}

		private void UpdateNeckJointRotation()
		{
			if (neckJoint != null)
			{
				// Adjust neck joint rotation based on camera pitch
				float horizontalRotation = _input.look.x; // Get horizontal look input (left/right)
				
				// Combine the rotations
				neckJoint.localRotation = Quaternion.Euler(0, horizontalRotation, -_cinemachineTargetPitch); // Negate the Z-axis for up/down
			}
		}

		private void GroundedCheck()
		{
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
		}

		private void CameraRotation()
		{
			// if there is an input
			if (_input.look.sqrMagnitude >= _threshold)
			{
				//Don't multiply mouse input by Time.deltaTime
				float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

				_cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
				_rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

				// clamp our pitch rotation
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

				// Update Cinemachine camera target pitch
				CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

				// rotate the player left and right
				transform.Rotate(Vector3.up * _rotationVelocity);

				// Update neck joint rotation to match camera rotation
				if (neckJoint != null)
				{
					neckJoint.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0, 0); // Adjust as needed
				}
			}
		}

		private void Move()
		{
			// Set target speed based on move speed, sprint speed and if sprint is pressed
			float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

			// If no input, set target speed to 0 (idle)
			if (_input.move == Vector2.zero) targetSpeed = 0.0f;

			// Get the current horizontal speed based on the CharacterController's velocity
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			// Offset and input magnitude for acceleration/deceleration
			float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			// Smoothly interpolate between current speed and target speed
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
				_speed = Mathf.Round(_speed * 1000f) / 1000f; // Round to 3 decimal places
			}
			else
			{
				_speed = targetSpeed;
			}

			// Determine the direction of movement based on input
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			if (_input.move != Vector2.zero)
			{
				inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
			}

			// Move the player using the CharacterController's Move method
			_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

			// Update animations based on movement and rotation
			UpdateAnimations();
		}

		private void JumpAndGravity()
		{
			if (Grounded)
			{
				_fallTimeoutDelta = FallTimeout;

				// Reset vertical velocity when grounded
				if (_verticalVelocity < 0.0f)
				{
					_verticalVelocity = -2f; // Small negative value to keep grounded
				}

				// Check for jump input
				if (_input.jump && _jumpTimeoutDelta <= 0.0f)
				{
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity); // Calculate jump velocity
					animatorComponent.SetBool("Jump", true); // Set jump animation to true
				}

				// Decrease jump timeout
				if (_jumpTimeoutDelta >= 0.0f)
				{
					_jumpTimeoutDelta -= Time.deltaTime;
				}
			}
			else
			{
				// Reset jump timeout when not grounded
				_jumpTimeoutDelta = JumpTimeout;

				// Decrease fall timeout
				if (_fallTimeoutDelta >= 0.0f)
				{
					_fallTimeoutDelta -= Time.deltaTime;
				}

				// Disable jump input when falling
				_input.jump = false;

				// If falling or grounded, set jump animation to false
				if (_verticalVelocity < 0)
				{
					animatorComponent.SetBool("Jump", false);
					animatorComponent.SetBool("Fall", true);  // Set falling animation to true
				}
				else
				{
					animatorComponent.SetBool("Jump", false);
					animatorComponent.SetBool("Fall", false); // When grounded, no jump or fall animation
				}
			}

			// Always apply gravity
			_verticalVelocity += Gravity * Time.deltaTime;

			// Clamp vertical velocity to terminal velocity
			if (_verticalVelocity < -_terminalVelocity)
			{
				_verticalVelocity = -_terminalVelocity; // Prevent falling too fast
			}
		}

		private void UpdateAnimations()
		{
			if (animatorComponent != null)
			{
				// Update the Speed parameter for the blend tree (idle vs walk vs run)
				animatorComponent.SetFloat("Speed", _speed);  // Update speed for walking or running

				// Update the Turn parameter based on rotation velocity
				if (_rotationVelocity != 0f)  // Player is turning
				{
					// Determine the direction of turn (left or right)
					if (_rotationVelocity > 0f)  // Turning right
					{
						_turnDirection = 1f; // Right turn
					}
					else if (_rotationVelocity < 0f)  // Turning left
					{
						_turnDirection = -1f; // Left turn
					}

					// Set the Turn parameter to trigger turning animation
					animatorComponent.SetFloat("Turn", _turnDirection);  // Set turn direction (left or right)
				}
				else // Not turning
				{
					animatorComponent.SetFloat("Turn", 0f);  // Set to 0 if not turning
				}

				// Set Jump and Fall states based on vertical velocity
				if (_verticalVelocity > 0f) // Jumping up
				{
					animatorComponent.SetBool("Jump", true);
					animatorComponent.SetBool("Fall", false);  // If jumping, don't show falling animation
				}
				else if (_verticalVelocity < 0f) // Falling down
				{
					animatorComponent.SetBool("Jump", false);
					animatorComponent.SetBool("Fall", true);  // If falling, show falling animation
				}
				else
				{
					animatorComponent.SetBool("Jump", false);
					animatorComponent.SetBool("Fall", false); // When grounded, no jump or fall animation
				}

				// Reset Jump boolean when grounded
				if (Grounded)
				{
					animatorComponent.SetBool("Jump", false); // Reset jump when grounded
				}
			}
		}
		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			Gizmos.color = Grounded ? transparentGreen : transparentRed;
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}
	}
}