using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	public class Axis : MonoBehaviour
	{
		public string key;
		public InputItem input;

		public void UpdateInput()
		{
			//Just get the axis value from Unity's input
			input.axis = Input.GetAxisRaw(key);
		}
	}
}