using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class SecondaryReloadAnim : MonoBehaviour 
	{
		public string val;
		private string cache;
		private bool applied = false;

		public void Apply(GunScript gScript)
		{
			cache = gScript.GetComponentInChildren<GunChildAnimation>().secondaryReloadAnim;
			gScript.GetComponentInChildren<GunChildAnimation>().secondaryReloadAnim = val;
		}

		public void Remove(GunScript gScript)
		{
			gScript.GetComponentInChildren<GunChildAnimation>().secondaryReloadAnim = cache;
		}
	}
}
