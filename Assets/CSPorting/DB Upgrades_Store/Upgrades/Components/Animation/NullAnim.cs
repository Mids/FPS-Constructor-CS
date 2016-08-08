using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class NullAnim : MonoBehaviour 
	{
		public string val;
		private string cache;
		private bool applied = false;

		public void Apply(GunScript gScript)
		{
			cache = gScript.GetComponentInChildren<GunChildAnimation>().nullAnim;
			gScript.GetComponentInChildren<GunChildAnimation>().nullAnim = val;
		}

		public void Remove(GunScript gScript)
		{
			gScript.GetComponentInChildren<GunChildAnimation>().nullAnim = cache;
		}
	}
}
