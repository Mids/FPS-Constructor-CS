using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class PlayerWeapons : MonoBehaviour
	{
		public GameObject[] weapons; //Array of equipped weapons (accessible by number keys)
		public int selectedWeapon = 0; //index of currently selected weapon in array
		public bool reloadWhileSprinting = false;
		public float displayTime = 1; //How long slot-related message is shown for
		public float sensitivity = 13; //Sensitivity of mouse
		public bool inverted = false; //Is the mouse y-axis inverted
		public float interactDistance = 5; //How far can an object be to be interacted with
		private Transform lastHit; //the last object we hit
		public LayerMask interactMask; //Mask for raycast

		//Settings
		public static bool autoReload = true; //Do guns automatically reload when emptied?
		public static float fieldOfView = 60; //Base field of view for cameras
		public static int playerLayer = 8;
		public static int ignorePlayerLayer = 8;
		public static bool canSwapSameWeapon = true; //Can we pick up multiples of the same weapon? (Replacing it)
		public static bool saveUpgradesToDrops = true;
		public LayerMask RaycastsIgnore; //Layers that gun raycasts hit


		//Control Variables
		public static bool canMove = true;
		public static bool canSprint = true;
		public static bool canLook = true; //Can the player look around?
		public static bool canFire = true;
		public static bool canAim = true;
		public static bool canCrouch = true;
		public static bool doesIdle = true;
		public static bool canInteract = true;
		public static bool canSwitchWeapon = true;

		//Status
		public static bool hidden = false;
		public static bool sprinting = false;

		//Don't change
		public static GameObject player;
		public static CharacterController controller;
		public static CharacterMotorDB CM;
		public static GameObject weaponCam;
		public static GameObject mainCam;
		public static bool autoFire;
		public static bool charge = false;
		private bool canKickback = true;
		private float emptyMessageTime;
		private string emptyMessage;
		private bool displayMessage = false;
		public static bool playerActive = true;
		public static PlayerWeapons PW; //Singleton object


		void Start()
		{
			// Select the first weapon
			//playerActive = true;
			ActivateWeapon();
		}

		void Awake()
		{
			if (PW)
				Debug.LogError("Too many instances of PlayerWeapons! There should only be one per scene");
			PW = this;
			weaponCam = GameObject.FindWithTag("WeaponCamera");
			mainCam = GameObject.FindWithTag("MainCamera");
			player = GameObject.FindWithTag("Player");
			CM = player.GetComponent<CharacterMotorDB>();
			controller = player.GetComponent<CharacterController>();
			hidden = false;

			SetSensitivity();
		}

		public void SetSensitivity()
		{
			transform.parent.GetComponent<MouseLookDBJS>().sensitivityStandardX = sensitivity;
			if (inverted)
				sensitivity *= -1;
			this.GetComponent<MouseLookDBJS>().sensitivityStandardY = sensitivity;
			sensitivity = Mathf.Abs(sensitivity);
		}

		void LateUpdate()
		{
			if (!playerActive)
				return;
			if (InputDB.GetButtonDown("Fire1") && canKickback)
			{
				canKickback = false;
			}
			else if (InputDB.GetButtonUp("Fire1"))
			{
				canKickback = true;
				gameObject.BroadcastMessage("ReleaseFire", 1, SendMessageOptions.DontRequireReceiver);
			}
			if (weapons[selectedWeapon] != null)
				if ( /*!InputDB.GetButton ("Fire1") || */Time.time > weapons[selectedWeapon].GetComponent<GunScript>().nextFireTime)
				{
					BroadcastMessage("Cooldown");
				}
			if (InputDB.GetButtonUp("Aim"))
			{
				gameObject.SendMessageUpwards("ReleaseFire", 2, SendMessageOptions.DontRequireReceiver);
			}
		}

		void Update()
		{
			if (!playerActive)
				return;

			/************************Interact****************************/
			if (interactDistance > 0)
			{
				//Set up ray
				Ray ray;
				RaycastHit hit;
				ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0));
				if (Physics.Raycast(ray, out hit, interactDistance, interactMask))
				{
					//We hit something new
					if (lastHit != hit.transform)
					{
						//Last object isn't still highlighted
						if (lastHit)
							lastHit.SendMessage("HighlightOff", SendMessageOptions.DontRequireReceiver);
						//New one is
						lastHit = hit.transform;
						lastHit.SendMessage("HighlightOn", SendMessageOptions.DontRequireReceiver);
					}
				}
				else if (lastHit != null)
				{
					//We hit nothing, but still have a object highlighted, so unhighlight it
					lastHit.SendMessage("HighlightOff", SendMessageOptions.DontRequireReceiver);
					lastHit = null;
				}

				//Interact
				if (InputDB.GetButtonDown("Interact") && lastHit != null)
				{
					lastHit.SendMessage("Interact", this.gameObject, SendMessageOptions.DontRequireReceiver);
				}
			}
			/**********************Fire & Reload******************************/

			// Did the user press fire?
			if (InputDB.GetButton("Fire1") && (autoFire || charge) && canFire)
			{
				transform.root.BroadcastMessage("Fire", SendMessageOptions.DontRequireReceiver);
			}
			else if (InputDB.GetButtonDown("Fire1") && canFire)
			{
				transform.root.BroadcastMessage("Fire", SendMessageOptions.DontRequireReceiver);
			}
			if (InputDB.GetButton("Fire2") && canFire && (autoFire || charge))
			{
				BroadcastMessage("Fire2", SendMessageOptions.DontRequireReceiver);
			}
			else if (InputDB.GetButtonDown("Fire2") && canFire)
			{
				BroadcastMessage("Fire2", SendMessageOptions.DontRequireReceiver);
			}

			if (InputDB.GetButtonDown("Reload"))
			{
				BroadcastMessage("Reload", SendMessageOptions.DontRequireReceiver);
			}

			/*************************Weapon Switching***************************/

			if (!AimMode.canSwitchWeaponAim || hidden || !canSwitchWeapon || Avoidance.collided)
			{
				return;
			}

			if (InputDB.GetButtonDown("SelectWeapon"))
			{
				int temp = WeaponSelector.selectedWeapon;
				if (weapons.Length > temp && (selectedWeapon != temp || weapons[selectedWeapon] == null) && temp >= 0)
				{
					if (weapons[temp] != null)
					{
						if (!weapons[temp].gameObject.GetComponent<WeaponInfo>().locked)
						{
							SelectWeapon(temp);
							selectedWeapon = temp;
							displayMessage = false;
						}
						else
						{
							SlotEmptyMessage(temp + 1);
						}
					}
					else
					{
						SlotEmptyMessage(temp + 1);
					}
				}
			}
		}

		public void SelectWeapon(int index)
		{
			bool allNull = true;
			for (int i = 0; i < weapons.Length; i++)
			{
				if (i != index && weapons[i] != null)
				{
					weapons[i].gameObject.BroadcastMessage("DeselectWeapon");
					allNull = false;
				}
			}
			if (allNull)
			{
				ActivateWeapon();
			}
		}

		public void ActivateWeapon()
		{
			if (hidden)
				return;
			if (weapons[selectedWeapon] != null)
			{
				weapons[selectedWeapon].BroadcastMessage("SelectWeapon");
			}
		}

		public void FullAuto()
		{
			autoFire = true;
		}

		public void SemiAuto()
		{
			autoFire = false;
		}

		public void Charge()
		{
			charge = true;
		}

		public void NoCharge()
		{
			charge = false;
		}

		public void DeactivateWeapons()
		{
			for (int i = 0; i < weapons.Length; i++)
			{
				if (weapons[i] != null)
					weapons[i].gameObject.BroadcastMessage("DeselectWeapon");
			}
		}

		public void SetWeapon(GameObject gun, int element)
		{
			weapons[element] = gun;
		}

		public void SlotEmptyMessage(int s)
		{
			//display message
			displayMessage = true;
			emptyMessageTime = Time.time + displayTime;
			emptyMessage = "No Weapon Equipped in Slot " + s;
		}

		void OnGUI()
		{
			if (Time.time < emptyMessageTime && displayMessage)
			{
				GUI.BeginGroup(new Rect(Screen.width / 2 - 120, Screen.height - 60, 240, 60), "");
				GUI.Box(new Rect(0, 0, 240, 60), "");
				GUI.Label(new Rect(20, 20, 200, 20), emptyMessage);
				GUI.EndGroup();
			}
			else
			{
				displayMessage = false;
			}
		}

		//Hides Player's weapon, with put away animation
		public static void HideWeapon()
		{
			hidden = true;
			if (PW.weapons[PW.selectedWeapon] != null)
				PW.weapons[PW.selectedWeapon].gameObject.BroadcastMessage("DeselectWeapon");
			SmartCrosshair.crosshair = true;
		}

		//Hides Player's weapon instantly
		public static void HideWeaponInstant()
		{
			hidden = true;
			if (PW.weapons[PW.selectedWeapon] != null)
				PW.weapons[PW.selectedWeapon].gameObject.BroadcastMessage("DeselectInstant");
			SmartCrosshair.crosshair = true;
		}

		//Unhides Player's weapon
		public static void ShowWeapon()
		{
			hidden = false;
			if (PW.weapons[PW.selectedWeapon] != null)
				PW.weapons[PW.selectedWeapon].gameObject.BroadcastMessage("SelectWeapon");
		}

		public static int HasEquipped()
		{
			int num = 0;
			for (int i = 0; i < PW.weapons.Length; i++)
			{
				if (PW.weapons[i] != null)
					num++;
			}
			return num;
		}
	}
}