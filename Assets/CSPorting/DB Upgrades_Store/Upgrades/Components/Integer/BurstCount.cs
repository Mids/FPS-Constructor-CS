using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class BurstCount : MonoBehaviour
	{
		public int val;
		private int cache;
		private bool applied = false;

		public void Apply(GunScript gscript)
		{
			cache = val - gscript.burstCount;
			gscript.burstCount += cache;
		}

		public void Remove(GunScript gscript)
		{
			gscript.burstCount -= cache;
		}
	}
}