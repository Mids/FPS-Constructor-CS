using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class SelectTouch : MonoBehaviour
	{
		public InputItem input;
		public Vector2 pos1; //position of button 1
		public Vector2 pos2; //position of button 2
		public Vector2 dimensions; //size of buttons
		private int selected = 0;

		//was either button already being touched?
		private bool touched1 = false;
		private bool touched2 = false;

		public int numWeapons; //number of weapons in PlayerWeapons array

		public string label1;
		public string label2;

		public void UpdateInput()
		{
			//are we touching one of the buttons this frame? Used in loop
			bool touching1 = false;
			bool touching2 = false;

			//flag variables for if touch 
			bool t1 = false;
			bool t2 = false;

			if (Input.touches.Length > 0)
			{
				foreach (Touch touch in Input.touches)
				{
					touching1 = false;
					touching2 = false;
					//Is the touch within the bounds of wither button?
					touching1 = Within(touch.position, new Rect(pos1.x, pos1.y, dimensions.x, dimensions.y));
					touching2 = Within(touch.position, new Rect(pos2.x, pos2.y, dimensions.x, dimensions.y));
					if (touching1)
						t1 = true;
					if (touching2)
						t2 = true;
				}
			}

			if (!AimMode.canSwitchWeaponAim || !PlayerWeapons.canSwitchWeapon)
			{
				return;
			}

			if (t1 && !touched1)
			{
				//we hit the first button
				CycleWeapon(-1); //previous weapon
				WeaponSelector.selectedWeapon = selected;
				touched1 = true;
				input.down = true;
			}
			else if (t2 && !touched2)
			{
				//we hit the second button
				CycleWeapon(1);
				WeaponSelector.selectedWeapon = selected;
				touched2 = true;
				input.down = true;
			}
			else if (!(t1 || t2))
			{
				//We are not touching
				input.down = false;
				touched1 = false;
				touched2 = false;
			}

			//Wraparound on weapon array

			if (selected < 0)
			{
				selected = numWeapons - 1;
				WeaponSelector.selectedWeapon = selected;
			}
			if (selected >= numWeapons)
			{
				selected = 0;
				WeaponSelector.selectedWeapon = selected;
			}
		}

		public int CycleWeapon(int dir)
		{
			selected += dir;
			if (selected >= PlayerWeapons.PW.weapons.Length)
			{
				selected = 0;
			}
			else if (selected < 0)
			{
				selected = PlayerWeapons.PW.weapons.Length - 1;
			}

			int temp = selected;

			for (int i = 0; i < PlayerWeapons.PW.weapons.Length; i++)
			{
				if (PlayerWeapons.PW.weapons[selected] != null)
				{
					return selected;
				}
				selected += dir;
				if (selected >= PlayerWeapons.PW.weapons.Length)
				{
					selected = 0;
				}
				else if (selected < 0)
				{
					selected = PlayerWeapons.PW.weapons.Length - 1;
				}
			}
			return temp;
		}

		void LateUpdate()
		{
			selected = PlayerWeapons.PW.selectedWeapon;
		}

		void OnGUI()
		{
			//Just buttons for display, not acutally used
			GUI.Button(new Rect(pos1.x, pos1.y, dimensions.x, dimensions.y), label1);
			GUI.Button(new Rect(pos2.x, pos2.y, dimensions.x, dimensions.y), label2);
		}

		public bool Within(Vector2 pos, Rect bounds)
		{
			pos.y = Screen.height - pos.y;
			return (pos.x > bounds.x && pos.x < (bounds.x + bounds.width) && pos.y > bounds.y && pos.y < (bounds.y + bounds.height));
		}
	}
}