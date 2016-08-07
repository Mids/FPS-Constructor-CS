using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class ShotCount : MonoBehaviour 
	{
		public int val;
		private int cache;
		private bool applied = false;

		public void Apply(GunScript gscript)
		{
			cache = val - gscript.shotCount;
			gscript.shotCount += cache;
		}
		public void Remove(GunScript gscript)
		{
			gscript.shotCount -= cache;
		}
	}
}
