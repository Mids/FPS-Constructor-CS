using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class AimSpreadRate : MonoBehaviour
	{
		public float multiplier = 1.5f;
		private float cache;
		private bool applied = false;

		public void Apply(GunScript gScript)
		{
			cache = gScript.aimSpreadRate * (multiplier - 1);
			gScript.aimSpreadRate += cache;
		}

		public void Remove(GunScript gScript)
		{
			gScript.aimSpreadRate -= cache;
		}
	}
}