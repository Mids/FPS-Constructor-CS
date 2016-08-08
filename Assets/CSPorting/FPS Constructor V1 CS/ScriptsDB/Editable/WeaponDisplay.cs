using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class WeaponDisplay : MonoBehaviour
	{
		private bool display = false;
		private float endTime;
		private GameObject weapon;
		private WeaponInfo WeaponInfo;

		public float displayTime = 2;

		void Start()
		{
			WeaponInfo = GetComponent<WeaponInfo>();
			display = false;
		}

		public void Select()
		{
			display = true;
			endTime = Time.time + displayTime;
		}

		void OnGUI()
		{
			if (display && Time.time != 0.0f)
			{
				if (Time.time > endTime) display = false;
				GUI.Box(new Rect(Screen.width - 255, Screen.height - 85, 135, 75), WeaponInfo.gunName + "\n" + WeaponInfo.gunDescription + "\n");
			}
		}
	}
}