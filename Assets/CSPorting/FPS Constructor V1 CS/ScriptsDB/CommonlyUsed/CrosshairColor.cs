using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class CrosshairColor : MonoBehaviour
	{
		//Custom editor 
		public enum crosshairTypes
		{
			Friend,
			Foe,
			Other
		}

		public crosshairTypes crosshairType;

		private GameObject weaponCam;

		public void Start()
		{
			weaponCam = GameObject.FindWithTag("WeaponCamera");
		}
	}
}