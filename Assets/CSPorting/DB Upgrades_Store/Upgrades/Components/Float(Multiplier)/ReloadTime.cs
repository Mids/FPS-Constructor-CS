using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class ReloadTime : MonoBehaviour
	{
		public float multiplier = 1.5f;
		private float cache;
		private bool applied = false;

		public void Apply(GunScript gScript)
		{
			cache = gScript.reloadTime * (multiplier - 1);
			gScript.reloadTime += cache;
		}

		public void Remove(GunScript gScript)
		{
			gScript.reloadTime -= cache;
		}
	}
}