using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class SecondaryFireAnim : MonoBehaviour
	{
		public string val;
		private string cache;
		private bool applied = false;

		public void Apply(GunScript gScript)
		{
			cache = gScript.GetComponentInChildren<GunChildAnimation>().secondaryFireAnim;
			gScript.GetComponentInChildren<GunChildAnimation>().secondaryFireAnim = val;
		}

		public void Remove(GunScript gScript)
		{
			gScript.GetComponentInChildren<GunChildAnimation>().secondaryFireAnim = cache;
		}
	}
}