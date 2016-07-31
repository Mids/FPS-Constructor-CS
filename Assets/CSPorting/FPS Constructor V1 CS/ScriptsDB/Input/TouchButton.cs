using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	public class TouchButton : MonoBehaviour
	{

		public InputItem input;

		public Vector2 position; //position of button
		public Vector2 dimensions; //size of button
		public string label; //text in button
		public bool toggle = false; //is this button a toggle?
		[HideInInspector]
		public bool toggled = false; //are we currently toggled on?
		public bool showInStore = false;
		private bool used = false;

		private bool touched = false; //had we already touched the button
		private bool touching = false; //are we currently touching the button
		[HideInInspector]
		public int curTouch = -1; //what touch id is this using?
		public bool useUpdate = true;

		void Update()
		{
			if (useUpdate)
				UpdateFunction();
		}

		public void UpdateInput()
		{
			if (!useUpdate)
				UpdateFunction();
		}

		public void UpdateFunction()
		{
			//are we touching the button this frame?
			if (Input.touches.Length > 0)
			{
				foreach (Touch touch in Input.touches)
				{ //for each touch
					//Is this touch within our button?
					touching = Within(touch.position, new Rect(position.x, position.y, dimensions.x, dimensions.y));
					if (touching)
					{
						curTouch = touch.fingerId; //save which touch we are using
						break;
					}
				}
			}
			else
			{
				touching = false;
			}

			if (toggle)
			{ //Toggle button
				input.got = toggled;

				if (touching)
				{
					if (!touched)
					{ //first frame touching the button
						touched = true;

						input.up = toggled;
						toggled = !toggled; //invert the toggle
						input.down = toggled;
					}
					else
					{
						input.down = false;
						input.up = false;
					}
				}
				else
				{
					input.down = false;
					input.up = false;
					touched = false;
					curTouch = -1;
				}

			}
			else
			{ //Normal Button
				if (touching)
				{ //We are touching
					input.got = true; //the button is down
					input.up = false; //the button is not up

					if (!touched)
					{// we hadn't already touched the button (first frame holding it)
						input.down = true; //the button was got
						touched = true; //we have touched	
					}
					else
					{
						input.down = false; //it isn't down because this isn't the first fram holding it
					}
				}
				else
				{ //We are not touching
					curTouch = -1;
					if (touched)
					{
						input.up = true; //if we were holding the button last fram, then up is true because this is the frame it was released
					}
					else
					{
						input.up = false;
					}
					touched = false;
					input.got = false;
					input.down = false;
				}
			}
		}

		void OnGUI()
		{
			if (!DBStoreController.inStore || showInStore)
				GUI.Button(new Rect(position.x, position.y, dimensions.x, dimensions.y), label);
		}

		public bool Within(Vector2 pos, Rect bounds)
		{
			pos.y = Screen.height - pos.y;
			return (pos.x > bounds.x && pos.x < (bounds.x + bounds.width) && pos.y > bounds.y && pos.y < (bounds.y + bounds.height));
		}
	}
}