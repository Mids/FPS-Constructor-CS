using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class SpreadDecreaseRate : MonoBehaviour 
	{
		public float multiplier = 1.5f;
		private float cache;
		private bool applied = false;

		public void Apply(GunScript gScript)
		{
			cache = gScript.spDecRate * (multiplier - 1);
			gScript.spDecRate += cache;
		}

		public void Remove(GunScript gScript)
		{
			gScript.spDecRate -= cache;
		}
	}
}
