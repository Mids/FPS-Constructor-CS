using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	[RequireComponent(typeof(CharacterController))]
	[RequireComponent(typeof(MovementValues))]
	public class CharacterMotorDB : MonoBehaviour
	{
		public bool canControl = true;

		public static bool paused = false;

		public bool useFixedUpdate = true;

		public float crouchDeltaHeight = 1.0f;
		public float proneDeltaHeight = 1.5f;
		public bool useProne = true;
		private GameObject weaponCamera;
		private float standardCamHeight;
		private float proneCamHeight;
		private float crouchingCamHeight;
		public static bool crouching = false;
		public static bool prone = false;
		public static bool walking = false;
		public static bool sprinting = false;
		[HideInInspector]
		public bool stopCrouching = false;
		[HideInInspector]
		public bool stopProne = false;
		public float velocityFactor = 2;
		private bool canSprint = true;
		public static float maxSpeed;
		private bool diving = false;
		private float standardHeight;
		private float standardCenter;
		public float camSpeed = 8;
		private float lastCamSpeed = 8;
		public AudioClip jumpSound;
		public float jumpSoundVolume = .2f;
		public AudioClip landSound;
		public float landSoundVolume = .13f;
		public AudioClip proneLandSound;
		public float proneLandSoundVolume = .8f;

		public bool hitProne = false;
		public bool proneFrame = false;


		public bool aim = false;

		// For the next variables, [System.NonSerialized] tells Unity to not serialize the variable or show it in the inspector view.
		// Very handy for organization!

		// The current global direction we want the character to move in.
		[System.NonSerialized]
		public Vector3 inputMoveDirection = Vector3.zero;

		// Is the jump button held down? We use this interface instead of checking
		// for the jump button directly so this script can also be used by AIs.
		[System.NonSerialized]
		public bool inputJump = false;

		public class CharacterMotorDBMovement
		{
			// The maximum horizontal speed when moving
			[HideInInspector]
			public float maxForwardSpeed = 10.0f;
			[HideInInspector]
			public float defaultForwardSpeed = 10;
			[HideInInspector]
			public float maxCrouchSpeed = 6;
			[HideInInspector]
			public float maxSprintSpeed = 13;
			[HideInInspector]
			public float minSprintSpeed = 10;
			[HideInInspector]
			public float maxAimSpeed = 4;
			[HideInInspector]
			public float maxProneSpeed = 4;

			[HideInInspector]
			public float maxSidewaysSpeed = 10.0f;
			[HideInInspector]
			public float defaultSidewaysSpeed = 10;
			[HideInInspector]
			public float sprintSidewaysSpeed = 15;
			[HideInInspector]
			public float crouchSidewaysSpeed = 6;
			[HideInInspector]
			public float aimSidewaysSpeed = 4;
			[HideInInspector]
			public float proneSidewaysSpeed = 2;

			[HideInInspector]
			public float maxBackwardsSpeed = 10.0f;
			[HideInInspector]
			public float defaultBackwardsSpeed = 10;
			[HideInInspector]
			public float crouchBackwardsSpeed = 6;
			[HideInInspector]
			public float aimBackwardsSpeed = 4;
			[HideInInspector]
			public float proneBackwardsSpeed = 2;

			// Curve for multiplying speed based on slope (negative = downwards)
			public AnimationCurve slopeSpeedMultiplier = new AnimationCurve(new Keyframe(-90, 1), new Keyframe(0, 1), new Keyframe(90, 0));

			// How fast does the character change speeds?  Higher is faster.
			public float maxGroundAcceleration = 30.0f;
			public float defaultGroundAcceleration = 30;
			public float sprintGroundAcceleration = 50;
			public float maxAirAcceleration = 20.0f;
			public float defaultAirAcceleration = 20;
			public float sprintAirAcceleration = 25;
			public bool useDive = false;

			public float crouchedTime;
			public float proneHoldTime = .3f;

			// The gravity for the character
			public float gravity = 10.0f;
			public float maxFallSpeed = 20.0f;
			public float fallDamageStart = 9;
			public float fallDamageEnd = 50;
			public float fallDamageMax = 100;

			// For the next variables, [System.NonSerialized] tells Unity to not serialize the variable or show it in the inspector view.
			// Very handy for organization!

			// The last collision flags returned from controller.Move
			[System.NonSerialized]
			public CollisionFlags collisionFlags;

			// We will keep track of the character's current velocity,
			[System.NonSerialized]
			public Vector3 velocity;

			// This keeps track of our current velocity while we're not grounded
			[System.NonSerialized]
			public Vector3 frameVelocity = Vector3.zero;

			[System.NonSerialized]
			public Vector3 hitPoint = Vector3.zero;

			[System.NonSerialized]
			public Vector3 lastHitPoint = new Vector3(Mathf.Infinity, 0, 0);
		}

		public CharacterMotorDBMovement movement = new CharacterMotorDBMovement();

		public enum MovementTransferOnJumpDB
		{
			None, // The jump is not affected by velocity of floor at all.
			InitTransfer, // Jump gets its initial velocity from the floor, then gradualy comes to a stop.
			PermaTransfer, // Jump gets its initial velocity from the floor, and keeps that velocity until landing.
			PermaLocked // Jump is relative to the movement of the last touched floor and will move together with that floor.
		}

		// We will contain all the jumping related variables in one helper class for clarity.
		public class CharacterMotorDBJumping
		{
			// Can the character jump?
			public bool enabled = true;

			// How high do we jump when pressing jump and letting go immediately
			public float baseHeight = 1.0f;

			// We add extraHeight units (meters) on top when holding the button down longer while jumping
			public float extraHeight = 4.1f;

			// How much does the character jump out perpendicular to the surface on walkable surfaces?
			// 0 means a fully vertical jump and 1 means fully perpendicular.
			public float perpAmount = 0.0f;

			// How much does the character jump out perpendicular to the surface on too steep surfaces?
			// 0 means a fully vertical jump and 1 means fully perpendicular.
			public float steepPerpAmount = 0.5f;

			// For the next variables, [System.NonSerialized] tells Unity to not serialize the variable or show it in the inspector view.
			// Very handy for organization!

			// Are we jumping? (Initiated with jump button and not grounded yet)
			// To see if we are just in the air (initiated by jumping OR falling) see the grounded variable.
			[System.NonSerialized]
			public bool jumping = false;

			[System.NonSerialized]
			public bool holdingJumpButton = false;

			// the time we jumped at (Used to determine for how long to apply extra jump power after jumping.)
			[System.NonSerialized]
			public float lastStartTime = 0.0f;

			[System.NonSerialized]
			public float lastButtonDownTime = -100;

			[System.NonSerialized]
			public Vector3 jumpDir = Vector3.up;
		}

		public CharacterMotorDBJumping jumping = new CharacterMotorDBJumping();

		public class CharacterMotorDBMovingPlatform
		{
			public bool enabled = true;

			public MovementTransferOnJumpDB movementTransfer = MovementTransferOnJumpDB.PermaTransfer;

			[System.NonSerialized]
			public Transform hitPlatform;

			[System.NonSerialized]
			public Transform activePlatform;

			[System.NonSerialized]
			public Vector3 activeLocalPoint;

			[System.NonSerialized]
			public Vector3 activeGlobalPoint;

			[System.NonSerialized]
			public Quaternion activeLocalRotation;

			[System.NonSerialized]
			public Quaternion activeGlobalRotation;

			[System.NonSerialized]
			public Matrix4x4 lastMatrix;

			[System.NonSerialized]
			public Vector3 platformVelocity;

			[System.NonSerialized]
			public bool newPlatform;
		}

		public CharacterMotorDBMovingPlatform movingPlatform = new CharacterMotorDBMovingPlatform();

		class CharacterMotorDBSliding
		{
			// Does the character slide on too steep surfaces?
			public bool enabled = true;

			// How fast does the character slide on steep surfaces?
			public float slidingSpeed = 15;

			// How much can the player control the sliding direction?
			// If the value is 0.5f the player can slide sideways with half the speed of the downwards sliding speed.
			public float sidewaysControl = 1.0f;

			// How much can the player influence the sliding speed?
			// If the value is 0.5f the player can speed the sliding up to 150% or slow it down to 50%.
			public float speedControl = 0.4f;
		}

		public CharacterMotorDBSliding sliding = new CharacterMotorDBSliding();

		[System.NonSerialized]
		public bool grounded = true;

		[System.NonSerialized]
		public Vector3 groundNormal = Vector3.zero;

		private Vector3 lastGroundNormal = Vector3.zero;

		private Transform tr;

		private CharacterController controller;

		void Awake()
		{
			MovementValues values = this.GetComponent<MovementValues>();
			if (values != null)
			{
				movement.defaultForwardSpeed = values.defaultForwardSpeed;
				movement.maxCrouchSpeed = values.maxCrouchSpeed;
				movement.maxSprintSpeed = values.maxSprintSpeed;
				movement.minSprintSpeed = values.minSprintSpeed;
				movement.maxAimSpeed = values.maxAimSpeed;
				movement.maxProneSpeed = values.maxProneSpeed;

				movement.defaultSidewaysSpeed = values.defaultSidewaysSpeed;
				movement.sprintSidewaysSpeed = values.sprintSidewaysSpeed;
				movement.crouchSidewaysSpeed = values.crouchSidewaysSpeed;
				movement.aimSidewaysSpeed = values.aimSidewaysSpeed;
				movement.proneSidewaysSpeed = values.proneSidewaysSpeed;

				movement.defaultBackwardsSpeed = values.defaultBackwardsSpeed;
				movement.crouchBackwardsSpeed = values.crouchBackwardsSpeed;
				movement.aimBackwardsSpeed = values.aimBackwardsSpeed;
				movement.proneBackwardsSpeed = values.proneBackwardsSpeed;
			}

			controller = GetComponent<CharacterController>();
			standardHeight = controller.height;
			standardCenter = controller.center.y;
			tr = transform;
			weaponCamera = GameObject.FindWithTag("WeaponCamera");
			crouching = false;
			prone = false;
			standardCamHeight = weaponCamera.transform.localPosition.y;
			crouchingCamHeight = standardCamHeight - crouchDeltaHeight;
			proneCamHeight = standardCamHeight - proneDeltaHeight;
			NormalSpeed();
		}

		private void UpdateFunction()
		{
			if (paused)
				return;
			if (diving)
			{
				float yVeloc = movement.velocity.y;
				SetVelocity(transform.forward * 20 + Vector3.up * yVeloc);
			}

			// We copy the actual velocity into a temporary variable that we can manipulate.
			Vector3 velocity = movement.velocity;

			// Update velocity based on input
			velocity = ApplyInputVelocityChange(velocity);

			// Apply gravity and jumping force
			velocity = ApplyGravityAndJumping(velocity);

			// Moving platform support
			Vector3 moveDistance = Vector3.zero;
			if (MoveWithPlatform())
			{
				Vector3 newGlobalPoint = movingPlatform.activePlatform.TransformPoint(movingPlatform.activeLocalPoint);
				moveDistance = (newGlobalPoint - movingPlatform.activeGlobalPoint);
				if (moveDistance != Vector3.zero)
					controller.Move(moveDistance);

				// Support moving platform rotation as well:
				Quaternion newGlobalRotation = movingPlatform.activePlatform.rotation * movingPlatform.activeLocalRotation;
				Quaternion rotationDiff = newGlobalRotation * Quaternion.Inverse(movingPlatform.activeGlobalRotation);

				float yRotation = rotationDiff.eulerAngles.y;
				if (yRotation != 0)
				{
					// Prevent rotation of the local up vector
					tr.Rotate(0, yRotation, 0);
				}
			}

			// Save lastPosition for velocity calculation.
			Vector3 lastPosition = tr.position;

			// We always want the movement to be framerate independent.  Multiplying by Time.deltaTime does this.
			Vector3 currentMovementOffset = velocity * Time.deltaTime;

			// Find out how much we need to push towards the ground to avoid loosing grouning
			// when walking down a step or over a sharp change in slope.
			float pushDownOffset = Mathf.Max(controller.stepOffset, new Vector3(currentMovementOffset.x, 0, currentMovementOffset.z).magnitude);
			if (grounded)
				currentMovementOffset -= pushDownOffset * Vector3.up;

			// Reset variables that will be set by collision function
			movingPlatform.hitPlatform = null;
			groundNormal = Vector3.zero;

			// Move our character!
			movement.collisionFlags = controller.Move(currentMovementOffset);

			movement.lastHitPoint = movement.hitPoint;
			lastGroundNormal = groundNormal;

			if (movingPlatform.enabled && movingPlatform.activePlatform != movingPlatform.hitPlatform)
			{
				if (movingPlatform.hitPlatform != null)
				{
					movingPlatform.activePlatform = movingPlatform.hitPlatform;
					movingPlatform.lastMatrix = movingPlatform.hitPlatform.localToWorldMatrix;
					movingPlatform.newPlatform = true;
				}
			}

			// Calculate the velocity based on the current and previous position.  
			// This means our velocity will only be the amount the character actually moved as a result of collisions.
			Vector3 oldHVelocity = new Vector3(velocity.x, 0, velocity.z);
			movement.velocity = (tr.position - lastPosition) / Time.deltaTime;
			Vector3 newHVelocity = new Vector3(movement.velocity.x, 0, movement.velocity.z);

			// The CharacterController can be moved in unwanted directions when colliding with things.
			// We want to prevent this from influencing the recorded velocity.
			if (oldHVelocity == Vector3.zero)
			{
				movement.velocity = new Vector3(0, movement.velocity.y, 0);
			}
			else
			{
				float projectedNewVelocity = Vector3.Dot(newHVelocity, oldHVelocity) / oldHVelocity.sqrMagnitude;
				movement.velocity = oldHVelocity * Mathf.Clamp01(projectedNewVelocity) + movement.velocity.y * Vector3.up;
			}

			if (movement.velocity.y < velocity.y - 0.001f)
			{
				if (movement.velocity.y < 0)
				{
					// Something is forcing the CharacterController down faster than it should.
					// Ignore this
					movement.velocity.y = velocity.y;
				}
				else
				{
					// The upwards movement of the CharacterController has been blocked.
					// This is treated like a ceiling collision - stop further jumping here.
					jumping.holdingJumpButton = false;
				}
			}

			// We were grounded but just loosed grounding
			if (grounded && !IsGroundedTest())
			{
				grounded = false;

				// Apply inertia from platform
				if (movingPlatform.enabled &&
					(movingPlatform.movementTransfer == MovementTransferOnJumpDB.InitTransfer ||
					movingPlatform.movementTransfer == MovementTransferOnJumpDB.PermaTransfer)
				)
				{
					movement.frameVelocity = movingPlatform.platformVelocity;
					movement.velocity += movingPlatform.platformVelocity;
				}

				SendMessage("OnFall", SendMessageOptions.DontRequireReceiver);
				// We pushed the character down to ensure it would stay on the ground if there was any.
				// But there wasn't so now we cancel the downwards offset to make the fall smoother.
				tr.position += pushDownOffset * Vector3.up;
			}
			// We were not grounded but just landed on something
			else if (!grounded && IsGroundedTest())
			{
				grounded = true;
				jumping.jumping = false;
				SubtractNewPlatformVelocity();

				if (diving)
				{
					diving = false;
					SetVelocity(transform.forward * 6);
					GetComponent<AudioSource>().volume = proneLandSoundVolume;
					GetComponent<AudioSource>().PlayOneShot(proneLandSound);
					camSpeed = lastCamSpeed;
					//Prone();
				}

				//Fall Damage!!!

				SendMessage("OnLand", SendMessageOptions.DontRequireReceiver);
				BroadcastMessage("Landed", SendMessageOptions.DontRequireReceiver);
				float fallVal = ((-1 * velocity.y) - movement.fallDamageStart) / (movement.fallDamageEnd - movement.fallDamageStart);
				fallVal *= movement.fallDamageMax;
				fallVal = Mathf.Round(fallVal);
				GunLook.jostleAmt = new Vector3(-.06f, -.1f, 0);
				CamSway.jostleAmt = new Vector3(-.01f, -.07f, 0);
				if (fallVal > 0 && PlayerWeapons.playerActive && !paused)
					BroadcastMessage("ApplyFallDamage", fallVal);
				//Debug.Log(fallVal);
				GetComponent<AudioSource>().volume = landSoundVolume;
				GetComponent<AudioSource>().PlayOneShot(landSound);

			}

			// Moving platforms support
			if (MoveWithPlatform())
			{
				// Use the center of the lower half sphere of the capsule as reference point.
				// This works best when the character is standing on moving tilting platforms. 
				movingPlatform.activeGlobalPoint = tr.position + Vector3.up * (controller.center.y - controller.height * 0.5f + controller.radius);
				movingPlatform.activeLocalPoint = movingPlatform.activePlatform.InverseTransformPoint(movingPlatform.activeGlobalPoint);

				// Support moving platform rotation as well:
				movingPlatform.activeGlobalRotation = tr.rotation;
				movingPlatform.activeLocalRotation = Quaternion.Inverse(movingPlatform.activePlatform.rotation) * movingPlatform.activeGlobalRotation;
			}
		}

		void FixedUpdate()
		{
			if (movingPlatform.enabled)
			{
				if (movingPlatform.activePlatform != null)
				{
					if (!movingPlatform.newPlatform)
					{
						Vector3 lastVelocity = movingPlatform.platformVelocity;

						movingPlatform.platformVelocity = (
							movingPlatform.activePlatform.localToWorldMatrix.MultiplyPoint3x4(movingPlatform.activeLocalPoint)
							- movingPlatform.lastMatrix.MultiplyPoint3x4(movingPlatform.activeLocalPoint)
						) / Time.deltaTime;
					}
					movingPlatform.lastMatrix = movingPlatform.activePlatform.localToWorldMatrix;
					movingPlatform.newPlatform = false;
				}
				else
				{
					movingPlatform.platformVelocity = Vector3.zero;
				}
			}

			if (useFixedUpdate)
				UpdateFunction();
		}

		void Update()
		{
			if (paused)
			{
				movement.velocity = Vector3.zero;
				return;
			}

			if ((inputMoveDirection.x != 0 || inputMoveDirection.y != 0 || inputMoveDirection.z != 0) && grounded && PlayerWeapons.canMove)
			{
				if (!walking)
					BroadcastMessage("Walking", SendMessageOptions.DontRequireReceiver);
				walking = true;
			}
			else
			{
				if (walking)
					BroadcastMessage("StopWalking", SendMessageOptions.DontRequireReceiver);
				walking = false;
			}

			if (!useFixedUpdate)
				UpdateFunction();

			/*if(weaponCamera.transform.localPosition.y > standardCamHeight){
				weaponCamera.transform.localPosition.y = standardCamHeight;
			} else if(weaponCamera.transform.localPosition.y < crouchingCamHeight){
				weaponCamera.transform.localPosition.y = crouchingCamHeight;
			}*/

			if (grounded)
			{
				if (InputDB.GetButtonUp("Crouch") && PlayerWeapons.canCrouch)
				{
					if (!proneFrame)
					{
						if (!crouching && !prone)
						{
							/*if(PlayerWeapons.sprinting && movement.useDive && !diving){
								SetVelocity(transform.forward*25 + Vector3.up*12);
								audio.volume = jumpSoundVolume;
								audio.PlayOneShot(jumpSound);
								diving = true;
								lastCamSpeed = camSpeed;
								crouching = false;
								Prone();
								camSpeed = 1;
								movement.crouchedTime = -1;
								//prone = true;
							} else {*/

							Crouch();
						}
						else if (crouching && AboveIsClear())
						{
							crouching = false;
							stopCrouching = true;
							NormalSpeed();
						}
						else if (prone && AboveIsClearProne())
						{
							//crouching = false;
							//stopProne = true;
							prone = false;
							Crouch();
						}
					}
					proneFrame = false;
				}
				else if (InputDB.GetButton("Crouch"))
				{
					if (movement.crouchedTime < 0)
						movement.crouchedTime = Time.time;
					if (Time.time > movement.crouchedTime + movement.proneHoldTime && movement.crouchedTime > 0 && !prone)
					{
						if (useProne)
						{
							Prone();
							proneFrame = true;
						}
						movement.crouchedTime = -1;
					}
				}
				else
				{
					movement.crouchedTime = -1;
				}
			}
			Vector3 weaponCameraLocalPos = weaponCamera.transform.localPosition;
			if (crouching && !prone)
			{
				if (weaponCameraLocalPos.y > crouchingCamHeight)
				{
					weaponCameraLocalPos.y = Mathf.Clamp(weaponCameraLocalPos.y - crouchDeltaHeight * Time.deltaTime * camSpeed, crouchingCamHeight, standardCamHeight);
				}
				else
				{
					weaponCameraLocalPos.y = Mathf.Clamp(weaponCameraLocalPos.y + crouchDeltaHeight * Time.deltaTime * camSpeed, proneCamHeight, crouchingCamHeight);
				}
			}
			else if (prone)
			{
				if (weaponCameraLocalPos.y > proneCamHeight)
				{
					weaponCameraLocalPos.y = Mathf.Clamp(weaponCameraLocalPos.y - crouchDeltaHeight * Time.deltaTime * camSpeed, proneCamHeight, weaponCameraLocalPos.y);
				}
				else if (!hitProne)
				{
					GunLook.jostleAmt = new Vector3(-.075f, -.12f, 0);
					CamSway.jostleAmt = new Vector3(-.01f, -.03f, 0);
					hitProne = true;
					GetComponent<AudioSource>().volume = proneLandSoundVolume;
					GetComponent<AudioSource>().PlayOneShot(proneLandSound);
				}
			}
			else
			{
				if (weaponCameraLocalPos.y < standardCamHeight)
				{
					weaponCameraLocalPos.y = Mathf.Clamp(weaponCameraLocalPos.y + standardCamHeight * Time.deltaTime * camSpeed, proneCamHeight, standardCamHeight);
				}
			}
			weaponCamera.transform.localPosition = weaponCameraLocalPos;
		}

		public void StandUp()
		{
			if (!AboveIsClear())
				return;
			if (crouching)
			{
				crouching = false;
				stopCrouching = true;
				NormalSpeed();
			}
			else if (prone)
			{
				stopProne = true;
				NormalSpeed();
			}
		}

		//Check if it is clear above us to stand up
		public bool AboveIsClear()
		{
			if (!crouching && !prone)
				return true;

			return !Physics.Raycast(transform.TransformPoint(controller.center) + (controller.height / 2) * Vector3.up, Vector3.up, standardHeight - controller.height);
		}

		//Check if it is clear for us to go to crouch
		public bool AboveIsClearProne()
		{
			return !Physics.Raycast(transform.TransformPoint(controller.center) + (controller.height / 2) * Vector3.up, Vector3.up, standardHeight - controller.height - crouchDeltaHeight);
		}

		private Vector3 ApplyInputVelocityChange(Vector3 velocity)
		{
			if (!canControl || !PlayerWeapons.canMove)
				inputMoveDirection = Vector3.zero;

			// Find desired velocity
			Vector3 desiredVelocity;
			if (grounded && TooSteep())
			{
				// The direction we're sliding in
				desiredVelocity = new Vector3(groundNormal.x, 0, groundNormal.z).normalized;
				// Find the input movement direction projected onto the sliding direction
				Vector3 projectedMoveDir = Vector3.Project(inputMoveDirection, desiredVelocity);
				// Add the sliding direction, the spped control, and the sideways control vectors
				desiredVelocity = desiredVelocity + projectedMoveDir * sliding.speedControl + (inputMoveDirection - projectedMoveDir) * sliding.sidewaysControl;
				// Multiply with the sliding speed
				desiredVelocity *= sliding.slidingSpeed;
			}
			else
				desiredVelocity = GetDesiredHorizontalVelocity();

			if (movingPlatform.enabled && movingPlatform.movementTransfer == MovementTransferOnJumpDB.PermaTransfer)
			{
				desiredVelocity += movement.frameVelocity;
				desiredVelocity.y = 0;
			}

			if (grounded)
				desiredVelocity = AdjustGroundVelocityToNormal(desiredVelocity, groundNormal);
			else
				velocity.y = 0;

			// Enforce max velocity change
			float maxVelocityChange = GetMaxAcceleration(grounded) * Time.deltaTime * velocityFactor;
			Vector3 velocityChangeVector = (desiredVelocity - velocity);
			if (velocityChangeVector.sqrMagnitude > maxVelocityChange * maxVelocityChange)
			{
				velocityChangeVector = velocityChangeVector.normalized * maxVelocityChange;
			}
			// If we're in the air and don't have control, don't apply any velocity change at all.
			// If we're on the ground and don't have control we do apply it - it will correspond to friction.
			if (grounded || canControl)
				velocity += velocityChangeVector;

			if (grounded)
			{
				// When going uphill, the CharacterController will automatically move up by the needed amount.
				// Not moving it upwards manually prevent risk of lifting off from the ground.
				// When going downhill, DO move down manually, as gravity is not enough on steep hills.
				velocity.y = Mathf.Min(velocity.y, 0);
			}

			return velocity;
		}

		private Vector3 ApplyGravityAndJumping(Vector3 velocity)
		{

			if (!inputJump || !canControl)
			{
				jumping.holdingJumpButton = false;
				jumping.lastButtonDownTime = -100;
			}

			if (inputJump && jumping.lastButtonDownTime < 0 && canControl && !prone)
				jumping.lastButtonDownTime = Time.time;

			if (grounded)
				velocity.y = Mathf.Min(0, velocity.y) - movement.gravity * Time.deltaTime;
			else
			{
				velocity.y = movement.velocity.y - movement.gravity * Time.deltaTime;

				// When jumping up we don't apply gravity for some time when the user is holding the jump button.
				// This gives more control over jump height by pressing the button longer.
				if (jumping.jumping && jumping.holdingJumpButton)
				{
					// Calculate the duration that the extra jump force should have effect.
					// If we're still less than that duration after the jumping time, apply the force.
					if (Time.time < jumping.lastStartTime + jumping.extraHeight / CalculateJumpVerticalSpeed(jumping.baseHeight))
					{
						// Negate the gravity we just applied, except we push in jumpDir rather than jump upwards.
						velocity += jumping.jumpDir * movement.gravity * Time.deltaTime;
					}
				}

				// Make sure we don't fall any faster than maxFallSpeed. This gives our character a terminal velocity.
				velocity.y = Mathf.Max(velocity.y, -movement.maxFallSpeed);
			}

			if (grounded)
			{
				// Jump only if the jump button was pressed down in the last 0.2f seconds.
				// We use this check instead of checking if it's pressed down right now
				// because players will often try to jump in the exact moment when hitting the ground after a jump
				// and if they hit the button a fraction of a second too soon and no new jump happens as a consequence,
				// it's confusing and it feels like the game is buggy.
				if (jumping.enabled && canControl && PlayerWeapons.canMove && (Time.time - jumping.lastButtonDownTime < 0.2f))
				{
					grounded = false;
					jumping.jumping = true;
					jumping.lastStartTime = Time.time;
					jumping.lastButtonDownTime = -100;
					jumping.holdingJumpButton = true;

					// Calculate the jumping direction
					if (TooSteep())
						jumping.jumpDir = Vector3.Slerp(Vector3.up, groundNormal, jumping.steepPerpAmount);
					else
						jumping.jumpDir = Vector3.Slerp(Vector3.up, groundNormal, jumping.perpAmount);

					// Apply the jumping force to the velocity. Cancel any vertical velocity first.
					velocity.y = 0;
					velocity += jumping.jumpDir * CalculateJumpVerticalSpeed(jumping.baseHeight);
					if (crouching)
					{
						stopCrouching = true;
						NormalSpeed();
					}
					if (prone)
					{
						stopProne = true;
						NormalSpeed();
						return Vector3.zero;//디폴트 값을 반환한다면 zero가 맞을터..
					}


					// Apply inertia from platform
					if (movingPlatform.enabled &&
						(movingPlatform.movementTransfer == MovementTransferOnJumpDB.InitTransfer ||
						movingPlatform.movementTransfer == MovementTransferOnJumpDB.PermaTransfer)
					)
					{
						movement.frameVelocity = movingPlatform.platformVelocity;
						velocity += movingPlatform.platformVelocity;
					}

					SendMessage("OnJump", SendMessageOptions.DontRequireReceiver);
					GetComponent<AudioSource>().volume = jumpSoundVolume;
					GetComponent<AudioSource>().PlayOneShot(jumpSound);
					BroadcastMessage("Airborne", SendMessageOptions.DontRequireReceiver);

				}
				else
				{
					jumping.holdingJumpButton = false;
				}
			}

			return velocity;
		}

		public void OnControllerColliderHit(ControllerColliderHit hit)
		{
			if (hit.normal.y > 0 && hit.normal.y > groundNormal.y && hit.moveDirection.y < 0)
			{
				if ((hit.point - movement.lastHitPoint).sqrMagnitude > 0.001f || lastGroundNormal == Vector3.zero)
					groundNormal = hit.normal;
				else
					groundNormal = lastGroundNormal;

				movingPlatform.hitPlatform = hit.collider.transform;
				movement.hitPoint = hit.point;
				movement.frameVelocity = Vector3.zero;
			}
		}
		private void SubtractNewPlatformVelocity()
		{
			StartCoroutine(SubtractNewPlatformVelocityRoutine());
		}
		private IEnumerator SubtractNewPlatformVelocityRoutine()
		{
			// When landing, subtract the velocity of the new ground from the character's velocity
			// since movement in ground is relative to the movement of the ground.
			if (movingPlatform.enabled &&
				(movingPlatform.movementTransfer == MovementTransferOnJumpDB.InitTransfer ||
				movingPlatform.movementTransfer == MovementTransferOnJumpDB.PermaTransfer)
			)
			{
				// If we landed on a new platform, we have to wait for two FixedUpdates
				// before we know the velocity of the platform under the character
				if (movingPlatform.newPlatform)
				{
					Transform platform = movingPlatform.activePlatform;
					yield return new WaitForFixedUpdate();
					yield return new WaitForFixedUpdate();
					if (grounded && platform == movingPlatform.activePlatform)
						yield return 1;
				}
				movement.velocity -= movingPlatform.platformVelocity;
			}
		}

		private bool MoveWithPlatform()
		{
			return (
			   movingPlatform.enabled
			   && (grounded || movingPlatform.movementTransfer == MovementTransferOnJumpDB.PermaLocked)
			   && movingPlatform.activePlatform != null
		   );
		}

		private Vector3 GetDesiredHorizontalVelocity()
		{
			// Find desired velocity
			Vector3 desiredLocalDirection = tr.InverseTransformDirection(inputMoveDirection);
			maxSpeed = MaxSpeedInDirection(desiredLocalDirection);
			if (grounded)
			{
				// Modify max speed on slopes based on slope speed multiplier curve
				float movementSlopeAngle = Mathf.Asin(movement.velocity.normalized.y) * Mathf.Rad2Deg;
				maxSpeed *= movement.slopeSpeedMultiplier.Evaluate(movementSlopeAngle);
			}
			return tr.TransformDirection(desiredLocalDirection * maxSpeed);
		}

		private Vector3 AdjustGroundVelocityToNormal(Vector3 hVelocity, Vector3 groundNormal)
		{
			Vector3 sideways = Vector3.Cross(Vector3.up, hVelocity);
			return Vector3.Cross(sideways, groundNormal).normalized * hVelocity.magnitude;
		}

		private bool IsGroundedTest()
		{
			return (groundNormal.y > 0.01f);
		}

		public float GetMaxAcceleration(bool grounded)
		{
			// Maximum acceleration on ground and in air
			if (grounded)
				return movement.maxGroundAcceleration;
			else
				return movement.maxAirAcceleration;
		}

		public float CalculateJumpVerticalSpeed(float targetJumpHeight)
		{
			// From the jump height and gravity we deduce the upwards speed 
			// for the character to reach at the apex.
			return Mathf.Sqrt(2 * targetJumpHeight * movement.gravity);
		}

		public bool IsJumping()
		{
			return jumping.jumping;
		}

		public bool IsSliding()
		{
			return (grounded && sliding.enabled && TooSteep());
		}

		public bool IsTouchingCeiling()
		{
			return (movement.collisionFlags & CollisionFlags.CollidedAbove) != 0;
		}

		public bool IsGrounded()
		{
			return grounded;
		}

		public bool TooSteep()
		{
			return (groundNormal.y <= Mathf.Cos(controller.slopeLimit * Mathf.Deg2Rad));
		}

		public Vector3 GetDirection()
		{
			return inputMoveDirection;
		}

		public void SetControllable(bool controllable)
		{
			canControl = controllable;
		}

		// Project a direction onto elliptical quater segments based on forward, sideways, and backwards speed.
		// The function returns the length of the resulting vector.
		public float MaxSpeedInDirection(Vector3 desiredMovementDirection)
		{
			if (desiredMovementDirection == Vector3.zero)
				return 0;
			else
			{
				float zAxisEllipseMultiplier = (desiredMovementDirection.z > 0 ? movement.maxForwardSpeed : movement.maxBackwardsSpeed) / movement.maxSidewaysSpeed;
				Vector3 temp = new Vector3(desiredMovementDirection.x, 0, desiredMovementDirection.z / zAxisEllipseMultiplier).normalized;
				float length = new Vector3(temp.x, 0, temp.z * zAxisEllipseMultiplier).magnitude * movement.maxSidewaysSpeed;
				return length;
			}
		}

		public void SetVelocity(Vector3 velocity)
		{
			grounded = false;
			movement.velocity = velocity;
			movement.frameVelocity = Vector3.zero;
			SendMessage("OnExternalVelocity", SendMessageOptions.DontRequireReceiver);
		}

		public void Aiming()
		{
			movement.maxForwardSpeed = Mathf.Min(movement.maxForwardSpeed, movement.maxAimSpeed);
			movement.maxSidewaysSpeed = Mathf.Min(movement.maxSidewaysSpeed, movement.aimSidewaysSpeed);
			movement.maxBackwardsSpeed = Mathf.Min(movement.maxBackwardsSpeed, movement.aimBackwardsSpeed);
			aim = true;
		}

		public void StopAiming()
		{
			aim = false;
			NormalSpeed();
		}

		public void Crouch()
		{
			weaponCamera.BroadcastMessage("Crouching", SendMessageOptions.DontRequireReceiver);
			movement.maxForwardSpeed = movement.maxCrouchSpeed;
			movement.maxSidewaysSpeed = movement.crouchSidewaysSpeed;
			movement.maxBackwardsSpeed = movement.crouchBackwardsSpeed;
			controller.height = standardHeight - crouchDeltaHeight;
			Vector3 newCenter = controller.center;
			newCenter.y = standardCenter - crouchDeltaHeight / 2;
			controller.center = newCenter;
			crouching = true;
			if (aim)
			{
				Aiming();
			}
		}

		public void Prone()
		{
			hitProne = false;
			weaponCamera.BroadcastMessage("Prone", SendMessageOptions.DontRequireReceiver);
			crouching = false;
			stopCrouching = false;
			movement.maxForwardSpeed = movement.maxProneSpeed;
			movement.maxSidewaysSpeed = movement.proneSidewaysSpeed;
			movement.maxBackwardsSpeed = movement.proneBackwardsSpeed;
			controller.height = standardHeight - proneDeltaHeight;
			Vector3 newCenter = controller.center;
			newCenter.y = standardCenter - proneDeltaHeight / 2;
			controller.center = newCenter;
			prone = true;
			if (aim)
			{
				Aiming();
			}
		}

		public void NormalSpeed()
		{
			sprinting = false;
			if (stopCrouching)
			{
				crouching = false;
				controller.height += crouchDeltaHeight;
				BroadcastMessage("StopWalking", SendMessageOptions.DontRequireReceiver);
				controller.center += new Vector3(0, crouchDeltaHeight / 2, 0);
				stopCrouching = false;
			}
			else if (crouching)
			{
				movement.maxForwardSpeed = movement.maxCrouchSpeed;
				movement.maxSidewaysSpeed = movement.crouchSidewaysSpeed;
				movement.maxBackwardsSpeed = movement.crouchBackwardsSpeed;
				if (aim)
				{
					Aiming();
				}
				return;
			}
			else if (stopProne)
			{
				prone = false;
				controller.height += proneDeltaHeight;
				BroadcastMessage("StopWalking", SendMessageOptions.DontRequireReceiver);
				controller.center += new Vector3(0, proneDeltaHeight / 2, 0);
				stopProne = false;
			}
			else if (prone)
			{
				movement.maxForwardSpeed = movement.maxProneSpeed;
				movement.maxSidewaysSpeed = movement.proneSidewaysSpeed;
				movement.maxBackwardsSpeed = movement.proneBackwardsSpeed;
				if (aim)
				{
					Aiming();
				}
				return;
			}
			movement.maxBackwardsSpeed = movement.defaultBackwardsSpeed;
			movement.maxSidewaysSpeed = movement.defaultSidewaysSpeed;
			movement.maxForwardSpeed = movement.defaultForwardSpeed;
			movement.maxAirAcceleration = movement.defaultAirAcceleration;
			movement.maxGroundAcceleration = movement.defaultGroundAcceleration;
			if (aim)
			{
				Aiming();
			}
		}

		public void Sprinting()
		{
			sprinting = true;
			if (crouching)
			{
				crouching = false;
				controller.height += crouchDeltaHeight;
				BroadcastMessage("StopWalking", SendMessageOptions.DontRequireReceiver);
				controller.center += new Vector3(0, crouchDeltaHeight / 2, 0);
			}
			/*if(prone){
				prone = false;
				controller.height += proneDeltaHeight;
				BroadcastMessage("StopWalking", SendMessageOptions.DontRequireReceiver);
				controller.center += Vector3(0,proneDeltaHeight/2, 0);
			}*/
			if (canSprint)
			{
				movement.maxForwardSpeed = movement.maxSprintSpeed;
				movement.maxGroundAcceleration = movement.sprintGroundAcceleration;
				movement.maxAirAcceleration = movement.defaultAirAcceleration;
				movement.maxBackwardsSpeed = movement.defaultBackwardsSpeed;
				movement.maxSidewaysSpeed = movement.sprintSidewaysSpeed;
			}
		}
	}
}