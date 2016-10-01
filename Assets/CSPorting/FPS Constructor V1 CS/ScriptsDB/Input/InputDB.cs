using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class InputDB : MonoBehaviour
	{
		public static InputDB thisObj;
		public static InputItem[] buttons = new InputItem[0];
		public static InputItem[] axes = new InputItem[0];
		public static bool updated = false;

		void Start()
		{
			thisObj = this;
		}

		public static bool GetButtonDown(string s)
		{
			/*if(!updated){
				BroadcastMessage("UpdateInput");
				updated = true;
			}*/
			for (int i = 0; i < buttons.Length; i++)
			{
				if (s == buttons[i].id)
				{
					buttons[i].BroadcastMessage("UpdateInput", SendMessageOptions.DontRequireReceiver);
					return buttons[i].down;
				}
			}
			return false;
		}

		public static bool GetButton(string s)
		{
			/*if(!updated){
				BroadcastMessage("UpdateInput");
				updated = true;
			}*/
			for (int i = 0; i < buttons.Length; i++)
			{
				if (s == buttons[i].id)
				{
					buttons[i].BroadcastMessage("UpdateInput", SendMessageOptions.DontRequireReceiver);
					return buttons[i].got;
				}
			}
			return false;
		}

		public static bool GetButtonUp(string s)
		{
			/*if(!updated){
				BroadcastMessage("UpdateInput");
				updated = true;
			}*/
			for (int i = 0; i < buttons.Length; i++)
			{
				if (s == buttons[i].id)
				{
					buttons[i].BroadcastMessage("UpdateInput", SendMessageOptions.DontRequireReceiver);
					return buttons[i].up;
				}
			}
			return false;
		}

		public static float GetAxis(string s)
		{
			/*if(!updated){
				BroadcastMessage("UpdateInput");
				updated = true;
			}*/
			for (int i = 0; i < axes.Length; i++)
			{
				if (s == axes[i].id)
				{
					axes[i].BroadcastMessage("UpdateInput", SendMessageOptions.DontRequireReceiver);
					return axes[i].axis;
				}
			}
			return 0;
		}

		public static void ResetInputAxes()
		{
			for (int i = 0; i < axes.Length; i++)
			{
				axes[i].axis = 0;
			}
		}

		void LateUpdate()
		{
			updated = false;
		}
	}
}