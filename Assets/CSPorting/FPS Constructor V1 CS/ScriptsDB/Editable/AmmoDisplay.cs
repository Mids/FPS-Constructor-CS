using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class AmmoDisplay : MonoBehaviour
	{
		[HideInInspector] public int bulletsLeft;
		[HideInInspector] public int clips;
		[HideInInspector] public bool display = true; //used by system
		public bool show = true; //used by user
		[HideInInspector] public string clipDisplay;
		private GunScript[] gunScripts;
		private GunScript gunScriptSecondary;
		private GunScript gunScript;

		void Start()
		{
			//This is AmmoDisplay getting all of the GunScripts from this weapon, then saving the primary and secondary.
			gunScripts = this.GetComponents<GunScript>();
			GunScript g;
			for (int i = 0; i < gunScripts.Length; i++)
			{
				g = gunScripts[i] as GunScript;
				if (g.isPrimaryWeapon)
				{
					gunScript = g;
				}
			}
			for (int q = 0; q < gunScripts.Length; q++)
			{
				g = gunScripts[q] as GunScript;
				if (!g.isPrimaryWeapon)
				{
					if (gunScript.secondaryWeapon == g)
						gunScriptSecondary = g;
				}
			}
		}

		public void reapply()
		{
			//This is AmmoDisplay getting all of the GunScripts from this weapon, then saving the primary and secondary.
			gunScripts = this.GetComponents<GunScript>();
			GunScript g;
			for (int i = 0; i < gunScripts.Length; i++)
			{
				g = gunScripts[i] as GunScript;
				if (g.isPrimaryWeapon)
				{
					gunScript = g;
				}
			}
			for (int q = 0; q < gunScripts.Length; q++)
			{
				g = gunScripts[q] as GunScript;
				if (!g.isPrimaryWeapon)
				{
					if (gunScript.secondaryWeapon == g)
						gunScriptSecondary = g;
				}
			}
		}

		void OnGUI()
		{
			if (!(display && show))
				return;
			//Decide whether or not to show clips depending on if the guns have infinite ammo
			//This will have to be modified if you change the display
			string clipDisplay;
			string clipDisplay2;

			if (!gunScript.infiniteAmmo)
			{
				clipDisplay = "/" + gunScript.clips;
			}
			else
			{
				clipDisplay = "";
			}

			if (gunScriptSecondary != null && !gunScriptSecondary.infiniteAmmo)
			{
				clipDisplay2 = "/" + gunScriptSecondary.clips;
			}
			else
			{
				clipDisplay2 = "";
			}

			GUI.skin.box.fontSize = 25;
			//This is where you'll want to edit to make your own ammo display
			if (gunScriptSecondary != null)
			{
				//If there is a secondary weapon, display it's ammo along with the main weapon's
				GUI.Box(new Rect(Screen.width - 110, Screen.height - 55, 100, 20), "Ammo: " + Mathf.Round(gunScript.ammoLeft) + clipDisplay);
				GUI.Box(new Rect(Screen.width - 80, Screen.height - 30, 70, 20), "Alt: " + Mathf.Round(gunScriptSecondary.ammoLeft) + clipDisplay2);
			}
			else
			{
				//Otherwise just display the main weapon's ammo
				GUI.Box(new Rect(Screen.width - 210, Screen.height - 50, 200, 40), "Ammo: " + Mathf.Round(gunScript.ammoLeft) + clipDisplay);
			}
		}

		public void SelectWeapon()
		{
			display = true;
		}

		public void DeselectWeapon()
		{
			display = false;
		}
	}
}