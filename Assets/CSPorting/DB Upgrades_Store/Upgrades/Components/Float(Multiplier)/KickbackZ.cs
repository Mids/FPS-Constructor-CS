using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class KickbackZ : MonoBehaviour
	{
		public float multiplier = 1.5f;
		private float cache;
		private bool applied = false;

		public void Apply(GunScript gScript)
		{
			cache = gScript.kickBackZ * (multiplier - 1);
			gScript.kickBackZ += cache;
		}

		public void Remove(GunScript gScript)
		{
			gScript.kickBackZ -= cache;
		}
	}
}