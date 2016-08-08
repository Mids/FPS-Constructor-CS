using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class CamSway : MonoBehaviour
	{
		public Vector2 moveSwayRate;
		public Vector2 moveSwayAmplitude;
		public Vector2 runSwayRate;
		public Vector2 runSwayAmplitude;
		public float swayStartTime = 0;
		public Vector2 idleSwayRate;
		public Vector2 idleAmplitude;

		private Vector3 val;
		private Vector3 lastVal;
		private Vector2 swayRate;
		private Vector2 swayAmplitude;
		public static CamSway singleton;
		public Vector3 curJostle;
		public Vector3 lastJostle;
		public static Vector3 jostleAmt;

		void Awake()
		{
			singleton = this;
		}

		public void WalkSway()
		{
			if (swayStartTime > Time.time)
				swayStartTime = Time.time;
			CharacterMotorDB CM = PlayerWeapons.CM;
			int speed = (int) CM.GetComponent<CharacterController>().velocity.magnitude;

			//Jostle
			lastJostle = curJostle;
			curJostle = Vector3.Lerp(curJostle, jostleAmt, Time.deltaTime * 16);
			jostleAmt = Vector3.Lerp(jostleAmt, Vector3.zero, Time.deltaTime * 4);
			transform.localPosition += (curJostle - lastJostle) * 15;

			if (speed < .2)
			{
				ResetPosition();
				return;
			}
			//sine function for motion
			float t = Time.time - swayStartTime;
			Vector3 curVect;
			swayAmplitude = moveSwayAmplitude;
			if (CharacterMotorDB.crouching)
			{
				swayRate = moveSwayRate * CM.movement.maxCrouchSpeed / CM.movement.defaultForwardSpeed;
			}
			else if (CharacterMotorDB.prone)
			{
				swayRate = moveSwayRate * CM.movement.maxProneSpeed / CM.movement.defaultForwardSpeed;
			}
			else if (AimMode.sprintingPublic)
			{
				swayRate = runSwayRate;
				swayAmplitude = runSwayAmplitude;
			}
			else
			{
				swayRate = moveSwayRate;
			}
			curVect.x = swayAmplitude.x * Mathf.Sin(swayRate.x * t); //*Mathf.Sin(swayRate.x*speed/14*t);
			curVect.y = Mathf.Abs(swayAmplitude.y * Mathf.Sin(swayRate.y * t));

			curVect.x -= swayAmplitude.x / 2;
			curVect.y -= swayAmplitude.y / 2;

			Vector3 eulerAngles = transform.localEulerAngles;

			//Move
			lastVal = val;
			val.x = Mathf.Lerp(val.x, curVect.x, Time.deltaTime * swayRate.x);
			eulerAngles.z = Mathf.LerpAngle(eulerAngles.z, -curVect.x * .5f, Time.deltaTime * swayRate.x);

			val.y = Mathf.Lerp(val.y, curVect.y, Time.deltaTime * swayRate.y);
			eulerAngles.x = Mathf.LerpAngle(eulerAngles.x, -curVect.y * .5f, Time.deltaTime * swayRate.y);
			//transform.localPosition.x = Vector3.Lerp(transform.localPosition.x, curVect.x, Time.deltaTime*swayRate.x);

			transform.localEulerAngles = eulerAngles;

			Vector3 localPos = transform.localPosition;

			localPos.x += val.x - lastVal.x;
			localPos.y += val.y - lastVal.y;

			transform.localPosition = localPos;
		}

		public void ResetPosition()
		{
			swayStartTime = 9999999999999;
			if (transform.localPosition == Vector3.zero)
			{
				return;
			}

			Vector3 eulerAngles = transform.localEulerAngles;
			Vector3 localPos = transform.localPosition;

			//Move
			lastVal = val;

			val.x = Mathf.Lerp(val.x, 0, Time.deltaTime * idleSwayRate.x);
			eulerAngles.z = Mathf.LerpAngle(eulerAngles.z, 0, Time.deltaTime * idleSwayRate.x);

			val.y = Mathf.Lerp(val.y, 0, Time.deltaTime * idleSwayRate.y);
			eulerAngles.x = Mathf.LerpAngle(eulerAngles.x, 0, Time.deltaTime * idleSwayRate.y);

			localPos.x += val.x - lastVal.x;
			localPos.y += val.y - lastVal.y;

			transform.localEulerAngles = eulerAngles;
			transform.localPosition = localPos;
		}

		void Update()
		{
			if (!AimMode.staticAiming && PlayerWeapons.CM.grounded && !CharacterMotorDB.paused)
			{
// && CM.walking){
				WalkSway();
			}
			else
			{
				ResetPosition();
			}
		}
	}
}