using UnityEngine;
using System.Collections;
using System.Text;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class DBStoreController : MonoBehaviour
	{
		public class WeaponClassArrayType
		{
			public string weaponClass;
			public WeaponInfo[] WeaponInfoArray;
		}

		public static bool storeActive = false;
		public static bool canActivate = true;
		public static DBStoreController singleton;

		public float balance; // Store account balance 

		//Vector2 scrollPosition;
		[HideInInspector] public WeaponInfo[] WeaponInfoArray;
		[HideInInspector] public WeaponClassArrayType[] WeaponInfoByClass;
		[HideInInspector] public string[] weaponClassNames;
		[HideInInspector] public string[] weaponClassNamesPopulated;
		[HideInInspector] public PlayerWeapons playerW;
		[HideInInspector] public GameObject nullWeapon; //there must be one null weapon as a placeholder to put in an empty slot.
		[HideInInspector] public SlotInfo slotInfo;
		public bool canExitWhileEmpty = false;
		public static bool inStore = false;

		public void Initialize()
		{
			singleton = this;
			playerW = FindObjectOfType(typeof (PlayerWeapons)) as PlayerWeapons;
			slotInfo = FindObjectOfType(typeof (SlotInfo)) as SlotInfo;
			WeaponInfoArray = FindObjectsOfType(typeof (WeaponInfo)) as WeaponInfo[];
			foreach (WeaponInfo w in WeaponInfoArray)
			{
				if (w.weaponClass == WeaponInfo.weaponClasses.Null)
					nullWeapon = w.gameObject;
			}
			setupWeaponClassNames();
			setupWeaponInfoByClass();
		}


		public int getNumOwned(int slot)
		{
			//will use the slot info later to restrict count
			int n = 0;
			for (int i = 0; i < WeaponInfoArray.Length; i++)
			{
				if (WeaponInfoArray[i].owned && slotInfo.isWeaponAllowed(slot, WeaponInfoArray[i]))
					n++;
			}
			return n;
		}

		public string[] getWeaponNamesOwned(int slot)
		{
			string[] names = new string[getNumOwned(slot)];
			int n = 0;
			for (int i = 0; i < WeaponInfoArray.Length; i++)
			{
				if (WeaponInfoArray[i].owned && slotInfo.isWeaponAllowed(slot, WeaponInfoArray[i]))
				{
					names[n] = WeaponInfoArray[i].gunName;
					n++;
				}
			}
			return names;
		}

		public WeaponInfo[] getWeaponsOwned(int slot)
		{
			WeaponInfo[] w = new WeaponInfo[getNumOwned(slot)];
			int n = 0;
			for (int i = 0; i < WeaponInfoArray.Length; i++)
			{
				if (WeaponInfoArray[i].owned && slotInfo.isWeaponAllowed(slot, WeaponInfoArray[i]))
				{
					w[n] = WeaponInfoArray[i];
					n++;
				}
			}
			return w;
		}


		public void Update()
		{
			if (InputDB.GetButtonDown("Store"))
			{
				if (!storeActive && canActivate && !GunScript.takingOut && !GunScript.puttingAway)
				{
					activateStore();
				}
				else if (storeActive)
				{
					deActivateStore();
				}
			}
		}

		public void setupWeaponClassNames()
		{
			string[] names;
			ArrayList nameArray = new ArrayList();

			foreach (WeaponInfo.weaponClasses w in WeaponInfo.weaponClasses.GetValues(typeof (WeaponInfo.weaponClasses)))
			{
				nameArray.Add(w.ToString().Replace("_", " "));
			}
			weaponClassNames = (string[]) nameArray.ToArray(typeof (string));
		}

		//Organize Weapon Information by Weapon Class for use within the GUI
		//Note: the code assumes the last weapon class is "Null" so the array is one element shorter than the number of
		//		weapon classes.

		public void setupWeaponInfoByClass()
		{
			//check to see how many Weapon Classes have one or more weapons

			int n = 0;
			for (int i = 0; i < weaponClassNames.Length - 1; i++)
			{
				for (int j = 0; j < WeaponInfoArray.Length; j++)
				{
					if (WeaponInfoArray[j].weaponClass == (WeaponInfo.weaponClasses) i)
					{
						n++;
						break;
					}
				}
			}
			weaponClassNamesPopulated = new string[n];
			WeaponInfoByClass = new WeaponClassArrayType[n]; // size array to hold all non-Null weapon classes with at least one weapon

			n = 0;
			for (int i = 0; i < weaponClassNames.Length - 1; i++)
			{
				ArrayList arr = new ArrayList();
				for (int j = 0; j < WeaponInfoArray.Length; j++)
				{
					if (WeaponInfoArray[j].weaponClass == (WeaponInfo.weaponClasses) i)
					{
						arr.Add(WeaponInfoArray[j]);
					}
				}
				if (arr.Count > 0)
				{
					WeaponInfoByClass[n] = new WeaponClassArrayType();
					WeaponInfoByClass[n].WeaponInfoArray = (WeaponInfo[]) arr.ToArray(typeof (string));
					WeaponInfoByClass[n].weaponClass = weaponClassNames[i];
					weaponClassNamesPopulated[n] = weaponClassNames[i];
					n++;
				}
			}
		}

		public void activateStore()
		{
			PlayerWeapons.HideWeaponInstant();
			Time.timeScale = 0;
			//	GUIController.state = GUIStates.store;
			inStore = true;
			//BroadcastMessage("Unlock");
			storeActive = true;
			Screen.lockCursor = false;
			LockCursor.canLock = false;
			//playerW.BroadcastMessage("DeselectWeapon"); //turn off graphics/weapons on entering store
			GameObject player = GameObject.FindWithTag("Player");
			player.BroadcastMessage("Freeze", SendMessageOptions.DontRequireReceiver);
		}

		public void deActivateStore()
		{
			if (PlayerWeapons.HasEquipped() <= 0 && !canExitWhileEmpty)
				return;
			storeActive = false;
			inStore = false;
			//GUIController.state = GUIStates.playing;
			//BroadcastMessage("Lock");
			Time.timeScale = 1;
			Screen.lockCursor = true;
			LockCursor.canLock = true;
			PlayerWeapons.ShowWeapon();
			//playerW.SelectWeapon(playerW.selectedWeapon); // activate graphics on selected weapon
			GameObject player = GameObject.FindWithTag("Player");
			player.BroadcastMessage("UnFreeze", SendMessageOptions.DontRequireReceiver);
		}

		public void equipWeapon(WeaponInfo g, int slot)
		{
			//if the weapon is equipped in another slot, unequip it
			if (slot < 0)
				return;
			for (int i = 0; i < playerW.weapons.Length; i++)
			{
				if (g.gameObject == playerW.weapons[i])
				{
					unEquipWeapon(g, i);
				}
			}
			if (playerW.weapons[playerW.selectedWeapon] == null)
				playerW.selectedWeapon = slot;
			playerW.BroadcastMessage("DeselectWeapon", SendMessageOptions.DontRequireReceiver);
			GunScript tempGScript = g.gameObject.GetComponent<GunScript>().GetPrimaryGunScript();
			tempGScript.ammoLeft = tempGScript.ammoPerClip;
			playerW.SetWeapon(g.gameObject, slot);
		}

		public void unEquipWeapon(WeaponInfo g, int slot)
		{
			playerW.weapons[slot] = null;
		}

		public void buyUpgrade(WeaponInfo g, Upgrade u)
		{
			withdraw(u.buyPrice);
			u.owned = true;
			g.ApplyUpgrade();
		}

		public void buyWeapon(WeaponInfo g)
		{
			withdraw(g.buyPrice);
			g.owned = true;
			g.ApplyUpgrade();
			equipWeapon(g, autoEquipWeapon(g, false));
		}

		public void BuyAmmo(WeaponInfo g)
		{
			withdraw(g.ammoPrice);
			GunScript temp = g.gameObject.GetComponent<GunScript>().GetPrimaryGunScript();
			temp.clips = Mathf.Min(temp.clips + temp.ammoPerClip, temp.maxClips);
			temp.ApplyToSharedAmmo();
		}

		public void sellWeapon(WeaponInfo g)
		{
			if (!g.canBeSold)
				return;
			for (int i = 0; i < g.upgrades.Length; i++)
			{
				g.upgrades[i].owned = false;
				g.upgrades[i].RemoveUpgrade();
			}
			DropWeapon(g);
			deposit(g.sellPriceUpgraded);
			g.owned = false;
		}

		public float getBalance()
		{
			return balance;
		}

		//Function to deposit money - returns the new balance
		public float deposit(float amt)
		{
			balance += amt;
			return balance;
		}

		// Function to withdraw money  - returns amount withdrawn 
		// You can't withdraw more than the balance.
		public float withdraw(float amt)
		{
			if (amt <= balance)
			{
				balance -= amt;
				return amt;
			}
			else
			{
				float oldBalance = balance;
				balance = 0;
				return oldBalance;
			}
		}

		public int autoEquipWeapon(WeaponInfo w, bool auto)
		{
			//Slot the weapon is equipped in, -1 if not equipped
			int slot = -1;

			//find the first empty slot that can hold w
			for (int i = 0; i < playerW.weapons.Length; i++)
			{
				if (slotInfo.isWeaponAllowed(i, w) && playerW.weapons[i] == null)
				{
					//equipWeapon(w,i);
					slot = i;
				}
				if (slot >= 0)
					return slot;
			}
			if (!auto)
				return slot;

			if (slotInfo.isWeaponAllowed(playerW.selectedWeapon, w))
			{
				//equipWeapon(w,i);
				slot = playerW.selectedWeapon;
				return slot;
			}

			for (int i = 0; i < playerW.weapons.Length; i++)
			{
				if (slotInfo.isWeaponAllowed(i, w))
				{
					//equipWeapon(w,i);
					slot = i;
				}
				if (slot >= 0)
					return slot;
			}
			return slot;
		}

		public int autoEquipWeaponWithReplacement(WeaponInfo w, bool auto)
		{
			//Slot the weapon is equipped in, -1 if not equipped
			int slot = -1;

			//See if the weapon is already equipped and can be replaced
			for (int i = 0; i < PlayerWeapons.PW.weapons.Length; i++)
			{
				if (slotInfo.isWeaponAllowed(i, w) && (playerW.weapons[i] == null || (playerW.weapons[i].gameObject == w.gameObject)))
				{
					//equipWeapon(w,i);
					slot = i;
				}
				if (slot >= 0)
					return slot;
			}

			//find the first empty slot that can hold w
			for (int i = 0; i < playerW.weapons.Length; i++)
			{
				if (slotInfo.isWeaponAllowed(i, w) && PlayerWeapons.PW.weapons[i] == null)
				{
					//equipWeapon(w,i);
					slot = i;
				}
				if (slot >= 0)
					return slot;
			}
			if (!auto)
				return slot;

			if (slotInfo.isWeaponAllowed(PlayerWeapons.PW.selectedWeapon, w))
			{
				//equipWeapon(w,i);
				slot = playerW.selectedWeapon;
				return slot;
			}
			for (int i = 0; i < PlayerWeapons.PW.weapons.Length; i++)
			{
				if (slotInfo.isWeaponAllowed(i, w))
				{
					//equipWeapon(w,i);
					slot = i;
				}
				if (slot >= 0)
					return slot;
			}
			return slot;
		}

		//Drops weapon at given index in Weapons[]
		public void DropWeapon(WeaponInfo g)
		{
			//Weapon Drop
			int wep = -1;
			for (int i = 0; i < playerW.weapons.Length; i++)
			{
				if (playerW.weapons[i].gameObject == g.gameObject)
				{
					wep = i;
					break;
				}
			}
			if (wep < 0) return;

			playerW.weapons[wep] = null;

			//Ceck if we have a weapon to switch to
			int prevWeapon = -1;
			for (int i = wep - 1; i >= 0; i--)
			{
				if (playerW.weapons[i] != null)
				{
					prevWeapon = i;
					break;
				}
			}

			int nextWeapon = -1;
			if (prevWeapon == -1)
			{
				for (int i = wep + 1; i < playerW.weapons.Length; i++)
				{
					if (playerW.weapons[i] != null)
					{
						nextWeapon = i;
						break;
					}
				}
				prevWeapon = nextWeapon;

				if (nextWeapon == -1)
					return;
			}

			playerW.selectedWeapon = prevWeapon;
			playerW.SelectWeapon(prevWeapon);
		}
	}
}