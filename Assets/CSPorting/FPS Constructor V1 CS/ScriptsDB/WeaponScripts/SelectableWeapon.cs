using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	public class SelectableWeapon : MonoBehaviour
	{

		private bool selected = false;
		public GameObject weapon;
		[HideInInspector]
		public WeaponInfo WeaponInfo;
		public bool isEquipped = false;
		public int ammo = 0;
		//@HideInInspector
		public bool[] upgradesApplied;

		void Start()
		{
			WeaponInfo = weapon.GetComponent<WeaponInfo>();
		}

		public void Interact()
		{
			PickupWeapon.Pickup(this.gameObject);
		}

		public void select(bool a)
		{
			this.SendMessage("HighlightOn", SendMessageOptions.DontRequireReceiver);
			isEquipped = a;
		}


		//Apply any additional effects to the gunscript
		public void Apply(GunScript g)
		{
			g.ammoLeft = ammo;
			if (!PlayerWeapons.saveUpgradesToDrops)
				return;
			for (int i = 0; i < WeaponInfo.upgrades.Length; i++)
			{
				if (i >= upgradesApplied.Length)
				{
					WeaponInfo.upgrades[i].RemoveUpgradeInstant();
					continue;
				}
				if (upgradesApplied[i])
				{
					WeaponInfo.upgrades[i].ApplyUpgradeInstant();
				}
				else
				{
					WeaponInfo.upgrades[i].RemoveUpgradeInstant();
				}
			}
		}

		public void PopulateDrop()
		{
			WeaponInfo = weapon.GetComponent<WeaponInfo>();
			ammo = (int)WeaponInfo.gun.ammoLeft;

			if (!PlayerWeapons.saveUpgradesToDrops)
				return;
			upgradesApplied = new bool[WeaponInfo.upgrades.Length];
			for (int i = 0; i < WeaponInfo.upgrades.Length; i++)
			{
				upgradesApplied[i] = WeaponInfo.upgrades[i].applied;
			}
		}
	}
}