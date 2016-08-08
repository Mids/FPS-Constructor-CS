using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class AimSpread : MonoBehaviour
	{
		public float multiplier = 1.5f;
		private float cache;
		private bool applied = false;

		public void Apply(GunScript gScript)
		{
			cache = gScript.aimSpread * (multiplier - 1);
			gScript.aimSpread += cache;
		}

		public void Remove(GunScript gScript)
		{
			gScript.aimSpread -= cache;
		}
	}
}