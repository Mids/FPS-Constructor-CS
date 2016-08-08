using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class ProgressiveReload : MonoBehaviour 
	{
		public bool val;
		private bool cache;
		private bool applied = false;

		public void Apply(GunScript gScript)
		{
			cache = gScript.progressiveReload;
			gScript.progressiveReload = val;
		}

		public void Remove(GunScript gScript)
		{
			gScript.progressiveReload = cache;
		}
	}
}
