using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class MovementValues : MonoBehaviour
	{
		public float defaultForwardSpeed = 10;
		public float maxCrouchSpeed = 6;
		public float maxSprintSpeed = 13;
		public float minSprintSpeed = 10;
		public float maxAimSpeed = 4;
		public float maxProneSpeed = 4;

		public float defaultSidewaysSpeed = 10;
		public float sprintSidewaysSpeed = 15;
		public float crouchSidewaysSpeed = 6;
		public float aimSidewaysSpeed = 4;
		public float proneSidewaysSpeed = 2;

		public float defaultBackwardsSpeed = 10;
		public float crouchBackwardsSpeed = 6;
		public float aimBackwardsSpeed = 4;
		public float proneBackwardsSpeed = 2;

		public bool sprintFoldout = false;
		public bool crouchFoldout = false;
		public bool defaultFoldout = false;
		public bool proneFoldout = false;
		public bool aimFoldout = false;
		public bool jumpFoldout = false;

		public CharacterMotorDB CM;

		public int sprintDuration = 5; //how long can we sprint for?
		public float sprintAddStand = 1; //how quickly does sprint replenish when idle?
		public float sprintAddWalk = .3f; //how quickly does sprint replenish when moving?
		public float sprintMin = 1; //What is the minimum value ofsprint at which we can begin sprinting?
		public float recoverDelay = .7f; //how much time after sprinting does it take to start recovering sprint?
		public float exhaustedDelay = 1; //how much time after sprinting to exhaustion does it take to start recovering sprint?

		public static MovementValues singleton;

		void Awake()
		{
			singleton = this;
		}
	}
}