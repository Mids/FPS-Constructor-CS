using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class StandardSpread : MonoBehaviour
	{
		public float multiplier = 1.5f;
		private float cache;
		private bool applied = false;

		public void Apply(GunScript gScript)
		{
			cache = gScript.standardSpread * (multiplier - 1);
			gScript.standardSpread += cache;
		}

		public void Remove(GunScript gScript)
		{
			gScript.standardSpread -= cache;
		}
	}
}