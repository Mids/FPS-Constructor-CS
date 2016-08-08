using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class BurstTime : MonoBehaviour
	{
		public float multiplier = 1.5f;
		private float cache;
		private bool applied = false;

		public void Apply(GunScript gScript)
		{
			cache = gScript.burstTime * (multiplier - 1);
			gScript.burstTime += cache;
		}

		public void Remove(GunScript gScript)
		{
			gScript.burstTime -= cache;
			cache = 0;
		}
	}
}