using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	public class MouseButton : MonoBehaviour
	{
		public int key;
		public InputItem input;

		public void UpdateInput()
		{
			//Just get the values from Unity's input
			input.got = Input.GetMouseButton(key);
			input.down = Input.GetMouseButtonDown(key);
			input.up = Input.GetMouseButtonUp(key);
		}
	}
}