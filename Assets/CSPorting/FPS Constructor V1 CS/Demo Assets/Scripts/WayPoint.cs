using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class WayPoint : MonoBehaviour
	{
		public Waypoint nextWaypoint;
		public static Waypoint[] Waypoints;

		public void Awake()
		{
			if (Waypoints == null)
			{
				Waypoints = GameObject.FindObjectsOfType(typeof (Waypoint)) as Waypoint[];
			}
		}

		public void OnTriggerEnter(Collider other)
		{
			EnemyMovement AI = other.transform.root.GetComponent<EnemyMovement>();

			if (AI != null)
			{
				AI.waypoint = nextWaypoint.transform;
			}
		}

		public static Transform GetClosestWaypoint(Vector3 pos)
		{
			float dist = 100000000;
			Transform closest = null;
			float temp;
			for (int i = 0; i < Waypoints.Length; i++)
			{
				if (Waypoints[i])
				{
					temp = (Waypoints[i].transform.position - pos).magnitude;
					if (temp < dist)
					{
						dist = temp;
						closest = Waypoints[i].transform;
					}
				}
			}
			if (closest == null)
			{
				Debug.LogError("closest is null");
			}
			return closest;
		}
	}
}