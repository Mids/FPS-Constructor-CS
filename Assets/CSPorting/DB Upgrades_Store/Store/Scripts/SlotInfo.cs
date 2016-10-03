using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class SlotInfo : MonoBehaviour
	{
		// Helper Class for configuring which weapon classes are allowed in which slots
		public string[] slotName;
		public int[] allowed;

		public void clearAllowed(int slot)
		{
			allowed[slot] = 0;
		}

		public void setAllowed(int slot, WeaponInfo.weaponClasses wc, bool b)
		{
			if (b)
			{
				allowed[slot] = allowed[slot] | 1 << (int)wc;
			}
			else
			{
				allowed[slot] = allowed[slot] & ~(1 << (int)wc);
			}
		}


		public bool isWCAllowed(int slot, WeaponInfo.weaponClasses wc)
		{
			bool ret = false;
			if ((allowed[slot] & 1 << (int)wc) != 0)
				ret = true;
			return ret;
		}

		public bool isWeaponAllowed(int slot, WeaponInfo w)
		{
			bool ret = false;
			if ((allowed[slot] & 1 << (int)w.weaponClass) != 0)
				ret = true;
			return ret;
		}
	}
}