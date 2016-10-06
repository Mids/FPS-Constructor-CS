using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class HighlightFX : MonoBehaviour
	{
		private bool selected = false;
		public string objectName;
		public string description;

		public void HighlightOn()
		{
			selected = true;
		}

		public void HighlightOff()
		{
			selected = false;
		}

		void OnGUI()
		{
			GUI.skin.box.wordWrap = true;
			if (selected && !DBStoreController.inStore)
			{
				GUI.skin.box.fontSize = 15;
				string s = "(Tab) to Use";

				Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
				GUI.Box(new Rect(pos.x - 77.5f, Screen.height - pos.y - (Screen.height / 4) - 52.5f, 155, 105), objectName + "\n \n" + description + "\n" + s);
			}
		}
	}
}