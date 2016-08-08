using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class GlobalUpgrade : MonoBehaviour
	{
		public Upgrade upgrade;
		public static WeaponInfo[] WeaponArray;
		private Upgrade[] Upgrades;
		private bool applied = false;
		public bool[] classesAllowed;

		public void Start()
		{
			if (WeaponArray == null)
				WeaponArray = FindObjectsOfType(typeof (WeaponInfo)) as WeaponInfo[];
		}

		public void Apply()
		{
			applied = true;
			Transform temp;
			Upgrade up;
			List<Upgrade> upgradeArray = new List<Upgrade>();
			for (int i = 0; i < WeaponArray.Length; i++)
			{
				int enumIndex = (int) WeaponArray[i].weaponClass;

				if (classesAllowed[enumIndex])
				{
					temp = ((GameObject) Instantiate(upgrade.gameObject, transform.position, transform.rotation)).transform;
					temp.parent = WeaponArray[i].transform;
					temp.name = upgrade.upgradeName;
					up = temp.GetComponent<Upgrade>();
					up.Init();
					up.ApplyUpgrade();
					up.showInStore = false;
					upgradeArray.Add(up);
				}
			}
			Upgrades = upgradeArray.ToArray();

			this.SendMessage("Apply");
		}

		public void UnApply()
		{
			applied = false;
			this.SendMessage("Remove");
			for (int i = 0; i < Upgrades.Length; i++)
			{
				Upgrades[i].DeleteUpgrade();
			}
		}
	}
}