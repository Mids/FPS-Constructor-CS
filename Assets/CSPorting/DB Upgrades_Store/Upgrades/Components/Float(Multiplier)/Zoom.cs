using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class Zoom : MonoBehaviour
	{
		private AimMode gscript;
		public float multiplier = 1.5f;
		private float cache;
		private bool applied = false;

		public void Start()
		{
			gscript = this.transform.parent.GetComponent<GunScript>().GetComponentInChildren<AimMode>();
		}

		public void Apply()
		{
			cache = gscript.zoomFactor1 * (multiplier - 1);
			gscript.zoomFactor1 += cache;
			gscript.zoomFactor = gscript.zoomFactor1;
		}

		public void Remove()
		{
			gscript.zoomFactor1 -= cache;
			gscript.zoomFactor = gscript.zoomFactor1;
		}
	}
}