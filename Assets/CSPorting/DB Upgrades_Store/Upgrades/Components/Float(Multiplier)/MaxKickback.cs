using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class MaxKickback : MonoBehaviour
	{
		public float multiplier = 1.5f;
		private float cache;
		private bool applied = false;

		public void Apply(GunScript gScript)
		{
			cache = gScript.maxKickback * (multiplier - 1);
			gScript.maxKickback += cache;
		}

		public void Remove(GunScript gScript)
		{
			gScript.maxKickback -= cache;
		}
	}
}