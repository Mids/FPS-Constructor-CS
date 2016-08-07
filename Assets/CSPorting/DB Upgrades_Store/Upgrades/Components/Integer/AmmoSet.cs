using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class AmmoSet : MonoBehaviour
	{
		public int val;
		private int cache;
		private bool applied = false;

		public void Apply(GunScript gscript)
		{
			gscript.ApplyToSharedAmmo();
			cache = gscript.ammoSetUsed;
			gscript.ammoSetUsed = val;
			gscript.AlignToSharedAmmo();
		}

		public void Remove(GunScript gscript)
		{
			gscript.ApplyToSharedAmmo();
			gscript.ammoSetUsed = cache;
			gscript.AlignToSharedAmmo();
		}
	}
}