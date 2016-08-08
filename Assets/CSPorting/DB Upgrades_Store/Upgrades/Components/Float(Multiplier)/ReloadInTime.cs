using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class ReloadInTime : MonoBehaviour
	{
		public float multiplier = 1.5f;
		private float cache;
		private bool applied = false;

		public void Apply(GunScript gScript)
		{
			cache = gScript.reloadInTime * (multiplier - 1);
			gScript.reloadInTime += cache;
		}

		public void Remove(GunScript gScript)
		{
			gScript.reloadInTime -= cache;
		}
	}
}