using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class AmmoPerClip : MonoBehaviour
	{
		public int val;
		private int cache;
		private bool applied = false;

		public void Apply(GunScript gscript)
		{
			cache = val - gscript.ammoPerClip;
			gscript.ammoPerClip += cache;
			if (gscript.ammoLeft > gscript.ammoPerClip)
				gscript.ammoLeft = gscript.ammoPerClip;
		}

		public void Remove(GunScript gscript)
		{
			gscript.ammoPerClip -= cache;
			if (gscript.ammoLeft > gscript.ammoPerClip)
				gscript.ammoLeft = gscript.ammoPerClip;
		}

		///when decreasing clip size add to clip reserve
	}
}