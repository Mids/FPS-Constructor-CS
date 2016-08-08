using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class Scoped : MonoBehaviour
	{
		private AimMode gscript;
		public bool val;
		private bool cache;
		private bool applied = false;

		public void Start()
		{
			gscript = this.transform.parent.GetComponent<GunScript>().GetComponentInChildren<AimMode>();
		}

		public void Apply()
		{
			cache = gscript.scoped1;
			gscript.scoped = val;
			gscript.scoped1 = val;
		}

		public void Remove()
		{
			gscript.scoped = cache;
			gscript.scoped1 = cache;
		}
	}
}