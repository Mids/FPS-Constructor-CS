using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class KickbackAngle : MonoBehaviour
	{
		public float multiplier = 1.5f;
		private float cache;
		private bool applied = false;

		public void Apply(GunScript gScript)
		{
			cache = gScript.kickbackAngle * (multiplier - 1);
			gScript.kickbackAngle += cache;
		}

		public void Remove(GunScript gScript)
		{
			gScript.kickbackAngle -= cache;
		}
	}
}