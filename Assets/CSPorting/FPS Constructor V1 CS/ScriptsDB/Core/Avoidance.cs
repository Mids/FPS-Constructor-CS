using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	public class Avoidance : MonoBehaviour
	{

		public Vector3 avoidRotation;
		public Vector3 avoidPosition;
		public float avoidStartDistance = 4;
		public float avoidMaxDistance = 1.3f;
		private Vector3 rot;
		private Vector3 pos;
		private float dist = 2;
		private float minDist = 1.5f;

		public LayerMask layerMask = ~(1 << PlayerWeapons.playerLayer + 1 << PlayerWeapons.ignorePlayerLayer);
		private Vector3 targetRot;
		private Vector3 targetPos;
		public static bool collided = false;
		public static bool canAim = true;
		public static Avoidance singleton;
		public bool avoid = true;
		private bool startAvoid;

		public float stopTime = 0;

		void Awake()
		{
			singleton = this;
			rot = avoidRotation;
			pos = avoidPosition;
			dist = avoidStartDistance;
			minDist = avoidMaxDistance;
			startAvoid = avoid;
		}

		//Sets values to given.
		public static void SetValues(Vector3 rotation, Vector3 position, float startDist, float maxDist, bool avoids)
		{
			if (!singleton) return;
			singleton.rot = rotation;
			singleton.pos = position;
			singleton.dist = startDist;
			singleton.minDist = maxDist;
			singleton.avoid = avoids;
		}

		//Reverts to default values
		public static void SetValues()
		{
			if (!singleton) return;
			singleton.rot = singleton.avoidRotation;
			singleton.pos = singleton.avoidPosition;
			singleton.dist = singleton.avoidStartDistance;
			singleton.minDist = singleton.avoidMaxDistance;
			singleton.avoid = singleton.startAvoid;
		}

		void Update()
		{
			if (!avoid)
			{
				collided = false;
				return;
			}
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
			Ray ray2 = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2 + Screen.width / 65, Screen.height / 2, 0));
			Ray ray3 = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2 - Screen.width / 65, Screen.height / 2, 0));

			if (Physics.Raycast(ray, out hit, dist, layerMask) && !GunScript.reloading && !GunScript.takingOut && !GunScript.puttingAway)
			{
				Collide(hit);
			}
			else if (stopTime < 0)
			{
				stopTime = Time.time + .06f;
			}/* else if(Physics.Raycast(ray2, hit, dist, layerMask) && collided && !GunScript.reloading && !GunScript.takingOut && !GunScript.puttingAway){
		Collide(hit);
	} else if(Physics.Raycast(ray3, hit, dist, layerMask) && collided && !GunScript.reloading && !GunScript.takingOut && !GunScript.puttingAway){
		Collide(hit);
	} else {
	*/
			if (Time.time > stopTime && stopTime > 0)
			{
				targetRot = new Vector3(0, 0, 0);
				targetPos = new Vector3(0, 0, 0);
				canAim = true;
				if (transform.localPosition.magnitude < .3)
					collided = false;
			}
			float rate = Time.deltaTime * 9;

			if (transform.localPosition != targetPos)
				transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, rate);

			if (transform.localEulerAngles != targetRot)
			{
				transform.localEulerAngles = new Vector3(
					Mathf.LerpAngle(transform.localEulerAngles.x, targetRot.x, rate),
					Mathf.LerpAngle(transform.localEulerAngles.y, targetRot.y, rate),
					Mathf.LerpAngle(transform.localEulerAngles.z, targetRot.z, rate)
					);
			}
		}

		public void Collide(RaycastHit hit)
		{
			stopTime = -1;
			float val;

			val = ((dist - minDist) - (hit.distance - minDist)) / (dist - minDist);
			val = Mathf.Min(val, 1);
			targetRot = rot * val;
			targetPos = pos * val;
			collided = true;
			canAim = false;
		}
	}
}