using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	public class SprintDisplay : MonoBehaviour
	{

		[HideInInspector]
		public AimMode aim;

		void Start()
		{
			aim = this.GetComponent<AimMode>();
		}

		void OnGUI()
		{
			if (!aim.GunScript1.gunActive)
			{
				return;
			}
			if (aim.scopeTexture != null && aim.inScope)
			{
				GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), aim.scopeTexture, ScaleMode.StretchToFill);
			}
			//temp is the percentage of sprint remaining
			float temp = AimMode.sprintNum / aim.sprintDuration;
			//baselength is the length of the bar at full sprint
			float baseLength = Screen.width * .5f;
			//ypos is the y position of the sprint bar
			float yPos = Screen.height;
			yPos -= (Screen.height / 10);

			//tempLength is the length of the sprint bar we want to display
			float tempLength = Mathf.Clamp(baseLength * (temp), baseLength * .03f, baseLength);

			//only display the bar if we are sprinting, and don't display if sprint is full
			if (aim.sprinting || AimMode.sprintNum < aim.sprintDuration)
			{
				//display the sprint bar - change this to modify the display.
				GUI.Box(new Rect((Screen.width - baseLength) / 2, yPos, tempLength, 10), "");
			}
		}
	}
}