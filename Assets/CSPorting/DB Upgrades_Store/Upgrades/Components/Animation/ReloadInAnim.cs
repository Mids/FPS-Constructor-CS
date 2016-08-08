using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class ReloadInAnim : MonoBehaviour 
	{
		public string val;
		private string cache;
		private bool applied = false;

		public void Apply(GunScript gScript)
		{
			cache = gScript.GetComponentInChildren<GunChildAnimation>().reloadIn;
			gScript.GetComponentInChildren<GunChildAnimation>().reloadIn = val;
		}

		public void Remove(GunScript gScript)
		{
			gScript.GetComponentInChildren<GunChildAnimation>().reloadIn = cache;
		}
	}
}
