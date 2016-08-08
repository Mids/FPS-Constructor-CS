using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class Drag : MonoBehaviour
	{
		public enum dirs
		{
			x,
			y
		} //which rotation axis should we use?
		public dirs direction = dirs.x;

		public bool invert; //should it be inverted?

		public InputItem input;
		public float sensitivity;
		public TouchButton[] buttons;
		private int touch;
		private bool inButtons;

		void FixedUpdate()
		{
			float x = 0;
			float y = 0;

			int t = 0;

			if (Input.touches.Length > 0)
			{
				//user is touching
				for (t = 0; t < Input.touches.Length; t++)
				{
					//for each touch

					//check if that touch is currently touching a button
					inButtons = false;
					for (int b = 0; b < buttons.Length; b++)
					{
						if (Input.touches[t].fingerId == buttons[b].curTouch)
						{
							inButtons = true;
							break;
						}
					}
					if (!inButtons) //if it wasn't, then we have found our touch
						break;
				}

				//if no touch was viable
				if (inButtons)
					return;

				if (Input.touches[t].phase == TouchPhase.Moved)
				{
					//the touch moved
					x = Input.touches[t].deltaPosition.x * sensitivity * Time.deltaTime;
					y = Input.touches[t].deltaPosition.y * sensitivity * Time.deltaTime;
				}
			}
			else
			{
				// zero out axis
				x = 0;
				y = 0;
			}

			//invert if needed
			if (invert)
			{
				x *= -1;
				y *= -1;
			}


			//set proper axis
			if (direction == dirs.x)
			{
				input.axis = x;
			}
			else
			{
				input.axis = y;
			}
		}
	}
}