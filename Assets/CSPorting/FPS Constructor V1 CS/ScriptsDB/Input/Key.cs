using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class Key : MonoBehaviour
	{
		public string key;
		public InputItem input;

		public void UpdateInput()
		{
			//Just get the values from Unity's input
			input.got = Input.GetButton(key);
			input.down = Input.GetButtonDown(key);
			input.up = Input.GetButtonUp(key);
		}
	}
}