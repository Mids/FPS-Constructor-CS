using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class ReloadOutTime : MonoBehaviour
	{
		public float multiplier = 1.5f;
		private float cache;
		private bool applied = false;

		public void Apply(GunScript gScript)
		{
			cache = gScript.reloadOutTime * (multiplier - 1);
			gScript.reloadOutTime += cache;
		}

		public void Remove(GunScript gScript)
		{
			gScript.reloadOutTime -= cache;
		}
	}
}