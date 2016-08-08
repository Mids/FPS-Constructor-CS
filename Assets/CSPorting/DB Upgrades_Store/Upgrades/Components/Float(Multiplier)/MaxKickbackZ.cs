using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class MaxKickbackZ : MonoBehaviour
	{
		public float multiplier = 1.5f;
		private float cache;
		private bool applied = false;

		public void Apply(GunScript gScript)
		{
			cache = gScript.maxZ * (multiplier - 1);
			gScript.maxZ += cache;
		}

		public void Remove(GunScript gScript)
		{
			gScript.maxZ -= cache;
		}
	}
}