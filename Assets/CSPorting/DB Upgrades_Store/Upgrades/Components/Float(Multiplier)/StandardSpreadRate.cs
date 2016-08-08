using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class StandardSpreadRate : MonoBehaviour
	{
		public float multiplier = 1.5f;
		private float cache;
		private bool applied = false;

		public void Apply(GunScript gScript)
		{
			cache = gScript.standardSpreadRate * (multiplier - 1);
			gScript.standardSpreadRate += cache;
		}

		public void Remove(GunScript gScript)
		{
			gScript.standardSpreadRate -= cache;
		}
	}
}