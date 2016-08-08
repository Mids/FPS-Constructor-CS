using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	[RequireComponent(typeof (CharacterController))]
	public class FPSWalkerDB : MonoBehaviour
	{
		public float speed = 6.0f;
		public float aimSpeed = 2.0f;
		public float sprintSpeed = 10.0f;
		private bool canSprint = true;

		public float sprintJumpSpeed = 8.0f;
		public float normSpeed = 6.0f;
		public float crouchSpeed = 6.0f;
		public float crouchDeltaHeight = 1.0f;
		public float jumpSpeed = 8.0f;
		public float normJumpSpeed = 8.0f;
		public float gravity = 20.0f;
		private GameObject mainCamera;
		private GameObject weaponCamera;
		public static bool crouching = false;
		private bool stopCrouching = false;
		private float standardCamHeight;
		private float crouchingCamHeight;

		private Vector3 moveDirection = Vector3.zero;
		public static bool grounded = false;

		public static bool walking = false;
		public static float notWalkingTime = 0;
		private float stopWalkingTime;

		void Start()
		{
			speed = normSpeed;
			mainCamera = GameObject.FindWithTag("MainCamera");
			weaponCamera = GameObject.FindWithTag("WeaponCamera");
			crouching = false;
			standardCamHeight = weaponCamera.transform.localPosition.y;
			crouchingCamHeight = standardCamHeight - crouchDeltaHeight;
		}

		void Update()
		{
			if (!walking)
			{
				notWalkingTime = Time.time - stopWalkingTime;
			}
			else
			{
				notWalkingTime = 0;
			}

			Vector3 weaponCameraLocalPos = weaponCamera.transform.localPosition;

			if (weaponCameraLocalPos.y > standardCamHeight)
			{
				weaponCameraLocalPos.y = standardCamHeight;
			}
			else if (weaponCameraLocalPos.y < crouchingCamHeight)
			{
				weaponCameraLocalPos.y = crouchingCamHeight;
			}

			if (grounded)
			{
				if (InputDB.GetButtonDown("Crouch"))
				{
					if (crouching)
					{
						stopCrouching = true;
						NormalSpeed();
						return;
					}

					if (!crouching)
						Crouch();
				}
			}

			if (crouching)
			{
				if (weaponCameraLocalPos.y > crouchingCamHeight)
				{
					weaponCameraLocalPos.y = Mathf.Clamp(weaponCameraLocalPos.y - crouchDeltaHeight * Time.deltaTime * 8, crouchingCamHeight, standardCamHeight);
				}
			}
			else
			{
				if (weaponCameraLocalPos.y < standardCamHeight)
				{
					weaponCameraLocalPos.y = Mathf.Clamp(weaponCameraLocalPos.y + standardCamHeight * Time.deltaTime * 8, 0, standardCamHeight);
				}
			}
			weaponCamera.transform.localPosition = weaponCameraLocalPos;
		}

		void FixedUpdate()
		{
			if (grounded && PlayerWeapons.canMove)
			{
				// We are grounded, so recalculate movedirection directly from axes
				moveDirection = new Vector3(InputDB.GetAxis("Horizontal"), 0, InputDB.GetAxis("Vertical"));
				moveDirection = transform.TransformDirection(moveDirection);
				moveDirection *= speed;

				if (InputDB.GetButton("Jump"))
				{
					moveDirection.y = jumpSpeed;
					if (crouching)
					{
						stopCrouching = true;
						NormalSpeed();
					}
				}
			}

			// Apply gravity
			moveDirection.y -= gravity * Time.deltaTime;

			// Move the controller
			CharacterController controller = GetComponent<CharacterController>();
			CollisionFlags flags = controller.Move(moveDirection * Time.deltaTime);
			grounded = (flags & CollisionFlags.CollidedBelow) != 0;

			if ((Mathf.Abs(moveDirection.x) > 0) && grounded || (Mathf.Abs(moveDirection.z) > 0 && grounded))
			{
				if (!walking)
				{
					walking = true;
					BroadcastMessage("Walking", SendMessageOptions.DontRequireReceiver);
				}
			}
			else if (walking)
			{
				walking = false;
				stopWalkingTime = Time.time;
				BroadcastMessage("StopWalking", SendMessageOptions.DontRequireReceiver);
			}
		}

		public void Aiming()
		{
			speed = aimSpeed;
		}

		public void Crouch()
		{
			speed = crouchSpeed;
			this.GetComponent<CharacterController>().height -= crouchDeltaHeight;
			this.GetComponent<CharacterController>().center -= new Vector3(0, crouchDeltaHeight / 2, 0);
			crouching = true;
		}

		public void NormalSpeed()
		{
			if (stopCrouching)
			{
				crouching = false;
				this.GetComponent<CharacterController>().height += crouchDeltaHeight;
				BroadcastMessage("StopWalking", SendMessageOptions.DontRequireReceiver);
				this.GetComponent<CharacterController>().center += new Vector3(0, crouchDeltaHeight / 2, 0);

				stopCrouching = false;
			}
			else if (crouching)
			{
				speed = crouchSpeed;
				return;
			}
			speed = normSpeed;
			jumpSpeed = normJumpSpeed;
		}

		public void Sprinting()
		{
			if (crouching)
			{
				crouching = false;
				this.GetComponent<BoxCollider>().size += new Vector3(0, crouchDeltaHeight, 0);
				this.GetComponent<BoxCollider>().center += new Vector3(0, crouchDeltaHeight, 0);
			}
			if (canSprint)
			{
				speed = sprintSpeed;
				jumpSpeed = sprintJumpSpeed;
			}
		}
	}
}