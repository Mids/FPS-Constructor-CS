using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class Force : MonoBehaviour
	{
		public float multiplier = 1.5f;
		private float cache;
		private bool applied = false;

		public void Apply(GunScript gScript)
		{
			cache = gScript.force * (multiplier - 1);
			gScript.force += cache;
		}

		public void Remove(GunScript gScript)
		{
			gScript.force -= cache;
		}
	}
}