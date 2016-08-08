using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class CubeSet : MonoBehaviour
	{
		public Transform[] cubes;
		public int[] amts;

		public IEnumerator SpawnCS(Transform pos, Waypoint w, float t)
		{
			Transform spawned;
			for (int j = 0; j < cubes.Length; j++)
			{
				for (int q = 0; q < amts[j]; q++)
				{
					spawned = (Transform) Instantiate(cubes[j], pos.position + new Vector3(0, 4, 0) * q, pos.rotation);
					EnemyMovement.enemies++;
					spawned.GetComponent<EnemyMovement>().waypoint = w.transform;
					yield return new WaitForSeconds(t);
				}
			}
		}
	}
}