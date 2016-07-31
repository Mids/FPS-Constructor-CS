using UnityEngine;
using System.Collections;

namespace ooparts.fpsctorcs
{
	public class AimMode : MonoBehaviour
	{

		/////////////////////////// CHANGEABLE BY USER ///////////////////////////

		Texture scopeTexture; //Currently used scope texture
		bool overrideSprint = false;
		int sprintDuration = 5; //how long can we sprint for?
		float sprintAddStand = 1; //how quickly does sprint replenish when idle?
		float sprintAddWalk = 0.3f; //how quickly does sprint replenish when moving?
		float sprintMin = 1; //What is the minimum value ofsprint at which we can begin sprinting?
		float recoverDelay = 0.7f;	//how much time after sprinting does it take to start recovering sprint?
		float exhaustedDelay = 1; //how much time after sprinting to exhaustion does it take to start recovering sprint?
		bool crosshairWhenAiming = false;

		//Changes to the following variable will only be reflected when AimPrimary() or AimSecondary() are called
		//Calling either while aiming is not suggested

		//Zoom info for secondary weapon
		float zoomFactor2 = 1;
		bool scoped2 = false;
		bool sightsZoom2 = false;
		//Zoom info for primary weapon
		float zoomFactor1 = 1;
		bool scoped1 = false;
		bool sightsZoom1 = false;

		//Set of positions and rotations used for primary weapon
		Vector3 aimPosition1;
		Vector3 aimRotation1;
		Vector3 hipPosition1;
		Vector3 hipRotation1;
		Vector3 sprintPosition1;
		Vector3 sprintRotation1;

		//Set of positions and rotations used for secondary weapon
		Vector3 aimPosition2;
		Vector3 aimRotation2;
		Vector3 hipPosition2;
		Vector3 hipRotation2;
		Vector3 sprintPosition2;
		Vector3 sprintRotation2;

		private float aimStartTime;

		///////////////////////// END CHANGEABLE BY USER /////////////////////////



		///////////////////////// Internal Variables /////////////////////////
		/*These variables should not be modified directly, weither because it could compromise
		the functioning of the package, or because changes will be overwritten or otherwise
		ignored.
		*/





		Texture st169; //scope texture for 16 : 9 aspect ration
		Texture st1610; // 16 :10
		Texture st43; // 4 : 3
		Texture st54; // 5 : 4
		private GameObject player; //Player object
		bool scoped = false; //Does this weapon use a scope?
		private float scopeTime; //Time when we should be in scope
		bool sightsZoom = false; //Does this weapon zoom when aiming? (Not scoped)
		bool inScope = false; //Are we currently scoped?
		bool aim = true; //Does the primary weapon aim?
		bool secondaryAim = true; //Does the Secondaey weapon Aim?
		bool canAim; //does the weapon currently aim?
		bool aiming; //are we currently aiming?

		static bool sprintingPublic; //are we currently sprinting?
		bool sprinting;
		bool canSprint; //can the player sprint currently?

		private Vector3 deltaAngle;
		private bool selected = false;
		static float sprintNum;
		float aimRate = 3;
		float sprintRate = 0.4f;
		float retRate = 0.4f;
		private GameObject cmra;
		private GameObject wcmra;

		float zoomFactor = 1; //how much does this zoom in when aiming? (currently)

		//Set of positions and rotations used
		Vector3 aimPosition;
		Vector3 aimRotation;
		Vector3 hipPosition;
		Vector3 hipRotation;
		Vector3 sprintPosition;
		Vector3 sprintRotation;

		float rotationSpeed = 180;
		CharacterController controller;
		private bool zoomed = false;
		static bool canSwitchWeaponAim = true;
		static bool staticAiming = false;
		bool hasSecondary = true;
		GunScript GunScript1;
		private Vector3 curVect;
		private float sprintEndTime = 0;
		private CharacterMotorDB CM;
		static bool exhausted = false;
		private bool switching = false;

		private Vector3 startPosition;
		private Vector3 startRotation;
		private float moveProgress;
		static float staticRate;


		void Start()
		{
			if (aimRate <= 0)
				aimRate = 0.3f;
			if (!overrideSprint)
			{
				//Get sprint info form MovementValues
				sprintDuration = MovementValues.singleton.sprintDuration;
				sprintAddStand = MovementValues.singleton.sprintAddStand;
				sprintAddWalk = MovementValues.singleton.sprintAddWalk;
				sprintMin = MovementValues.singleton.sprintMin;
				recoverDelay = MovementValues.singleton.recoverDelay;
				exhaustedDelay = MovementValues.singleton.exhaustedDelay;
			}
			AimPrimary();
			cmra = PlayerWeapons.mainCam;
			wcmra = PlayerWeapons.weaponCam;
			player = PlayerWeapons.player;
			sprintNum = sprintDuration;
			canSprint = true;
			aiming = false;
			sprinting = false;
			controller = player.GetComponent<CharacterController>();
			if (zoomFactor == 0)
			{
				zoomFactor = 1;
			}
			AspectCheck();
			CM = GameObject.FindWithTag("Player").GetComponent<CharacterMotorDB>();
		}

