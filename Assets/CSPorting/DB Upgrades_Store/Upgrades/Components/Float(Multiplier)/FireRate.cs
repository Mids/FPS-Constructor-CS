using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class FireRate : MonoBehaviour
	{
		public float multiplier = 1.5f;
		private float cache;
		private bool applied = false;

		public void Apply(GunScript gScript)
		{
			cache = gScript.fireRate * (multiplier - 1);
			gScript.fireRate += cache;
		}

		public void Remove(GunScript gScript)
		{
			gScript.fireRate -= cache;
		}
	}
}