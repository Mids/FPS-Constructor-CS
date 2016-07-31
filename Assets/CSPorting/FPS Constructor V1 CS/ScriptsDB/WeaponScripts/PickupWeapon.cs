using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	public class PickupWeapon : MonoBehaviour
	{
		public RaycastHit hit;
		public Ray ray;
		//@HideInInspector
		//static GameObject selectedWeapon;
		//@HideInInspector
		public static GameObject lastSelectedWeapon;
		//@HideInInspector
		public static PlayerWeapons playerWeapons;
		public bool throwWeapons = true;
		public float throwForce;
		public float throwTorque;
		public bool destroyPickups = false;
		public static DBStoreController store;
		public static PickupWeapon singleton;


		void Start()
		{
			playerWeapons = PlayerWeapons.PW;
			store = FindObjectOfType<DBStoreController>();
			singleton = this;
		}

		public static bool CheckWeapons(GameObject selectedWeapon)
		{
			for (int i = 0; i < playerWeapons.weapons.Length; i++)
			{
				if (playerWeapons.weapons[i] == selectedWeapon.GetComponent<SelectableWeapon>().weapon)
				{
					return true;
				}
			}
			return false;
		}

		//Drops weapon at given index in Weapons[]
		public static void DropWeapon(int wep)
		{
			//Weapon Drop

			//Ceck if we have a weapon to switch to
			int prevWeapon = -1;
			for (int i = wep - 1; i >= 0; i--)
			{
				if (playerWeapons.weapons[i] != null)
				{
					prevWeapon = i;
					break;
				}
			}

			int nextWeapon = -1;
			if (prevWeapon == -1)
			{
				for (int i = wep + 1; i < playerWeapons.weapons.Length; i++)
				{
					if (playerWeapons.weapons[i] != null)
					{
						nextWeapon = i;
						break;
					}
				}
				prevWeapon = nextWeapon;

				if (nextWeapon == -1)
					return;
			}

			DropObject(wep, null);

			playerWeapons.selectedWeapon = prevWeapon;
			playerWeapons.SelectWeapon(prevWeapon);
		}

		//Swaps out currently selected for given one, dropping currently selected weapon
		public static void Pickup(GameObject selectedWeapon)
		{
			if (GunScript.takingOut)
				return;

			bool hasFlag = false;

			if (!PlayerWeapons.canSwapSameWeapon)
			{
				for (int i = 0; i < playerWeapons.weapons.Length; i++)
				{
					if (playerWeapons.weapons[i] == selectedWeapon.GetComponent<SelectableWeapon>().weapon)
					{
						return;
					}
				}
			}

			if (playerWeapons.weapons[playerWeapons.selectedWeapon] == selectedWeapon.GetComponent<SelectableWeapon>().weapon)
			{
				hasFlag = true;
			}
			WeaponInfo selectedWeaponInfo = selectedWeapon.GetComponent<SelectableWeapon>().weapon.GetComponent<WeaponInfo>();

			//Get applicable slot
			int theSlot = 1;//store.autoEquipWeaponWithReplacement(selectedWeaponInfo, true);
			if (theSlot < 0 && !playerWeapons.weapons[playerWeapons.selectedWeapon] == null)
			{
				return;
			}

			//We now own the weapon
			if (selectedWeaponInfo != null)
			{
				selectedWeaponInfo.owned = true;
				selectedWeaponInfo.locked = false;
			}

			DropObject(theSlot, selectedWeapon);

			GunScript gscript;
			if (playerWeapons.weapons[playerWeapons.selectedWeapon] != null)
				gscript = playerWeapons.weapons[playerWeapons.selectedWeapon].GetComponent<GunScript>().GetPrimaryGunScript();

			//Get new weapon
			if (hasFlag)
			{
				gscript = playerWeapons.weapons[playerWeapons.selectedWeapon].GetComponent<GunScript>().GetPrimaryGunScript();
				selectedWeapon.GetComponent<SelectableWeapon>().Apply(gscript);
				playerWeapons.weapons[playerWeapons.selectedWeapon].BroadcastMessage("DeselectInstant");
				playerWeapons.ActivateWeapon();
				//playerWeapons.weapons[playerWeapons.selectedWeapon].BroadcastMessage("SelectWeapon");
			}
			else
			{
				playerWeapons.weapons[theSlot] = selectedWeapon.GetComponent<SelectableWeapon>().weapon;
				playerWeapons.selectedWeapon = theSlot;
				gscript = playerWeapons.weapons[playerWeapons.selectedWeapon].GetComponent<GunScript>().GetPrimaryGunScript();
				selectedWeapon.GetComponent<SelectableWeapon>().Apply(gscript);
				playerWeapons.ActivateWeapon();
			}

			if (singleton.destroyPickups)
				Destroy(selectedWeapon);
		}


		public static void DropObject(int index, GameObject selectedWeapon)
		{
			//Deselect old weapon
			if (playerWeapons.weapons[playerWeapons.selectedWeapon])
			{
				if (selectedWeapon.GetComponent<SelectableWeapon>() != null)
				{
					if (playerWeapons.weapons[playerWeapons.selectedWeapon] != selectedWeapon.GetComponent<SelectableWeapon>().weapon)
					{
						playerWeapons.weapons[playerWeapons.selectedWeapon].gameObject.BroadcastMessage("DeselectWeapon");
					}
				}
			}

			//Weapon Drop
			if (playerWeapons.weapons[index] != null)
			{
				GameObject dropObj = playerWeapons.weapons[index].GetComponent<WeaponInfo>().drops;
				if (dropObj != null)
				{
					GameObject temp = Instantiate(dropObj, new Vector3(singleton.transform.position.x, singleton.transform.position.y - 1, singleton.transform.position.z), Quaternion.identity) as GameObject;
					temp.GetComponent<SelectableWeapon>().weapon = playerWeapons.weapons[index];
					temp.GetComponent<SelectableWeapon>().PopulateDrop();
					if (singleton.throwWeapons || selectedWeapon == null)
					{
						temp.GetComponent<Rigidbody>().AddForce(singleton.transform.forward * singleton.throwForce, ForceMode.Impulse);
						temp.GetComponent<Rigidbody>().AddTorque(singleton.transform.forward * singleton.throwTorque, ForceMode.Impulse);
					}
					else
					{
						Vector3 pos = selectedWeapon.transform.position;
						temp.transform.position = new Vector3(pos.x, pos.y + .4f, pos.z);
					}
				}
			}
		}
	}
}