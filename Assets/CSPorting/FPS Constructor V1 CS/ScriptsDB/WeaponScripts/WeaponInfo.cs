using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class WeaponInfo : MonoBehaviour
	{
		// You can change the WeaponClasses enum to define your own Weapon Classes, 
		// "Null" must be the last value in the enum and should be applied to one empty weapon object.
		// The store will replace any underscores with a space in the display name. Sniper_Rifle will be displayed as "Sniper Rifle"

		public GameObject drops;

		public enum weaponClasses
		{
			Sidearm,
			Primary,
			Special,
			Null
		};

		public bool owned = false;
		public bool locked = false;
		public weaponClasses weaponClass;
		[HideInInspector]
		public string weaponClassName;
		public string gunDescription;
		public string lockedDescription = "Weapon Locked";
		public string gunName;
		public int buyPrice;
		public int ammoPrice;
		public float sellPrice;
		[HideInInspector]
		public float sellPriceUpgraded;
		public Texture icon; //Icon should be X by Y pixels for store.
		[HideInInspector]
		public bool[] upgradesApplied;

		[HideInInspector]
		public Upgrade[] upgrades;

		private Upgrade[] storeUpgrades;
		public bool canBeSold = true; //Can this weapon be sold (it's often best to have one base weapon which cannot)

		[HideInInspector]
		public GunScript gun;

		private GunScript[] guns;

		public void Awake()
		{
			gun = getPrimaryGunscript();
			upgrades = GetComponentsInChildren<Upgrade>();
			ArrayList tempArr = new ArrayList();
			upgradesApplied = new bool[upgrades.Length];
			// Initialize array of applied;
			for (int i = 0; i < upgrades.Length; i++)
			{
				upgradesApplied[i] = upgrades[i].applied;
				if (upgrades[i].showInStore)
				{
					tempArr.Add(upgrades[i]);
				}
			}
			storeUpgrades = tempArr.ToArray() as Upgrade[];
			// Create Display string for gunClass 
			weaponClassName = weaponClass.ToString().Replace("_", " ");
		}

		public GunScript getPrimaryGunscript()
		{
			guns = GetComponents<GunScript>();
			for (int i = 0; i < guns.Length; i++)
			{
				if (guns[i].isPrimaryWeapon)
					return guns[i];
			}
			return null;
		}

		public Upgrade[] getUpgrades()
		{
			return storeUpgrades;
		}

		public bool[] getUpgradesApplied()
		{
			return upgradesApplied;
		}

		public void ApplyUpgrade()
		{
			float tmpPrice;
			tmpPrice = sellPrice;
			for (int i = 0; i < upgrades.Length; i++)
			{
				if (upgrades[i].owned)
				{
					tmpPrice += upgrades[i].sellPrice;
				}
			}
			sellPriceUpgraded = tmpPrice;
		}

		public void updateApplied()
		{
			for (int i = 0; i < upgrades.Length; i++)
			{
				upgradesApplied[i] = upgrades[i].applied;
			}
		}
	}
}