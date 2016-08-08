using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class EnemyMovement : MonoBehaviour
	{
		private Transform target;
		public Transform waypoint;
		private Quaternion targetRotation;
		private Vector3 targetVector;
		private bool move = false;
		private Vector3 playerLastSeen;
		private bool visitedLastSeen = true;
		private int targetPriority = 0;

		private Vector3 curTarget;
		private float loseTime = 0;

		public float turnSpeed;
		public float attackRange;
		public float targetBuffer;
		public float desiredSpeed;
		public float forceConstant;
		public float viewAngle;
		public float viewRange;
		public float hearRange;
		public LayerMask blocksVision;
		public bool moves = true;
		private bool sees;

		private bool hitOverride = false;

		public static int enemies = 0;

		public void Start()
		{
			target = PlayerWeapons.weaponCam.transform;
			this.GetComponent<EnemyDamageReceiver>().isEnemy = true;
		}

		public void Update()
		{
			if (MouseLookDBJS.freeze)
				return;
			sees = CanSeeTarget();
			if (hitOverride)
			{
				playerLastSeen = target.position;
				visitedLastSeen = false;
			}
			hitOverride = false;
			Vector3 relativePos;
			if (sees)
			{
				curTarget = target.position;
			}
			else if (!visitedLastSeen)
			{
				curTarget = playerLastSeen;
				loseTime += Time.deltaTime;
				if (loseTime > 3)
				{
					visitedLastSeen = true;
					loseTime = 0;
					waypoint = Waypoint.GetClosestWaypoint(transform.position);
				}
			}
			else
			{
				if (waypoint)
				{
					curTarget = waypoint.position;
				}
				else
				{
					curTarget = Vector3.zero;
				}
			}

			relativePos = curTarget - transform.position;
			targetRotation = Quaternion.LookRotation(relativePos);
			ToRotation(targetRotation.eulerAngles);

			if (move && moves)
			{
				// this reduces the amount of force that acts on the object if it is already
				// moving at speed.
				float forceMultiplier = Mathf.Clamp01((desiredSpeed - GetComponent<Rigidbody>().velocity.magnitude) / desiredSpeed);
				// now we actually perform the push
				GetComponent<Rigidbody>().AddForce(transform.forward * (forceMultiplier * Time.deltaTime * forceConstant));
			}

			if (Vector3.Distance(transform.position, target.position) < attackRange && sees)
				this.SendMessage("Attack");

			if (Vector3.Distance(transform.position, curTarget) < targetBuffer)
			{
				visitedLastSeen = true;
				move = false;
			}
			else
			{
				move = true;
			}
		}

		public void ToRotation(Vector3 v3)
		{
			float xtarget = transform.localEulerAngles.x;
			float ztarget = transform.localEulerAngles.z;
			float ytarget = Mathf.MoveTowardsAngle(transform.localEulerAngles.y, v3.y, Time.deltaTime * turnSpeed);
			transform.localEulerAngles = new Vector3(xtarget, ytarget, ztarget);
		}

		public bool CanSeeTarget()
		{
			//checks if target is visible, within field of view, or close enough to be heard

			bool canSee = false;
			RaycastHit hit;

			float targetAngle = Vector3.Angle(target.position - transform.position, transform.forward);
			float targetDistance = Vector3.Distance(transform.position, target.position);
			//is target within range and angle
			if (targetDistance < viewRange && Mathf.Abs(targetAngle) < viewAngle / 2)
			{
				if (!Physics.Linecast(transform.position, (target.position), blocksVision))
				{
					playerLastSeen = target.position;
					canSee = true;
					visitedLastSeen = false;
				}
			}
			//is target close enough to hear?
			if (targetDistance < hearRange)
			{
				playerLastSeen = target.position;
				canSee = true;
				visitedLastSeen = false;
			}
			return canSee;
		}

		public void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;

			//Draw field of view
			Quaternion leftRayRotation = Quaternion.AngleAxis(-viewAngle / 2, Vector3.up);
			Quaternion rightRayRotation = Quaternion.AngleAxis(viewAngle / 2, Vector3.up);

			Vector3 leftRayDirection = leftRayRotation * transform.forward;
			Vector3 rightRayDirection = rightRayRotation * transform.forward;

			Gizmos.DrawRay(transform.position, leftRayDirection * viewRange);
			Gizmos.DrawRay(transform.position, rightRayDirection * viewRange);

			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, attackRange);

			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(transform.position, hearRange);
		}

		public void ApplyDamagePlayer(float damage)
		{
			hitOverride = true;
		}
	}
}