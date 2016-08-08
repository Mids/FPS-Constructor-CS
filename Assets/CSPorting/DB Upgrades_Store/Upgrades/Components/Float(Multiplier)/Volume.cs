using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class Volume : MonoBehaviour
	{
		public float multiplier = 1.5f;
		private float cache;
		private bool applied = false;

		public void Apply(GunScript gScript)
		{
			cache = gScript.fireVolume * (multiplier - 1);
			gScript.fireVolume += cache;
		}

		public void Remove(GunScript gScript)
		{
			gScript.fireVolume -= cache;
		}
	}
}