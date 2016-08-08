using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class Spawner : MonoBehaviour
	{
		public int curWave = 0;
		public Waypoint[] waypoints;
		public Wave[] waves;
		public Transform[] spawners;
		public float spawnDelay = 3;

		public float spawnTime = 0.2f;
		private bool spawning = false;
		private float nextSpawnTimme;


		public IEnumerator Spawn()
		{
			Wave w;
			CubeSet cs;

			while (curWave < waves.Length)
			{
				w = waves[curWave];
				for (int i = 0; i < w.cubeSets.Length; i++)
				{
					cs = w.cubeSets[i];
					cs.SpawnCS(spawners[i], waypoints[i], spawnTime);
				}
				while (EnemyMovement.enemies > 0)
				{
					yield return new WaitForFixedUpdate();
				}
				curWave++;
				yield return new WaitForSeconds(spawnDelay + 1 * curWave);
				if (curWave >= waves.Length)
					curWave = 0;
			}
		}

		public void OnTriggerEnter(Collider other)
		{
			//if(other.tag == "Player")
			if (!spawning)
			{
				spawning = true;
				Spawn();
			}
		}
	}
}