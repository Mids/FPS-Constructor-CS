using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class Pitch : MonoBehaviour
	{
		public float multiplier = 1.5f;
		private float cache;
		private bool applied = false;

		public void Apply(GunScript gScript)
		{
			cache = gScript.firePitch * (multiplier - 1);
			gScript.firePitch += cache;
		}

		public void Remove(GunScript gScript)
		{
			gScript.firePitch -= cache;
		}
	}
}