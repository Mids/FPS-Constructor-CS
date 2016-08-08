using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class GunLook : MonoBehaviour
	{
		private float sensitivityX = 15F;
		private float sensitivityY = 15F;
		public float sensitivityStandardX = 15F;
		public float sensitivityStandardZ = 15F;
		public float sensitivityStandardY = 15F;
		public float sensitivityAimingX = 15F;
		public float sensitivityAimingZ = 15F;
		public float sensitivityAimingY = 15F;
		public float retSensitivity = -.5F;

		[HideInInspector] public float minimumX = 5F;
		[HideInInspector] public float maximumX = 3F;

		public float xRange = 5F;
		public float xRangeAim = 3F;

		public float zRange = 5F;
		public float zRangeAim = 3F;

		private float actualZRange;
		private float sensitivityZ;

		public float yRange = 5F;
		public float yRangeAim = 3F;

		public float zMoveRange = 10;
		public float zMoveSensitivity = .5f;
		public float zMoveAdjustSpeed = 4;

		public float xMoveRange = 10;
		public float xMoveSensitivity = .5f;
		public float xMoveAdjustSpeed = 4;

		public float xAirMoveRange = 10;
		public float xAirMoveSensitivity = .5f;
		public float xAirAdjustSpeed = 4;

		public float zPosMoveRange = .13f;
		public float zPosMoveSensitivity = .5f;
		public float zPosAdjustSpeed = 4;

		public float xPosMoveRange = .13f;
		public float xPosMoveSensitivity = .5f;
		public float xPosAdjustSpeed = 4;

		private float minimumY = -60F;
		private float maximumY = 60F;

		//added by dw to pause camera when in store
		[HideInInspector] public bool freeze = false;
		[HideInInspector] public float rotationX = 0F;
		[HideInInspector] public float rotationY = 0F;
		public float rotationZ = 0F;

		private Vector3 startPos;
		private Vector3 lastOffset;
		private Vector3 posOffset;

		private float curZ;
		private float curX;
		private float curX2;
		private float lastZ;
		private float lastX;
		private float tX;

		public bool useLookMotion = true;
		public bool useWalkMotion = true;

		public bool lookMotionOpen = false;
		public bool walkMotionOpen = false;

		private Quaternion originalRotation;

		public static Vector3 jostleAmt;
		public Vector3 curJostle;
		public Vector3 lastJostle;

		private Vector3 targetPosition;
		private Vector3 curTarget;
		private Vector3 lastTarget;

		public void Freeze()
		{
			freeze = true;
		}

		public void UnFreeze()
		{
			freeze = false;
		}

		void Update()
		{
			if (freeze || !PlayerWeapons.playerActive) return;

			if (retSensitivity > 0)
				retSensitivity *= -1;

			if (useLookMotion && PlayerWeapons.canLook)
			{
				Quaternion xQuaternion;
				Quaternion yQuaternion;

				// Read the mouse input axis
				rotationX += InputDB.GetAxis("Mouse X") * sensitivityX;
				rotationY += InputDB.GetAxis("Mouse Y") * sensitivityY;
				rotationZ += InputDB.GetAxis("Mouse X") * sensitivityZ;

				rotationX = ClampAngle(rotationX, minimumX, maximumX);
				rotationY = ClampAngle(rotationY, minimumY, maximumY);
				rotationZ = ClampAngle(rotationZ, -actualZRange, actualZRange);
				if (Mathf.Abs(Input.GetAxis("Mouse X")) < .05)
				{
					if (sensitivityX > 0)
					{
						rotationX -= rotationX * Time.deltaTime * retSensitivity * 7;
						rotationZ -= rotationZ * Time.deltaTime * retSensitivity * 7;
						rotationY += rotationY * Time.deltaTime * retSensitivity * 7;
					}
					else
					{
						rotationX += rotationX * Time.deltaTime * retSensitivity * 7;
						rotationZ += rotationZ * Time.deltaTime * retSensitivity * 7;
						rotationY += rotationY * Time.deltaTime * retSensitivity * 7;
					}
				}

				xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
				Quaternion zQuaternion = Quaternion.AngleAxis(rotationZ, Vector3.forward);
				yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);

				transform.localRotation = Quaternion.Lerp(transform.localRotation, originalRotation * xQuaternion * yQuaternion * zQuaternion, Time.deltaTime * 10);
			}

			if (useWalkMotion)
			{
				//Velocity-based changes
				Vector3 relVelocity = transform.InverseTransformDirection(PlayerWeapons.CM.movement.velocity);
				float zVal;
				float xVal;
				float xVal2;

				lastOffset = posOffset;

				float s = new Vector3(PlayerWeapons.CM.movement.velocity.x, 0, PlayerWeapons.CM.movement.velocity.z).magnitude / 14;


				if (!AimMode.staticAiming)
				{
					float xPos = Mathf.Clamp(relVelocity.x * xPosMoveSensitivity, -xPosMoveRange * s, xPosMoveRange * s);
					posOffset.x = Mathf.Lerp(posOffset.x, xPos, Time.deltaTime * xPosAdjustSpeed); // + startPos.x;

					float zPos = Mathf.Clamp(relVelocity.z * zPosMoveSensitivity, -zPosMoveRange * s, zPosMoveRange * s);
					posOffset.z = Mathf.Lerp(posOffset.z, zPos, Time.deltaTime * zPosAdjustSpeed); // + startPos.z;
				}
				else
				{
					posOffset.x = Mathf.Lerp(posOffset.x, 0, Time.deltaTime * xPosAdjustSpeed * 3); // + startPos.x;
					posOffset.z = Mathf.Lerp(posOffset.z, 0, Time.deltaTime * zPosAdjustSpeed * 3); // + startPos.z;
				}

				//Apply Jostle
				lastJostle = curJostle;
				curJostle = Vector3.Lerp(curJostle, jostleAmt, Time.deltaTime * 10);
				jostleAmt = Vector3.Lerp(jostleAmt, Vector3.zero, Time.deltaTime * 3);

				lastTarget = curTarget;
				curTarget = Vector3.Lerp(curTarget, posOffset, Time.deltaTime * 8);

				transform.localPosition += curTarget - lastTarget;
				transform.localPosition += curJostle - lastJostle;
			}
		}

		void Start()
		{
			// Make the rigid body not change rotation
			if (GetComponent<Rigidbody>())
				GetComponent<Rigidbody>().freezeRotation = true;
			originalRotation = transform.localRotation;
			startPos = transform.localPosition;
			StopAiming();
		}

		public static float ClampAngle(float angle, float min, float max)
		{
			if (angle < -360F)
				angle += 360F;
			if (angle > 360F)
				angle -= 360F;
			return Mathf.Clamp(angle, min, max);
		}

		public void Aiming()
		{
			sensitivityX = sensitivityAimingX;
			sensitivityY = sensitivityAimingY;

			minimumX = -xRangeAim;
			maximumX = xRangeAim;
			minimumY = -yRangeAim;
			maximumY = yRangeAim;
			sensitivityZ = sensitivityAimingZ;
			actualZRange = zRangeAim;
		}

		public void StopAiming()
		{
			sensitivityX = sensitivityStandardX;
			sensitivityY = sensitivityStandardY;

			minimumX = -xRange;
			maximumX = xRange;
			minimumY = -yRange;
			maximumY = yRange;
			sensitivityZ = sensitivityStandardZ;
			actualZRange = zRange;
		}
	}
}