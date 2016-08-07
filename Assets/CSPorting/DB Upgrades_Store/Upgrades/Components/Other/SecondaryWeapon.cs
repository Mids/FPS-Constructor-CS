using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class SecondaryWeapon : MonoBehaviour
	{
		private GunScript gscript;
		public int s;
		private GunScript script;
		private GunScript cache;
		private bool applied = false;

		public void Start()
		{
			GunScript[] gscripts = this.transform.parent.GetComponents<GunScript>();
			for (int q = 0; q < gscripts.Length; q++)
			{
				if (gscripts[q] != null && gscripts[q].isPrimaryWeapon)
				{
					gscript = gscripts[q];
				}
				if (q == s)
				{
					script = gscripts[q];
				}
			}
			cache = gscript.secondaryWeapon;
		}

		public void Apply()
		{
			gscript.secondaryWeapon = script;
		}

		public void Remove()
		{
			gscript.secondaryWeapon = cache;
		}
	}
}