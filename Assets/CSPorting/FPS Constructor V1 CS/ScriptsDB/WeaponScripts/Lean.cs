using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	public class Lean : MonoBehaviour
	{

		public float leanAmount; //how far does the camera lean?
		public float leanRate; //how fast does the camera move when leaning? 

		public float leanRotate; //how much does the camera rotate when leaning
		private float rotateRate; //how fast do we rotate;

		private float startPos; //standard position of the camera
		private float targetPos; //current target position
		private float targetRot;
		private bool leaning = false; //are we currently leaning? 
		private bool left = false;
		private bool colliding = false;
		public LayerMask mask = ~(1 << PlayerWeapons.playerLayer + 1 << PlayerWeapons.ignorePlayerLayer);

		public float skinWidth = .2f;

		void Awake()
		{
			startPos = transform.localPosition.x;
			leaning = false;
			rotateRate = leanRate * (leanRotate / leanAmount);
		}

		void LateUpdate()
		{
			float maxLean = 0;
			if (InputDB.GetButton("LeanRight") && PlayerWeapons.CM.grounded && !CharacterMotorDB.walking)
			{
				if (!leaning || left)
				{
					leaning = true;
					targetPos = startPos + leanAmount;
					targetRot = -leanRotate;
					left = false;
					colliding = false;
				}
			}
			else if (InputDB.GetButton("LeanLeft") && PlayerWeapons.CM.grounded && !CharacterMotorDB.walking)
			{
				if (!leaning || !left)
				{
					leaning = true;
					targetPos = startPos - leanAmount;
					targetRot = leanRotate;
					left = true;
					colliding = false;
				}
			}
			else if (leaning)
			{
				colliding = false;
				leaning = false;
				targetPos = startPos;
			}

			if (left && leaning)
			{
				maxLean = Check(-1 * transform.right);
				targetPos = Mathf.Max(startPos - leanAmount, -maxLean + skinWidth);
			}
			else if (leaning)
			{
				maxLean = Check(transform.right);
				targetPos = Mathf.Min(startPos + leanAmount, maxLean - skinWidth);
			}

			Vector3 localpos = transform.localPosition;
			localpos.x = Mathf.Lerp(transform.localPosition.x, targetPos, Time.deltaTime * leanRate * 4);

			Vector3 localEuler = transform.localEulerAngles;
			localEuler.z = Mathf.LerpAngle(0, targetRot, Mathf.Abs(transform.localPosition.x) / leanAmount);

			//clamp our position if necessary
			if (colliding)
				localpos.x = Mathf.Clamp(transform.localPosition.x, -maxLean, maxLean);

			transform.localPosition = localpos;
			transform.localEulerAngles = localEuler;
		}

		public float Check(Vector3 dir)
		{
			RaycastHit hit;
			if (Physics.Raycast(transform.parent.position, dir, out hit, leanAmount, mask))
			{
				colliding = true;
				return hit.distance;
			}
			else
			{
				colliding = false;
				return leanAmount + skinWidth;
			}
		}
	}
}