		void AspectCheck()
		{
			if (cmra.GetComponent<Camera>().aspect == 1.6f && st1610 != null)
			{
				scopeTexture = st1610;
			}
			else if (Mathf.Round(cmra.GetComponent<Camera>().aspect) == 2 && st169 != null)
			{
				scopeTexture = st169;
			}
			else if (cmra.GetComponent<Camera>().aspect == 1.25f && st54 != null)
			{
				scopeTexture = st54;
			}
			else if (Mathf.Round(cmra.GetComponent<Camera>().aspect) == 1 && st43 != null)
			{
				scopeTexture = st43;
			}
		}

		void Update()
		{
			if (!GunScript1.gunActive)
			{
				if (transform.localPosition != hipPosition)
					transform.localPosition = hipPosition;
				if (transform.localEulerAngles != hipRotation)
					transform.localEulerAngles = hipRotation;
				sprinting = false;
				return;
			}

			staticAiming = aiming;

			//Replenish Sprint time
			float tempSprintTime = 0;
			if (controller.velocity.magnitude == 0)
			{
				tempSprintTime = sprintEndTime;
			}
			if (sprintNum < sprintDuration && !sprinting && Time.time > tempSprintTime)
			{
				if (controller.velocity.magnitude == 0)
				{
					sprintNum = Mathf.Clamp(sprintNum + sprintAddStand * Time.deltaTime, 0, sprintDuration);
				}
				else
				{
					sprintNum = Mathf.Clamp(sprintNum + sprintAddWalk * Time.deltaTime, 0, sprintDuration);
				}
			}
			if (sprintNum > sprintMin)
			{
				exhausted = false;
			}

			//Turn on scope if it is time
			if ((inScope && !aiming) || (zoomed && !aiming))
			{
				inScope = false;
				zoomed = false;
				var gos = GetComponentsInChildren<Renderer>();
				foreach (Renderer go in gos)
				{
					if (go.name != "muzzle_flash")
						go.enabled = true;
				}
			}
			//Reset Camera
			if (!aiming && cmra.GetComponent<Camera>().fieldOfView != PlayerWeapons.fieldOfView)
			{
				if (sightsZoom1 && !scoped)
				{
					cmra.GetComponent<Camera>().fieldOfView = Mathf.Lerp(cmra.GetComponent<Camera>().fieldOfView, PlayerWeapons.fieldOfView, Mathf.SmoothStep(0, 1, 1 - (aimStartTime - Time.time) / aimRate));
					wcmra.GetComponent<Camera>().fieldOfView = Mathf.Lerp(wcmra.GetComponent<Camera>().fieldOfView, PlayerWeapons.fieldOfView, Mathf.SmoothStep(0, 1, 1 - (aimStartTime - Time.time) / aimRate));
				}
				else
				{
					cmra.GetComponent<Camera>().fieldOfView = PlayerWeapons.fieldOfView;
					wcmra.GetComponent<Camera>().fieldOfView = PlayerWeapons.fieldOfView;
				}
			}

			staticRate = aimRate;
			//aiming
			if (InputDB.GetButton("Aim") && canAim && PlayerWeapons.canAim && selected && !sprinting /*&& !GunScript1.sprint*/ && Avoidance.canAim)
			{

				if (!aiming)
				{
					aimStartTime = Time.time + aimRate;
					scopeTime = Time.time + aimRate;
					aiming = true;
					canSwitchWeaponAim = false;
					startPosition = transform.localPosition;
					startRotation = transform.localEulerAngles;
					curVect = aimPosition - transform.localPosition;

					player.BroadcastMessage("Aiming", zoomFactor, SendMessageOptions.DontRequireReceiver);
				}

				//Align to position
				GunToRotation(aimRotation, aimRate);
				if (aiming)
				{
					transform.localPosition = Vector3.Slerp(startPosition, aimPosition, Mathf.SmoothStep(0, 1, 1 - (aimStartTime - Time.time) / aimRate));
				}

				//Turn on scope if it's time
				if (scoped && selected && Time.time >= scopeTime && !inScope)
				{
					inScope = true;
					var go = GetComponentsInChildren<Renderer>();
					foreach (Renderer g in go)
					{
						if (g.gameObject.name != "Sparks")
							g.enabled = false;
					}
					cmra.GetComponent<Camera>().fieldOfView = PlayerWeapons.fieldOfView / zoomFactor;
				}

				//Otherwise if sights zoom then zoom in camera
				if (sightsZoom && selected && !zoomed && !scoped)
				{
					cmra.GetComponent<Camera>().fieldOfView = Mathf.Lerp(cmra.GetComponent<Camera>().fieldOfView, PlayerWeapons.fieldOfView / zoomFactor, Mathf.SmoothStep(0, 1, 1 - (aimStartTime - Time.time) / aimRate));
					wcmra.GetComponent<Camera>().fieldOfView = Mathf.Lerp(wcmra.GetComponent<Camera>().fieldOfView, PlayerWeapons.fieldOfView / zoomFactor, Mathf.SmoothStep(0, 1, 1 - (aimStartTime - Time.time) / aimRate));

					if (cmra.GetComponent<Camera>().fieldOfView == PlayerWeapons.fieldOfView / zoomFactor)
					{
						zoomed = true;
					}
				}

				//sprinting
			}
			else if (InputDB.GetButton("Sprint") && !InputDB.GetButton("Aim") && canSprint && PlayerWeapons.canSprint && selected && !aiming && CM.grounded && !exhausted && (controller.velocity.magnitude > CM.movement.minSprintSpeed || (/*CM.prone || */CharacterMotorDB.crouching)))
			{

				sprintNum = Mathf.Clamp(sprintNum - Time.deltaTime, 0, sprintDuration);
				aiming = false;
				if (!sprinting)
				{
					aimStartTime = Time.time + sprintRate;
					curVect = sprintPosition - transform.localPosition;
					sprinting = true;
					player.BroadcastMessage("Sprinting", SendMessageOptions.DontRequireReceiver);
					canSwitchWeaponAim = false;
					startPosition = transform.localPosition;
					startRotation = transform.localEulerAngles;
				}

				//Align to position
				transform.localPosition = Vector3.Slerp(startPosition, sprintPosition, Mathf.SmoothStep(0, 1, 1 - (aimStartTime - Time.time) / sprintRate));
				GunToRotation(sprintRotation, sprintRate);

				//Check if we're out of sprint
				if (sprintNum <= 0)
				{
					exhausted = true;
					sprintEndTime = Time.time + recoverDelay;
				}

				//returning to normal		
			}
			else
			{

				if ((aiming || sprinting || switching))
				{
					if (sprinting)
					{
						sprintEndTime = Time.time + recoverDelay;
						player.BroadcastMessage("StopSprinting", SendMessageOptions.DontRequireReceiver);
					}
					switching = false;
					aimStartTime = Time.time + retRate;
					startPosition = transform.localPosition;
					startRotation = transform.localEulerAngles;
					sprinting = false;
					canSwitchWeaponAim = true;
					curVect = hipPosition - transform.localPosition;

					SendMessageUpwards("NormalSpeed", SendMessageOptions.DontRequireReceiver);
					if (aiming)
					{
						aiming = false;
						player.BroadcastMessage("StopAiming", SendMessageOptions.DontRequireReceiver);
					}
				}

				//Align to position
				transform.localPosition = Vector3.Slerp(startPosition, hipPosition, Mathf.SmoothStep(0, 1, 1 - (aimStartTime - Time.time) / retRate));
				GunToRotation(hipRotation, retRate);
			}
			staticAiming = aiming;
			sprintingPublic = sprinting;
		}

