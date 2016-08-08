using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class AutoFire : MonoBehaviour
	{
		public bool val;
		private bool cache;
		private bool applied = false;

		public void Apply(GunScript gScript)
		{
			cache = gScript.autoFire;
			gScript.autoFire = val;
		}

		public void Remove(GunScript gScript)
		{
			gScript.autoFire = cache;
		}
	}
}