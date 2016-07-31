using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	public class SprintDisplayPlayer : MonoBehaviour
	{
		[HideInInspector]
		public PlayerSprint aim;

		void Start()
		{
			aim = this.GetComponent<PlayerSprint>();
		}

		void OnGUI()
		{
			if (!aim.weaponsInactive)
			{
				return;
			}

			//temp is the percentage of sprint remaining
			float temp = AimMode.sprintNum / aim.values.sprintDuration;
			//baselength is the length of the bar at full sprint
			float baseLength = Screen.width * .5f;
			//ypos is the y position of the sprint bar
			float yPos = Screen.height;
			yPos -= (Screen.height / 10);

			//tempLength is the length of the sprint bar we want to display
			float tempLength = Mathf.Clamp(baseLength * (temp), baseLength * .03f, baseLength);

			//only display the bar if we are sprinting, and don't display if sprint is full
			if (PlayerSprint.sprinting || AimMode.sprintNum < aim.values.sprintDuration)
			{
				//display the sprint bar - change this to modify the display.
				GUI.Box(new Rect((Screen.width - baseLength) / 2, yPos, tempLength, 10), "");
			}
		}
	}
}