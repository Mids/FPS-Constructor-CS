using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	public class InputDB : MonoBehaviour
	{

		public static InputDB thisObj;
		public InputItem[] buttons;
		public InputItem[] axes;
		public static bool updated = false;

		void Awake()
		{
			thisObj = this;
		}

		public static bool GetButtonDown(string s)
		{
			/*if(!updated){
				thisObj.BroadcastMessage("UpdateInput");
				updated = true;
			}*/
			for (int i = 0; i < thisObj.buttons.Length; i++)
			{
				if (s == thisObj.buttons[i].id)
				{
					thisObj.buttons[i].BroadcastMessage("UpdateInput", SendMessageOptions.DontRequireReceiver);
					return thisObj.buttons[i].down;
				}
			}
			return false;
		}

		public static bool GetButton(string s)
		{
			/*if(!updated){
				thisObj.BroadcastMessage("UpdateInput");
				updated = true;
			}*/
			for (int i = 0; i < thisObj.buttons.Length; i++)
			{
				if (s == thisObj.buttons[i].id)
				{
					thisObj.buttons[i].BroadcastMessage("UpdateInput", SendMessageOptions.DontRequireReceiver);
					return thisObj.buttons[i].got;
				}
			}
			return false;
		}

		public static bool GetButtonUp(string s)
		{
			/*if(!updated){
				thisObj.BroadcastMessage("UpdateInput");
				updated = true;
			}*/
			for (int i = 0; i < thisObj.buttons.Length; i++)
			{
				if (s == thisObj.buttons[i].id)
				{
					thisObj.buttons[i].BroadcastMessage("UpdateInput", SendMessageOptions.DontRequireReceiver);
					return thisObj.buttons[i].up;
				}
			}
			return false;
		}

		public static float GetAxis(string s)
		{
			/*if(!updated){
				thisObj.BroadcastMessage("UpdateInput");
				updated = true;
			}*/
			for (int i = 0; i < thisObj.axes.Length; i++)
			{
				if (s == thisObj.axes[i].id)
				{
					thisObj.axes[i].BroadcastMessage("UpdateInput", SendMessageOptions.DontRequireReceiver);
					return thisObj.axes[i].axis;
				}
			}
			return 0;
		}

		public static void ResetInputAxes()
		{
			for (int i = 0; i < thisObj.axes.Length; i++)
			{
				thisObj.axes[i].axis = 0;
			}
		}

		void LateUpdate()
		{
			updated = false;
		}
	}
}