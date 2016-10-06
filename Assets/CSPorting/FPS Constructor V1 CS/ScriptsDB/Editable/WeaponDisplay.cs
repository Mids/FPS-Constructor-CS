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
				GUI.skin.box.fontSize = 15;
				if (Time.time > endTime) display = false;
				GUI.Box(new Rect(Screen.width - 490, Screen.height - 160, 270, 150), WeaponInfo.gunName + "\n" + WeaponInfo.gunDescription + "\n");
			}
		}
	}
}