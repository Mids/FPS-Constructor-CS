using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class WeaponSelector : MonoBehaviour
	{
		public static int selectedWeapon; //this value must be changed to switch weapons.

		void Awake()
		{
			selectedWeapon = 0;
		}

		void LateUpdate()
		{
			selectedWeapon = PlayerWeapons.PW.selectedWeapon;
		}
	}
}