		void DeselectWeapon()
		{
			selected = false;
			inScope = false;
			aiming = false;
		}

		void SelectWeapon()
		{
			selected = true;
			aiming = false;
			SmartCrosshair.displayWhenAiming = crosshairWhenAiming;
		}

		void AimPrimary()
		{
			aimPosition = aimPosition1;
			aimRotation = aimRotation1;
			hipPosition = hipPosition1;
			hipRotation = hipRotation1;
			sprintPosition = sprintPosition1;
			sprintRotation = sprintRotation1;
			curVect = hipPosition - transform.localPosition;
			GetGunScript(0);
			zoomFactor = zoomFactor1;
			scoped = scoped1;
			sightsZoom = sightsZoom1;
			canAim = aim;
			switching = true;
		}

		void AimSecondary()
		{
			aimPosition = aimPosition2;
			aimRotation = aimRotation2;
			hipPosition = hipPosition2;
			hipRotation = hipRotation2;
			sprintPosition = sprintPosition2;
			sprintRotation = sprintRotation2;
			curVect = hipPosition - transform.localPosition;
			GetGunScript(1);
			zoomFactor = zoomFactor2;
			scoped = scoped2;
			sightsZoom = sightsZoom2;
			canAim = secondaryAim;
			switching = true;
		}

		void GetGunScript(int n)
		{
			GunScript[] gunScripts = transform.parent.GetComponents<GunScript>();
			foreach (GunScript gs in gunScripts)
			{
				if (n == 0 && gs.isPrimaryWeapon)
				{
					GunScript1 = gs;
				}
				else if (n == 1 && !gs.isPrimaryWeapon)
				{
					GunScript1 = gs;
				}
			}
		}

		void GunToRotation(Vector3 v3, float rate)
		{
			transform.localEulerAngles = new Vector3(Mathf.LerpAngle(startRotation.x, v3.x, Mathf.SmoothStep(0, 1, 1 - (aimStartTime - Time.time) / rate)), Mathf.LerpAngle(startRotation.y, v3.y, Mathf.SmoothStep(0, 1, 1 - (aimStartTime - Time.time) / rate)), Mathf.LerpAngle(startRotation.z, v3.z, Mathf.SmoothStep(0, 1, 1 - (aimStartTime - Time.time) / rate)));

		}
	}
}