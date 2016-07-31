using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	public class GunScript : MonoBehaviour
	{
		float kickbackAngle; //Vertical kickback per shot (degrees)
		float xKickbackFactor = .5f; //Horizontal kickback per shot (percent of vertical)
		float maxKickback = 15; //Maximum vertical kickback (degrees)
		float kickbackAim;
		float crouchKickbackMod = .6f;
		float proneKickbackMod = .35f;
		float moveKickbackMod = 1.3f;

		private float curKickback;
		float recoilDelay = .11f; //Delay between stopping firing and recoil decreasing

		/*Spread variables: Spread (between 0 and 1) determines the accuracy of the weapon.
		A spread of 0 means that the weapon will fire exactly towards the crosshair, and
		a spread of 1 means that it will fire anywhere within 90 degrees of the crosshair.
		*/
		float standardSpread = .1f; //default spread of this weapon
		float maxSpread = .25f; //Maximum spread of this weapon
		float crouchSpreadModifier = .7f; //When crouching, spread is multiplied by this
		float proneSpreadModifier = .4f; //When prone, spread is multiplied by this
		float moveSpreadModifier = 1.5f; //When walking, spread is multiplied by this
		float standardSpreadRate = .05f; //Standard increase in spread per shot
		float aimSpreadRate = .01f; //Increase in spread per shot when aiming
		float aimSpread = .01f; //Default spread when aiming
		float spDecRate = .05f; //Speed at which spread decreases when not firing


		////////// Ammo variables //////////
		float ammoLeft = 0; //Ammo left in the curent clip (ammo before next reload)
		int ammoPerClip = 40; //Shots per clip
		int ammoPerShot = 1; //Ammo used per shot
		int clips = 20; //Number of spare clips (reloads) left
		int maxClips = 20; //Maximum number of clips
		bool infiniteAmmo = false; //Does this gun deplete clips whe reoading?
		enum ammoTypes { byClip, byBullet }
		ammoTypes ammoType = ammoTypes.byClip; //Does this weapon conserve ammo when reloading? (e.g. if you reload after shooting one bullet, does it use a whole clip)


		////////// Fire Variables //////////
		AudioClip fireSound; //Sound that plays when firing
		float fireVolume = 1;
		float firePitch = 1;//Pitch of fire sound
		float fireRate = 0.05f; //Time in seconds between shots
		bool autoFire; //Is this weapon automatic (can you hold down the fre button?)
		bool fireAnim = false; //Does this weapon's fire animation scale to fit the fire rate (generally used for non-automatic weapons)
		float delay = 0; //Delay between hitting fire button and actually firing (can be used to sync firing with animation)
		AudioClip emptySound; //Sound that plays when firing
		float emptyVolume = 1;
		float emptyPitch = 1;//Pitch of fire sound

		//Burst fire
		//note: burst fire doesn't work well with automatic weapons
		bool burstFire = false; //does this wepon fire in bursts
		int burstCount = 1; //shots per burst
		float burstTime = .5f; //time to fire full burst


		////////// Reloading variables //////////
		float reloadTime = 0.5f;
		float emptyReloadTime = .4f;
		bool addOneBullet = false;
		float waitforReload = 0.00f;

		/*Progressive Reload is a different kind of reloading where the reload is broken into stages.
		The first stage initializes the animation to get to the second stage. The second stage represents
		reloading one shot, and will repeat as many times as necessary to reach a full clip unless
		interrupted. Then the third stage returns the weapon to its standrad position. This is useful
		for weapons like shotguns that reload by shells.
		*/
		bool progressiveReload = false; //Does this weapon use progressive reload?
		bool progressiveReset = false; //Does this weapon's ammo reset to 0 when starting a reload? 
		//(e.g. a revolver where the shells are discarded and replaced)
		float reloadInTime = 0.5f; //time in seconds for the first stage
		float reloadOutTime = 0.5f; //time in seconds for the third stage
		//the time for the second stage is just reloadTime


		////////// Gun-Specific Variables //////////
		float range = 100.0f; //Range of bullet raycast in meters
		float force = 10.0f; //Force of bullet
		float damage = 5.0f; //Damage per bullet
		int shotCount = 6; //Bullets per shot
		int penetrateVal = 1; //penetration level of bullet

		/* Damage falloff allows raycast weapons to do less damage at long distances
		*/
		bool hasFalloff = false; //Does this weapon use damage falloff?
		float minFalloffDist = 10; //Distance at which falloff begins to take effect
		float maxFalloffDist = 100; //Distance at which falloff stops (minumum damage)
		float falloffCoefficient = 1; //Coefficient for multiplying damage
		float falloffDistanceScale = 4; //Scaling value to change speed of falloff


		////////// Launcher-Specific Variables //////////
		Rigidbody projectile; //The object to launch. This can be anything, as long as it has a rigidbody.
		float initialSpeed = 20.0f; //Initial speed of projectile, applied  forward
		int projectileCount = 1; //Number of projectiles fired
		GameObject launchPosition; //GameObject whose position the projectile is fired from (place this at the end of the weapon's barrel, generally)


		////////// Tracer related variables //////////
		/* Tracers are done using particle emitters.
		*/
		GameObject tracer; //Tracer object. Must have a particle emitter attached
		int traceEvery = 0; //Activate a tracer evey x shots.
		float simulateTime = .02f; //How long to simulate tracer before it appears
		float minDistForTracer = 2; //This isn't exposed, but can be tweaked if needed


		////////// Sway //////////
		bool sway; //Does the weapon sway?
		Vector2 moveSwayRate = new Vector2(2.5f, 5); //How fast does the weapon sway when walking? (xy)
		Vector2 moveSwayAmplitude = new Vector2(.04f, .01f); //How much does the weapon sway when walking? (xy)
		Vector2 runSwayRate = new Vector2(4.5f, .9f); //How fast does the weapon sway when sprinting? (xy)
		Vector2 runAmplitude = new Vector2(.04f, .04f); //How much does the weapon sway when sprinting? (xy)
		Vector2 idleSwayRate = new Vector2(2, 1); //How fast does the weapon sway when standing? (xy)
		Vector2 idleAmplitude = new Vector2(.002f, .001f); //How much does the weapon sway when standing? (xy)


		////////// Secondary Weapons //////////
		GunScript secondaryWeapon; //Gunscript of secondary weapon (additional weapon script on this object)
		bool secondaryInterrupt = false; //Can primary and secondary weapon interrupt each other's actions
		bool secondaryFire = false; //Is the secondary weapon fired with Mouse2?
		float enterSecondaryTime = .5f; //How long does it take to switch to secondary (animation)?
		float exitSecondaryTime = .5f; //How long does it take to switch from secondary (animation)?


		////////// Charge weapon variables //////////
		float minCharge = 0; //Minimum charge value at ahich the weapon can fire
		float maxCharge = 10; // Maximum charge value the weapon can have
		float chargeLevel = 0; //current charge level of the weapon
		bool forceFire = false; //Does this weapon have to fire when it hits max charge?
		AudioClip chargeLoop; //Sound to play when charging
		bool chargeAuto = false; //Does the weapon automatically start charging again after a forced release?


		//Specifically for hitscan charge weapons
		float chargeCoefficient = 1.1f; //Damage multiplier as charge increases
		float additionalAmmoPerCharge = 0; //Ammo change as charge increases (add this per 1 charge level)


		//////////Other variables//////////
		float idleTime = 0; //Time in seconds that the player has been idle
		float timeToIdle = 7; //Time in seconds of being idle which will cause the idle animation to play
		float takeOutTime = .6f; //Time to take out (switch to) weapon
		float putAwayTime = .6f; //Time to put away (switch from) weapon 

		//////////Z KickBack//////////
		bool useZKickBack = true; //Does this weapon use z kickback?
		float kickBackZ = 2; //Rate of z kickback when firing
		float zRetRate = 1; //rate of return from z when not firing
		float maxZ = .3f; //maximum z kickback

		//////////Avoidance//////////
		//Avoidance is by default handled globall by the Avoidance Component. This just overrides its values for this weapon.
		bool overrideAvoidance = false; //Does this weapon override global object avoidance values
		bool avoids = true;
		Vector3 rot;
		Vector3 pos;
		float dist = 2;
		float minDist = 1.5f;

		//Shell Ejection
		bool shellEjection = false; //Does this weapon use shell ejection?
		GameObject ejectorPosition; //If it does, this gameobject provides the position where shells are instantiated
		float ejectDelay = 0;
		GameObject shell; //The shell prefab to instantiate


		//Custom crosshair variables
		bool scale = false; //Does the crosshair scale with accuracy?
		GameObject crosshairObj; //Crosshair object to use
		float crosshairSize; //Default scale of the crosshair object

		///////////////////////// END CHANGEABLE BY USER /////////////////////////



		///////////////////////// Internal Variables /////////////////////////
		/*These variables should not be modified directly, weither because it could compromise
		the functioning of the package, or because changes will be overwritten.
		*/


		public bool gunActive = false; // Is the weapon currently selected & activated


		//Status
		private bool interruptPutAway = false;
		private bool progressiveReloading = false;
		bool inDelay = false;
		private int m_LastFrameShot = -1;
		public bool reloading = false;
		float nextFireTime = 0.0f;
		static bool takingOut = false;
		static bool puttingAway = false;

		bool secondaryActive = false;
		static float crosshairSpread = 0;
		private float shotSpread;
		private float actualSpread;
		private float spreadRate = .05f;
		public bool isPrimaryWeapon = true;
		bool aim = false;
		bool aim2 = false;
		private float pReloadTime = 0;
		private bool stopReload = false;
		private Vector3 startPosition;
		bool gunDisplayed;
		private float totalKickBack; //How far have we kicked back?

		//Components
		AmmoDisplay ammo;
		SprintDisplay sprint;
		WeaponDisplay wepDis;
		static GameObject mainCam;
		static GameObject weaponCam;
		private GunScript primaryWeapon;
		private GameObject player;
		AimMode aim1;
		MouseLookDBJS mouseY;
		MouseLookDBJS mouseX;
		bool reloadCancel = false;


		////////// Spray //////////
		/* Spray weapons are meant to be a simple solution for something that can now be done better with a 
		charge weapon.
		*/
		private float tempAmmo = 1;
		bool sprayOn = false;
		GameObject sprayObj;
		SprayScript sprayScript;
		float deltaTimeCoefficient = 1;
		float forceFalloffCoefficient = .99f;
		AudioClip loopSound;
		AudioClip releaseSound;
		float ammoPerSecond;

		////////// Charge weapon variables //////////
		bool chargeWeapon = false; //Is this weapon a charge weapon?
		bool chargeReleased = false;
		bool chargeLocked = false;

		//Gun Types
		enum gunTypes { hitscan, launcher, melee, spray }
		gunTypes gunType = gunTypes.hitscan;

		//Melee
		bool hitBox = false;

		//Tracer related variables
		private int shotCountTracer = 0;

		//Ammo Sharing
		bool sharesAmmo = false;
		bool shareLoadedAmmo = false;
		int ammoSetUsed = 0;
		GameObject managerObject;
		AmmoManager ammoManagerScript;

		//Effects
		EffectsManager effectsManager;
		CharacterMotorDB CM;

		//Inspector only variables
		bool shotPropertiesFoldout = false;
		bool firePropertiesFoldout = false;
		bool accuracyFoldout = false;
		bool altFireFoldout = false;
		bool ammoReloadFoldout = false;
		bool audioVisualFoldout = false;

		//Sway (Internal)
		float swayStartTime = 0;
		Vector2 swayRate;
		Vector2 swayAmplitude;
		bool overwriteSway = false;

		private bool airborne = false;

		void Awake()
		{
			//if(gunActive){
			Renderer[] gos = GetComponentsInChildren<Renderer>();
			foreach (Renderer go in gos)
			{
				go.enabled = false;
			}
			gunActive = false;
			//}
			startPosition = transform.localPosition;
			if (gunType == gunTypes.spray)
			{
				if (sprayObj)
				{
					sprayScript = sprayObj.GetComponent<SprayScript>();
					sprayScript.isActive = false;
				}
				else
				{
					Debug.LogWarning("Spray object is undefined; all spray weapons must have a spray object!");
				}
			}
			crosshairSpread = 0;
			managerObject = GameObject.FindWithTag("Manager");
			ammoManagerScript = managerObject.GetComponent<AmmoManager>();
			effectsManager = managerObject.GetComponent<EffectsManager>();
			aim1 = this.GetComponentInChildren<AimMode>();
			ammo = this.GetComponent<AmmoDisplay>();
			sprint = aim1.GetComponent<SprintDisplay>();
			wepDis = this.GetComponent<WeaponDisplay>();
			ammo.enabled = false;
			sprint.enabled = false;
			wepDis.enabled = false;
		}

		void Start()
		{
			mainCam = PlayerWeapons.mainCam;
			weaponCam = PlayerWeapons.weaponCam;
			player = PlayerWeapons.player;
			CM = player.GetComponent<CharacterMotorDB>();
			mouseY = weaponCam.GetComponent<MouseLookDBJS>();
			mouseX = player.GetComponent<MouseLookDBJS>();

			if (maxSpread > 1)
				maxSpread = 1;


			inDelay = false;
			hitBox = false;

			if (sharesAmmo)
			{
				clips = ammoManagerScript.clipsArray[ammoSetUsed];
				maxClips = ammoManagerScript.maxClipsArray[ammoSetUsed];
				infiniteAmmo = ammoManagerScript.infiniteArray[ammoSetUsed];
			}

			if (!isPrimaryWeapon)
			{
				gunActive = false;
				GunScript[] wpns = this.GetComponents<GunScript>();
				for (int p = 0; p < wpns.Length; p++)
				{
					GunScript g = wpns[p] as GunScript;
					if (g.isPrimaryWeapon)
					{
						primaryWeapon = g;
					}
				}
			}

			if (!overwriteSway && sway)
			{
				CamSway overSway = CamSway.singleton;
				runSwayRate = overSway.runSwayRate;
				moveSwayRate = overSway.moveSwayRate;
				idleSwayRate = overSway.idleSwayRate;
			}

			curKickback = kickbackAngle;
			shotSpread = standardSpread;
			spreadRate = standardSpreadRate;
			ammoLeft = ammoPerClip;
			SendMessage("UpdateAmmo", ammoLeft, SendMessageOptions.DontRequireReceiver);
			SendMessage("UpdateClips", clips, SendMessageOptions.DontRequireReceiver);
			swayRate = moveSwayRate;
			swayAmplitude = moveSwayAmplitude;
		}

		void Aiming()
		{
			idleTime = 0;
			shotSpread = aimSpread;
			spreadRate = aimSpreadRate;
			curKickback = kickbackAim;
			if (CharacterMotorDB.crouching)
				Crouching();
			if (CharacterMotorDB.prone)
				Prone();
			if (CharacterMotorDB.walking)
				Walking();
			if (!CM.grounded)
				Airborne();
		}

		void Crouching()
		{
			if (aim1.aiming)
			{
				spreadRate = aimSpreadRate * crouchSpreadModifier;
				shotSpread = Mathf.Max(aimSpread * crouchSpreadModifier, shotSpread);
				curKickback = kickbackAim * crouchKickbackMod;
			}
			else
			{
				curKickback = kickbackAngle * crouchKickbackMod;
				spreadRate = standardSpreadRate * crouchSpreadModifier;
				shotSpread = Mathf.Max(standardSpread * crouchSpreadModifier, shotSpread);
			}

		}

		void Prone()
		{
			if (aim1.aiming)
			{
				curKickback = kickbackAim * proneKickbackMod;
				spreadRate = aimSpreadRate * proneSpreadModifier;
				shotSpread = Mathf.Max(aimSpread * proneSpreadModifier, shotSpread);
			}
			else
			{
				curKickback = kickbackAngle * proneKickbackMod;
				spreadRate = standardSpreadRate * proneSpreadModifier;
				shotSpread = Mathf.Max(standardSpread * proneSpreadModifier, shotSpread);
			}
		}

		void Walking()
		{
			if (aim1.aiming)
			{
				curKickback = kickbackAim * moveKickbackMod;
				spreadRate = aimSpreadRate * moveSpreadModifier;
				shotSpread = Mathf.Max(aimSpread * moveSpreadModifier, shotSpread);
			}
			else
			{
				curKickback = kickbackAngle * moveKickbackMod;
				spreadRate = standardSpreadRate * moveSpreadModifier;
				shotSpread = Mathf.Max(standardSpread * moveSpreadModifier, shotSpread);
			}
		}

		void StopWalking()
		{
			if (airborne)
				return;
			spreadRate = standardSpreadRate;
			curKickback = kickbackAngle;
			if (shotSpread < standardSpread)
				shotSpread = standardSpread;
			if (aim1.aiming)
			{
				curKickback = kickbackAim;
				spreadRate = aimSpreadRate;
				shotSpread = aimSpread;
			}
		}

		void Landed()
		{
			airborne = false;
			spreadRate = standardSpreadRate;
			curKickback = kickbackAngle;
			if (shotSpread < standardSpread)
				shotSpread = standardSpread;
			if (aim1.aiming)
			{
				curKickback = kickbackAim;
				spreadRate = aimSpreadRate;
				shotSpread = aimSpread;
			}
		}

		void Airborne()
		{
			airborne = true;
			if (aim1.aiming)
			{
				curKickback = kickbackAim * moveKickbackMod;
				spreadRate = aimSpreadRate * moveSpreadModifier;
				shotSpread = Mathf.Max(aimSpread * moveSpreadModifier, shotSpread);
			}
			else
			{
				curKickback = kickbackAngle * moveKickbackMod;
				spreadRate = standardSpreadRate * moveSpreadModifier;
				shotSpread = Mathf.Max(standardSpread * moveSpreadModifier, shotSpread);
			}
		}

		void StopAiming()
		{
			idleTime = 0;
			shotSpread = standardSpread;
			spreadRate = standardSpreadRate;
			curKickback = kickbackAngle;
			if (CharacterMotorDB.crouching)
				Crouching();
			if (CharacterMotorDB.prone)
				Prone();
			if (CharacterMotorDB.walking)
				Walking();
			if (!CM.grounded)
				Airborne();
		}
		void Cooldown()
		{
			if (!gunActive)
				return;
			ReturnKickBackZ();
			float targ;
			if (aim1.aiming)
			{
				targ = aimSpread;
			}
			else
			{
				targ = standardSpread;
			}
			if (CharacterMotorDB.crouching)
				targ *= crouchSpreadModifier;
			if (CharacterMotorDB.prone)
				targ *= proneSpreadModifier;
			if (CharacterMotorDB.walking || !CM.grounded)
				targ *= moveSpreadModifier;
			shotSpread = Mathf.Clamp(shotSpread - spDecRate * Time.deltaTime, targ, maxSpread);
		}

		void Update()
		{
			if (progressiveReloading)
			{
				if (ammoLeft < ammoPerClip && clips >= 1 && !stopReload)
				{
					if (Time.time > pReloadTime)
					{
						BroadcastMessage("ReloadAnim", reloadTime);
						pReloadTime = Time.time + reloadTime;
						ammoLeft++;
						clips--;
						SendMessage("UpdateAmmo", ammoLeft, SendMessageOptions.DontRequireReceiver);
						SendMessage("UpdateClips", clips, SendMessageOptions.DontRequireReceiver);
					}
				}
				else if (Time.time > pReloadTime)
				{
					progressiveReloading = false;
					PlayerWeapons.autoFire = autoFire;
					stopReload = false;
					BroadcastMessage("ReloadOut", reloadOutTime);
					reloading = false;
					if (aim)
					{
						aim1.canAim = true;
					}
					aim1.canSprint = true;
					//aim1.canSwitchWeaponAim = true;
					ApplyToSharedAmmo();
				}
			}

			if (actualSpread != shotSpread)
			{
				if (actualSpread > shotSpread)
				{
					actualSpread = Mathf.Clamp(actualSpread - Time.deltaTime / 4, shotSpread, maxSpread);
				}
				else
				{
					actualSpread = Mathf.Clamp(actualSpread + Time.deltaTime / 4, 0, shotSpread);
				}
			}

			if (gunActive)
			{
				idleTime += Time.deltaTime;
				if (!PlayerWeapons.autoFire && autoFire)
				{
					SendMessageUpwards("FullAuto");
				}
				if (PlayerWeapons.autoFire && !autoFire)
				{
					SendMessageUpwards("SemiAuto");
				}
				if (!PlayerWeapons.charge && chargeWeapon)
				{
					SendMessageUpwards("Charge");
				}
				if (PlayerWeapons.charge && !chargeWeapon)
				{
					SendMessageUpwards("NoCharge");
				}
			}
		}

		void LateUpdate()
		{

			if (InputDB.GetButtonDown("Fire2") && secondaryWeapon != null && !secondaryFire && !aim1.aiming && !Avoidance.collided)
			{
				if (!secondaryWeapon.gunActive)
				{
					ActivateSecondary();
				}
				else if (secondaryWeapon.gunActive)
				{
					ActivatePrimary();
				}
			}

			if (gunActive)
			{
				if (idleTime > timeToIdle)
				{
					if (!aim1.aiming && !Avoidance.collided)
					{
						BroadcastMessage("IdleAnim", SendMessageOptions.DontRequireReceiver);
					}
					idleTime = 0;
				}
				shotSpread = Mathf.Clamp(shotSpread, 0, maxSpread);
				crosshairSpread = actualSpread * 180 / weaponCam.GetComponent<Camera>().fieldOfView * Screen.height;
			}
			else
			{
				return;
			}

			if (CharacterMotorDB.walking && !aim1.aiming && sway && !CharacterMotorDB.paused)
			{
				//if(swayStartTime > Time.time)
				//	swayStartTime = Time.time;
				WalkSway();
				idleTime = 0;
			}
			else
			{
				//swayStartTime = 999999999999999999;
				ResetPosition();
			}

			if (chargeLevel > 0 && chargeWeapon)
			{
				if (GetComponent<AudioSource>().clip != chargeLoop || !GetComponent<AudioSource>().isPlaying)
				{
					GetComponent<AudioSource>().clip = chargeLoop;
					GetComponent<AudioSource>().loop = true;
					GetComponent<AudioSource>().Play();
				}
			}
			else
			{
				if (GetComponent<AudioSource>().clip == chargeLoop)
				{
					GetComponent<AudioSource>().Stop();
				}
			}

			// We shot this frame, enable the muzzle flash
			if (m_LastFrameShot == Time.frameCount)
			{
			}
			else
			{
				// Play sound
				if (GetComponent<AudioSource>())
				{
					GetComponent<AudioSource>().loop = false;
				}
			}
		}

		void FireAlt()
		{
			if (!isPrimaryWeapon)
			{
				AlignToSharedAmmo();
				gunActive = true;
				Fire();
				gunActive = false;
			}
		}

		void AlignToSharedAmmo()
		{
			if (sharesAmmo)
			{
				clips = ammoManagerScript.clipsArray[ammoSetUsed];
				maxClips = ammoManagerScript.maxClipsArray[ammoSetUsed];
				infiniteAmmo = ammoManagerScript.infiniteArray[ammoSetUsed];
				SendMessage("UpdateAmmo", ammoLeft, SendMessageOptions.DontRequireReceiver);
				SendMessage("UpdateClips", clips, SendMessageOptions.DontRequireReceiver);
			}
		}

		void ApplyToSharedAmmo()
		{
			if (sharesAmmo)
			{
				ammoManagerScript.clipsArray[ammoSetUsed] = clips;
				ammoManagerScript.maxClipsArray[ammoSetUsed] = maxClips;
				ammoManagerScript.infiniteArray[ammoSetUsed] = infiniteAmmo;
			}
		}

		void Fire2()
		{
			if (isPrimaryWeapon && secondaryWeapon != null && gunActive && secondaryFire)
			{
				ApplyToSharedAmmo();
				secondaryWeapon.FireAlt();
			}
		}

		public void Fire()
		{
			StartCoroutine(FireRoutine());
		}

		IEnumerator FireRoutine()
		{
			idleTime = 0;
			if (!gunActive || aim1.sprinting || inDelay || LockCursor.unPaused)
			{
				if ((gunType == gunTypes.spray) && sprayOn)
				{
					if (GetComponent<AudioSource>())
					{
						if (GetComponent<AudioSource>().clip == loopSound)
						{
							GetComponent<AudioSource>().Stop();
						}
						sprayOn = false;
						sprayScript.ToggleActive(false);
					}
				}
				yield break;
			}
			//Melee attack
			if (gunType == gunTypes.melee && nextFireTime < Time.time)
			{
				BroadcastMessage("FireMelee", delay, SendMessageOptions.DontRequireReceiver);
				nextFireTime = Time.time + fireRate;
				inDelay = true;
				hitBox = true;
				GetComponent<AudioSource>().clip = fireSound;
				GetComponent<AudioSource>().Play();
				yield return new WaitForSeconds(delay);
				inDelay = false;
				if (reloadTime > 0)
					BroadcastMessage("ReloadMelee", reloadTime, SendMessageOptions.DontRequireReceiver);
				hitBox = false;
				yield break;
			}

			//Prog reload cancel
			if (progressiveReloading && ammoLeft > 0)
			{
				stopReload = true;
			}

			int b = 1; //variable to control burst fire

			//Can we fire?
			if ((ammoLeft < ammoPerShot) || (nextFireTime > Time.time) || !gunActive || reloading || Avoidance.collided)
			{
				if (ammoLeft < ammoPerShot && !((nextFireTime > Time.time) || !gunActive || reloading || Avoidance.collided))
				{
					if (PlayerWeapons.autoReload && clips > 0)
					{
						Reload();
					}
					else
					{
						if (isPrimaryWeapon)
						{
							BroadcastMessage("EmptyFireAnim");
						}
						else
						{
							BroadcastMessage("SecondaryEmptyFireAnim");
						}
						nextFireTime = Time.time + 0.3f;
					}
					if (!reloading)
					{
						PlayerWeapons.autoFire = false;
						GetComponent<AudioSource>().pitch = emptyPitch;
						GetComponent<AudioSource>().volume = emptyVolume;
						GetComponent<AudioSource>().clip = emptySound;
						GetComponent<AudioSource>().Play();
					}
				}
				if (gunType == gunTypes.spray)
				{
					sprayOn = false;
					sprayScript.ToggleActive(false);
				}
				yield break;
			}

			//KickBack
			KickBackZ();

			if (gunType != gunTypes.spray)
			{

				//Handle charging
				if (chargeWeapon)
				{
					if (chargeLevel < maxCharge && !chargeLocked && gunActive)
					{
						if (ammoPerShot + additionalAmmoPerCharge * chargeLevel >= ammoLeft && additionalAmmoPerCharge != 0)
						{
							chargeReleased = true;
							chargeLocked = true;
						}
						else
						{
							chargeLevel += Time.deltaTime;
						}
					}
					else if (forceFire && chargeLocked == false)
					{
						chargeReleased = true;
						if (!chargeAuto)
						{
							chargeLocked = true;
						}
					}
				}

				//Handle firing
				if (!chargeWeapon || (chargeWeapon && chargeReleased))
				{
					if (chargeWeapon)
					{
						chargeReleased = false;
					}
					if (burstFire)
					{
						b = burstCount;
					}
					else
					{
						b = 1;
					}
					for (int i = 0; i < b; i++)
					{
						if (ammoLeft >= ammoPerShot)
						{
							FireShot();
							if (chargeWeapon)
							{
								ammoLeft -= ammoPerShot + Mathf.Floor(additionalAmmoPerCharge * chargeLevel);
							}
							else
							{
								ammoLeft -= ammoPerShot;
							}
							if (fireRate < delay)
							{
								nextFireTime = Time.time + delay;
							}
							else
							{
								nextFireTime = Time.time + fireRate;
							}
							if (secondaryWeapon != null && secondaryFire && !secondaryWeapon.secondaryInterrupt)
							{
								if (fireRate < delay)
								{
									secondaryWeapon.nextFireTime = Time.time + delay;
								}
								else
								{
									secondaryWeapon.nextFireTime = Time.time + fireRate;
								}
							}
							else if (secondaryFire && !secondaryInterrupt && !isPrimaryWeapon)
							{
								if (fireRate < delay)
								{
									primaryWeapon.nextFireTime = Time.time + delay;
								}
								else
								{
									primaryWeapon.nextFireTime = Time.time + fireRate;
								}

							}
							if (burstFire && i < (b - 1))
							{
								if (burstTime / burstCount < delay)
								{
									yield return new WaitForSeconds(delay);
								}
								else
								{
									yield return new WaitForSeconds(burstTime / burstCount);
								}
							}
						}
					}
				}
			}
			else if (gunType == gunTypes.spray)
			{
				FireSpray();
			}
			ApplyToSharedAmmo();

			SendMessage("UpdateAmmo", ammoLeft, SendMessageOptions.DontRequireReceiver);
			SendMessage("UpdateClips", clips, SendMessageOptions.DontRequireReceiver);
			if (ammoLeft <= 0 && PlayerWeapons.autoReload)
			{
				if (fireRate < delay)
				{
					yield return new WaitForSeconds(delay);
				}
				else
				{
					yield return new WaitForSeconds(fireRate);
				}
				Reload();
			}
		}


		//Kickback function which moves the gun transform backwards when called
		void KickBackZ()
		{
			if (!useZKickBack)
				return;
			float amt = Time.deltaTime * kickBackZ;
			amt = Mathf.Min(amt, maxZ - totalKickBack);
			transform.localPosition = transform.localPosition - new Vector3(0, 0, amt);
			totalKickBack += amt;
		}

		//Reset Kickback function which moves the gun transform forwards when called
		void ReturnKickBackZ()
		{
			float amt = Time.deltaTime * zRetRate;
			amt = Mathf.Min(amt, totalKickBack);
			transform.localPosition = transform.localPosition + new Vector3(0, 0, amt);
			totalKickBack -= amt;
		}

		public void FireShot()
		{
			StartCoroutine(FireShotRoutine());
		}
		IEnumerator FireShotRoutine()
		{
			if (isPrimaryWeapon)
			{
				if (fireAnim && !autoFire && !burstFire)
				{
					BroadcastMessage("SingleFireAnim", fireRate, SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					BroadcastMessage("FireAnim", SendMessageOptions.DontRequireReceiver);
				}
			}
			else
			{
				if (fireAnim && !autoFire && !burstFire)
				{
					BroadcastMessage("SingleSecFireAnim", fireRate, SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					BroadcastMessage("SecondaryFireAnim", SendMessageOptions.DontRequireReceiver);
				}
			}
			if (shellEjection && !aim1.inScope)
				EjectShell();
			if (gunType == gunTypes.hitscan)
			{
				inDelay = true;
				yield return new WaitForSeconds(delay);
				inDelay = false;
				for (int i = 0; i < shotCount; i++)
				{
					FireOneBullet();
					Kickback();
				}
			}
			else if (gunType == gunTypes.launcher)
			{
				inDelay = true;
				yield return new WaitForSeconds(delay);
				inDelay = false;
				for (int p = 0; p < projectileCount; p++)
				{
					FireOneProjectile();
				}
			}
			m_LastFrameShot = Time.frameCount;
			shotSpread = Mathf.Clamp(shotSpread + spreadRate, 0, maxSpread);
			chargeLevel = 0;
			FireEffects();
		}

		void FireOneProjectile()
		{
			Vector3 direction = SprayDirection();
			Quaternion convert = Quaternion.LookRotation(direction);
			/*FIXME_VAR_TYPE layer1= 1 << PlayerWeapons.playerLayer;
			FIXME_VAR_TYPE layer2= 1 << 2;
			FIXME_VAR_TYPE layerMask= layer1 | layer2;
			layerMask = ~layerMask;*/
			Rigidbody instantiatedProjectile;
			Transform launchPos;
			if (launchPosition != null && !Physics.Linecast(launchPosition.transform.position, weaponCam.transform.position, ~(PlayerWeapons.PW.RaycastsIgnore.value)))
			{
				launchPos = launchPosition.transform;
			}
			else
			{
				launchPos = weaponCam.transform;
			}
			instantiatedProjectile = (Instantiate(projectile, launchPos.position, convert) as GameObject).GetComponent<Rigidbody>();
			instantiatedProjectile.velocity = instantiatedProjectile.transform.TransformDirection(new Vector3(0, 0, initialSpeed));
			instantiatedProjectile.transform.rotation = launchPos.transform.rotation;
			Physics.IgnoreCollision(instantiatedProjectile.GetComponent<Collider>(), transform.root.GetComponent<Collider>());
			Physics.IgnoreCollision(instantiatedProjectile.GetComponent<Collider>(), player.transform.GetComponent<Collider>());
			instantiatedProjectile.BroadcastMessage("ChargeLevel", chargeLevel, SendMessageOptions.DontRequireReceiver);
			Kickback();
		}

		void FireOneBullet()
		{
			bool penetrate = true;
			int pVal = penetrateVal;
			/*FIXME_VAR_TYPE layer1= 1 << PlayerWeapons.playerLayer;
			FIXME_VAR_TYPE layer2= 1 << 2;
			FIXME_VAR_TYPE layerMask= layer1 | layer2;
			layerMask = ~layerMask;*/
			RaycastHit[] hits;
			//FIXME_VAR_TYPE direction= SprayDirection();
			Ray ray = mainCam.GetComponent<Camera>().ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
			ray.direction = SprayDirection(ray.direction);
			hits = Physics.RaycastAll(ray, range, ~PlayerWeapons.PW.RaycastsIgnore.value);

			//Tracer
			shotCountTracer += 1;
			if (tracer != null && traceEvery <= shotCountTracer && traceEvery != 0)
			{
				ParticleEmitter emitter = tracer.GetComponent<ParticleEmitter>();
				shotCountTracer = 0;
				/*if(hits.length > 0){
					if(Vector3.Distance(hits[0].point, transform.position) >= minDistForTracer){
						tracer.transform.LookAt(hits[0].point);
						emitter.Emit(); //This code is written twice because if there is a hit it may not happen
						emitter.GetComponent<ParticleEmitter>().Simulate(simulateTime);
					}
				}else{*/
				tracer.transform.rotation = Quaternion.LookRotation(ray.direction);//(transform.position + 90 * direction));
				emitter.Emit();
				emitter.Simulate(simulateTime);
				//}
			}
			System.Array.Sort(hits, Comparison);

			//	 Did we hit anything?
			for (int i = 0; i < hits.Length; i++)
			{
				RaycastHit hit = hits[i];
				BulletPenetration BP = hit.transform.GetComponent<BulletPenetration>();
				if (penetrate)
				{
					if (BP == null)
					{
						penetrate = false;
					}
					else
					{
						if (pVal < BP.penetrateValue)
						{
							penetrate = false;
						}
						else
						{
							pVal -= BP.penetrateValue;
						}
					}

					//Apply charge if applicable
					float chargedDamage = damage;
					if (chargeWeapon)
					{
						chargedDamage = damage * Mathf.Pow(chargeCoefficient, chargeLevel);
					}

					//Calculate damage falloff
					float finalDamage = chargedDamage;
					float hitDist = 0;
					if (hasFalloff)
					{
						hitDist = Vector3.Distance(hit.transform.position, transform.position);
						if (hitDist > maxFalloffDist)
						{
							finalDamage = chargedDamage * Mathf.Pow(falloffCoefficient, (maxFalloffDist - minFalloffDist) / falloffDistanceScale);
						}
						else if (hitDist < maxFalloffDist && hitDist > minFalloffDist)
						{
							finalDamage = chargedDamage * Mathf.Pow(falloffCoefficient, (hitDist - minFalloffDist) / falloffDistanceScale);
						}
					}

					// Send a damage message to the hit object
					/*Object[] sendArray = new Object[2];
					sendArray[0] = finalDamage;
					sendArray[1] = true;	*/
					hit.collider.SendMessageUpwards("ApplyDamagePlayer", finalDamage, SendMessageOptions.DontRequireReceiver);
					//hit.collider.SendMessageUpwards("Accuracy", SendMessageOptions.DontRequireReceiver);
					//And send a message to the decal manager, if the target uses decals
					if (hit.transform.gameObject.GetComponent<UseEffects>())
					{
						//The effectsManager needs five bits of information
						Quaternion hitRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
						int hitSet = hit.transform.gameObject.GetComponent<UseEffects>().setIndex;
						ArrayList hitInfo = new ArrayList { hit.point, hitRotation, hit.transform, hit.normal, hitSet };
						effectsManager.SendMessage("ApplyDecal", hitInfo, SendMessageOptions.DontRequireReceiver);
					}

					//Calculate force falloff
					float finalForce = force;
					if (hasFalloff)
					{
						if (hitDist > maxFalloffDist)
						{
							finalForce = finalForce * Mathf.Pow(forceFalloffCoefficient, (maxFalloffDist - minFalloffDist) / falloffDistanceScale);
						}
						else if (hitDist < maxFalloffDist && hitDist > minFalloffDist)
						{
							finalForce = finalForce * Mathf.Pow(forceFalloffCoefficient, (hitDist - minFalloffDist) / falloffDistanceScale);
						}
					}

					// Apply a force to the rigidbody we hit
					if (hit.rigidbody)
						hit.rigidbody.AddForceAtPosition(finalForce * ray.direction, hit.point);
				}
			}
		}

		void FireSpray()
		{
			if (!sprayOn)
			{
				sprayOn = true;
				sprayScript.ToggleActive(true);
				GetComponent<AudioSource>().clip = fireSound;
				GetComponent<AudioSource>().Play();
			}
			if (GetComponent<AudioSource>().clip == loopSound && GetComponent<AudioSource>().isPlaying && AimMode.sprintingPublic)
			{
				GetComponent<AudioSource>().Stop();
			}
			else if (GetComponent<AudioSource>() && !GetComponent<AudioSource>().isPlaying && !AimMode.sprintingPublic)
			{
				GetComponent<AudioSource>().clip = loopSound;
				GetComponent<AudioSource>().loop = true;
				GetComponent<AudioSource>().Play();
			}
			if (tempAmmo <= 0)
			{
				tempAmmo = 1;
				ammoLeft -= ammoPerShot;
			}
			else
			{
				tempAmmo -= Time.deltaTime * deltaTimeCoefficient;
			}
		}

		void ReleaseFire(int key)
		{
			if (GetComponent<AudioSource>())
			{
				if (GetComponent<AudioSource>().isPlaying && GetComponent<AudioSource>().clip == chargeLoop)
				{
					GetComponent<AudioSource>().Stop();
				}
			}
			if (sprayOn)
			{
				sprayScript.ToggleActive(false);
				sprayOn = false;
				if (GetComponent<AudioSource>())
				{
					GetComponent<AudioSource>().clip = releaseSound;
					GetComponent<AudioSource>().loop = false;
					GetComponent<AudioSource>().Play();
				}
			}
			if (chargeWeapon)
			{
				if (chargeLocked)
				{
					chargeLocked = false;
					chargeLevel = 0;
				}
				else if (chargeLevel > minCharge)
				{
					chargeReleased = true;
					Fire();
				}
				else
				{
					chargeLevel = 0;
				}
			}
		}

		int Comparison(RaycastHit x, RaycastHit y)
		{
			return x.distance > y.distance ? 1 : -1;
		}

		Vector3 SprayDirection()
		{
			float vx = (1 - 2 * Random.value) * actualSpread;
			float vy = (1 - 2 * Random.value) * actualSpread;
			float vz = 1.0f;
			return weaponCam.transform.TransformDirection(new Vector3(vx, vy, vz));
		}

		Vector3 SprayDirection(Vector3 dir)
		{
			float vx = (1 - 2 * Random.value) * actualSpread;
			float vy = (1 - 2 * Random.value) * actualSpread;
			float vz = (1 - 2 * Random.value) * actualSpread;
			return dir + new Vector3(vx, vy, vz);
		}
		public void Reload()
		{
			StartCoroutine(ReloadRoutine());
		}
		IEnumerator ReloadRoutine()
		{
			if (ammoLeft >= ammoPerClip || clips <= 0 || !gunActive || Avoidance.collided)
			{
				yield break;
			}
			reloadCancel = false;
			idleTime = 0;
			aim1.canSprint = PlayerWeapons.PW.reloadWhileSprinting;
			if (progressiveReload)
			{
				ProgReload();
				yield break;
			}

			if (reloading)
				yield break;

			//aim1.canSwitchWeaponAim = false;
			if (aim1.canAim)
			{
				aim1.canAim = false;
				aim = true;
			}
			if (gunType == gunTypes.spray)
			{
				if (GetComponent<AudioSource>())
				{
					if (GetComponent<AudioSource>().clip == loopSound && GetComponent<AudioSource>().isPlaying)
					{
						GetComponent<AudioSource>().Stop();
					}
				}
			}
			reloading = true;
			if (secondaryWeapon != null)
			{
				secondaryWeapon.reloading = true;
			}
			else if (!isPrimaryWeapon)
			{
				primaryWeapon.reloading = true;
			}
			bool tempEmpty;
			yield return new WaitForSeconds(waitforReload);
			if (reloadCancel)
			{
				yield break;
			}

			if (isPrimaryWeapon)
			{
				BroadcastMessage("ReloadAnimEarly", SendMessageOptions.DontRequireReceiver);
				if (ammoLeft >= ammoPerShot)
				{
					tempEmpty = false;
					BroadcastMessage("ReloadAnim", reloadTime, SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					tempEmpty = true;
					BroadcastMessage("ReloadEmpty", emptyReloadTime, SendMessageOptions.DontRequireReceiver);
				}
			}
			else
			{
				BroadcastMessage("SecondaryReloadAnimEarly", SendMessageOptions.DontRequireReceiver);
				if (ammoLeft >= ammoPerShot)
				{
					tempEmpty = false;
					BroadcastMessage("SecondaryReloadAnim", reloadTime, SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					tempEmpty = true;
					BroadcastMessage("SecondaryReloadEmpty", emptyReloadTime, SendMessageOptions.DontRequireReceiver);
				}
			}

			// Wait for reload time first - then add more bullets!
			if (ammoLeft > ammoPerShot)
			{
				yield return new WaitForSeconds(reloadTime);
			}
			else
			{
				yield return new WaitForSeconds(emptyReloadTime);
			}
			if (reloadCancel)
			{
				yield break;
			}
			reloading = false;
			if (secondaryWeapon != null)
			{
				secondaryWeapon.reloading = false;
			}
			else if (!isPrimaryWeapon)
			{
				primaryWeapon.reloading = false;
			}
			// We have a clip left reload
			if (ammoType == ammoTypes.byClip)
			{
				if (clips > 0)
				{
					if (!infiniteAmmo)
						clips--;
					ammoLeft = ammoPerClip;
				}
			}
			else if (ammoType == ammoTypes.byBullet)
			{
				if (clips > 0)
				{
					if (clips > ammoPerClip)
					{
						if (!infiniteAmmo)
							clips -= (int)(ammoPerClip - ammoLeft);

						ammoLeft = ammoPerClip;
					}
					else
					{
						float ammoVal = Mathf.Clamp(ammoPerClip, clips, ammoLeft + clips);
						if (!infiniteAmmo)
							clips -= (int)(ammoVal - ammoLeft);

						ammoLeft = ammoVal;
					}
				}
			}
			if (!tempEmpty && addOneBullet)
			{
				if (ammoType == ammoTypes.byBullet && clips > 0)
				{
					ammoLeft += 1;
					clips -= 1;
				}
			}
			if (aim)
				aim1.canAim = true;
			aim1.canSprint = true;
			//aim1.canSwitchWeaponAim = true;
			SendMessage("UpdateAmmo", ammoLeft, SendMessageOptions.DontRequireReceiver);
			SendMessage("UpdateClips", clips, SendMessageOptions.DontRequireReceiver);
			ApplyToSharedAmmo();
			PlayerWeapons.autoFire = autoFire;
		}

		void StopReloading()
		{
			reloading = false;
			if (secondaryWeapon != null)
			{
				secondaryWeapon.reloading = false;
			}
			else if (!isPrimaryWeapon)
			{
				primaryWeapon.reloading = false;
			}
			progressiveReloading = false;
			aim1.canSprint = true;
			PlayerWeapons.autoFire = autoFire;
			if (aim)
				aim1.canAim = true;
		}
		public void ProgReload()
		{
			StartCoroutine(ProgReloadRoutine());
		}
		IEnumerator ProgReloadRoutine()
		{
			if (reloading)
				yield break;
			//aim1.canSwitchWeaponAim = false;
			if (aim1.canAim)
			{
				aim1.canAim = false;
				aim = true;
			}

			BroadcastMessage("ReloadIn", reloadInTime);
			yield return new WaitForSeconds(reloadInTime);
			if (reloadCancel)
				yield break;
			if (progressiveReset)
			{
				clips += (int)ammoLeft;
				ammoLeft = 0;
			}

			progressiveReloading = true;
			reloading = true;
			if (secondaryWeapon != null && secondaryFire && !secondaryWeapon.secondaryInterrupt)
			{
				secondaryWeapon.reloading = true;
			}
			else if (secondaryFire && !secondaryInterrupt && !isPrimaryWeapon)
			{
				primaryWeapon.reloading = false;
			}
		}

		public float GetBulletsLeft()
		{
			return ammoLeft;
		}

		public void SelectWeapon()
		{
			StartCoroutine(SelectWeaponRoutine());
		}
		IEnumerator SelectWeaponRoutine()
		{
			AlignToSharedAmmo();
			idleTime = 0;
			if (!isPrimaryWeapon || puttingAway)
			{
				yield break;
			}
			if (!mainCam)
				mainCam = PlayerWeapons.mainCam;
			SetCrosshair();

			if (overrideAvoidance)
			{
				Avoidance.SetValues(rot, pos, dist, minDist, avoids);
			}
			else
			{
				Avoidance.SetValues();
			}

			if (secondaryWeapon != null)
			{
				secondaryWeapon.gunActive = false;
				secondaryWeapon.secondaryActive = false;
				BroadcastMessage("AimPrimary", SendMessageOptions.DontRequireReceiver);
			}
			if (!takingOut && !gunActive)
			{
				Renderer[] gos = GetComponentsInChildren<Renderer>();
				foreach (Renderer go in gos)
				{
					go.enabled = true;
				}

				wepDis.enabled = true;
				aim1.canSwitchWeaponAim = false;
				BroadcastMessage("TakeOutAnim", takeOutTime, SendMessageOptions.DontRequireReceiver);
				mainCam.SendMessage("TakeOutAnim", takeOutTime, SendMessageOptions.DontRequireReceiver);
				takingOut = true;
				interruptPutAway = true;
				yield return new WaitForSeconds(takeOutTime);
				if (puttingAway)
				{
					yield break;
				}
				//	return;
				SmartCrosshair.crosshair = true;
				gunActive = true;
				takingOut = false;
				aim1.canSwitchWeaponAim = true;
				ammo.enabled = true;
				sprint.enabled = true;
				wepDis.Select();
				SendMessage("UpdateAmmo", ammoLeft, SendMessageOptions.DontRequireReceiver);
				SendMessage("UpdateClips", clips, SendMessageOptions.DontRequireReceiver);
				NormalSpeed();

				if (gos.Length > 0)
					if (gos[0].GetComponent<Renderer>().enabled == false)
						foreach (Renderer go in gos)
						{
							go.enabled = true;
						}

				if (PlayerWeapons.autoReload && ammoLeft <= 0 && gunType != gunTypes.melee)
				{
					Reload();
				}
			}
		}

		public void DeselectWeapon()
		{
			StartCoroutine(DeselectWeaponRoutine());
		}
		IEnumerator DeselectWeaponRoutine()
		{
			if (GetComponent<AudioSource>())
			{
				if (GetComponent<AudioSource>().clip == loopSound && GetComponent<AudioSource>().isPlaying)
				{
					GetComponent<AudioSource>().Stop();
				}
			}
			chargeLevel = 0;
			reloadCancel = true;
			reloading = false;
			if (!gunActive)
				yield break;
			StopReloading();
			interruptPutAway = false;
			puttingAway = true;
			takingOut = false;
			ammo.enabled = false;
			sprint.enabled = false;
			wepDis.enabled = false;
			aim1.canSwitchWeaponAim = false;
			BroadcastMessage("PutAwayAnim", putAwayTime, SendMessageOptions.DontRequireReceiver);
			mainCam.SendMessage("PutAwayAnim", putAwayTime, SendMessageOptions.DontRequireReceiver);
			gunActive = false;
			SmartCrosshair.crosshair = false;
			yield return new WaitForSeconds(putAwayTime);
			puttingAway = false;

			/*if(takingOut || interruptPutAway){
				return;		
			}*/

			SendMessageUpwards("ActivateWeapon");
			Renderer[] gos = GetComponentsInChildren<Renderer>();
			foreach (Renderer go in gos)
			{
				go.enabled = false;
			}
		}
		void DeselectInstant()
		{
			if (GetComponent<AudioSource>())
			{
				if (GetComponent<AudioSource>().clip == loopSound && GetComponent<AudioSource>().isPlaying)
				{
					GetComponent<AudioSource>().Stop();
				}
			}
			chargeLevel = 0;
			if (!gunActive)
				return;
			takingOut = false;
			ammo.enabled = false;
			sprint.enabled = false;
			wepDis.enabled = false;
			gunActive = false;
			SmartCrosshair.crosshair = false;
			//SendMessageUpwards("ActivateWeapon");
			Renderer[] gos = GetComponentsInChildren<Renderer>();
			foreach (Renderer go in gos)
			{
				go.enabled = false;
			}
			BroadcastMessage("DeselectWeapon");
		}


		void EditorSelect()
		{
			gunActive = true;
			Renderer[] gos = GetComponentsInChildren<Renderer>();
			foreach (Renderer go in gos)
			{
				go.enabled = true;
			}
		}

		void EditorDeselect()
		{
			gunActive = false;
			Renderer[] gos = GetComponentsInChildren<Renderer>();
			foreach (Renderer go in gos)
			{
				go.enabled = false;
			}
		}

		void WalkSway()
		{
			int speed = (int)CM.GetComponent<CharacterController>().velocity.magnitude;
			if (speed < .2)
			{
				ResetPosition();
				return;
			}
			if (!sway || !gunActive)
				return;

			//sine function for motion
			float t = Time.time - CamSway.singleton.swayStartTime;
			Vector3 curVect = Vector3.zero;
			/*if(CM.crouching){
				swayRate = moveSwayRate*CM.movement.maxCrouchSpeed/CM.movement.defaultForwardSpeed;
			} else if (CM.prone) {
				swayRate = moveSwayRate*CM.movement.maxProneSpeed/CM.movement.defaultForwardSpeed;
			} else if (AimMode.sprintingPublic) {
				swayRate = runSwayRate;
			} else {
				swayRate = moveSwayRate;
			}*/
			curVect.x = swayAmplitude.x * Mathf.Sin(swayRate.x * t + (idleSwayRate.x / 2)) * Mathf.Sin(swayRate.x * t + (idleSwayRate.x / 2));
			curVect.y = Mathf.Abs(swayAmplitude.y * Mathf.Sin(swayRate.y * t + (idleSwayRate.y / 2)));

			curVect.x -= swayAmplitude.x / 2;
			curVect.y -= swayAmplitude.y / 2;

			//offset from start position
			curVect += startPosition;

			float s = new Vector3(PlayerWeapons.CM.movement.velocity.x, 0, PlayerWeapons.CM.movement.velocity.z).magnitude / 14;

			//move towards target
			transform.localPosition = new Vector3(Mathf.Lerp(transform.localPosition.x, curVect.x, Time.deltaTime * swayRate.x * s), Mathf.Lerp(transform.localPosition.y, curVect.y, Time.deltaTime * swayRate.y * s), transform.localPosition.z);
			transform.localEulerAngles = new Vector3(Mathf.LerpAngle(transform.localEulerAngles.x, -curVect.y, Time.deltaTime * swayRate.y * s), transform.localEulerAngles.y, Mathf.LerpAngle(transform.localEulerAngles.z, -curVect.x, Time.deltaTime * s));
		}

		void ResetPosition()
		{
			if (((transform.localPosition == startPosition) && !sway) || !gunActive)
			{
				return;
			}
			float rate = .15f * Time.deltaTime;

			Vector3 curVect = Vector3.zero;
			if (sway && !aim1.aiming)
			{
				//sine function for idle motion
				curVect.x = idleAmplitude.x * Mathf.Sin(idleSwayRate.x * Time.time);
				curVect.y = idleAmplitude.y * Mathf.Sin(idleSwayRate.y * Time.time);
				curVect.x -= idleAmplitude.x / 2;
				curVect.y -= idleAmplitude.y / 2;

				//offset from start position
				curVect += startPosition;
			}
			else
			{
				curVect = startPosition;
			}
			//move towards target
			transform.localPosition = new Vector3(Mathf.Lerp(transform.localPosition.x, curVect.x, Time.deltaTime * swayRate.x), Mathf.Lerp(transform.localPosition.y, curVect.y, Time.deltaTime * swayRate.y), transform.localPosition.z);
			transform.localEulerAngles = new Vector3(Mathf.LerpAngle(transform.localEulerAngles.x, curVect.y, Time.deltaTime * swayRate.y), transform.localEulerAngles.y, Mathf.LerpAngle(transform.localEulerAngles.z, curVect.x, Time.deltaTime * swayRate.x));

		}

		void Sprinting()
		{
			if (!gunActive)
				return;
			idleTime = 0;
			PlayerWeapons.sprinting = true;
			swayRate = runSwayRate;
			swayAmplitude = runAmplitude;

			//Only affects charge weapons
			if (chargeWeapon)
			{
				chargeLocked = true;
				chargeLevel = 0;
			}
		}

		void NormalSpeed()
		{
			if (airborne)
				return;
			PlayerWeapons.sprinting = false;
			if (secondaryWeapon != null)
			{
				if (isPrimaryWeapon && secondaryWeapon.secondaryActive)
					return;
			}
			if (!isPrimaryWeapon && !secondaryActive)
				return;
			swayRate = moveSwayRate;
			if (CharacterMotorDB.crouching)
			{
				swayRate = moveSwayRate * CM.movement.maxCrouchSpeed / CM.movement.defaultForwardSpeed;
			}
			else if (CharacterMotorDB.prone)
			{
				swayRate = moveSwayRate * CM.movement.maxProneSpeed / CM.movement.defaultForwardSpeed;
			}
			swayAmplitude = moveSwayAmplitude;
			//gunActive = true;

			//Only affects charge weapons
			if (chargeWeapon)
			{
				chargeLocked = false;
				chargeLevel = 0;
			}
		}

		void Kickback()
		{
			mouseY.offsetY = curKickback;
			mouseY.maxKickback = maxKickback;
			mouseX.offsetX = curKickback * xKickbackFactor;//*Random.value;
			mouseX.maxKickback = maxKickback;
			if (mouseY.offsetY < mouseY.maxKickback)
				mouseY.resetDelay = recoilDelay;
			if (Mathf.Abs(mouseX.offsetX) < mouseX.maxKickback)
				mouseX.resetDelay = recoilDelay;
		}
		public void ActivateSecondary()
		{
			StartCoroutine(ActivateSecondaryRoutine());
		}
		IEnumerator ActivateSecondaryRoutine()
		{
			if (secondaryWeapon == null || secondaryFire || reloading)
				yield break;
			AlignToSharedAmmo();
			if (gunActive)
			{
				SmartCrosshair.crosshair = false;
				gunActive = false;
				BroadcastMessage("EnterSecondary", enterSecondaryTime);
				yield return new WaitForSeconds(enterSecondaryTime);
				SmartCrosshair.crosshair = true;
				secondaryWeapon.gunActive = true;
				secondaryActive = true;
				secondaryWeapon.SetCrosshair();
				SendMessage("UpdateAmmo", secondaryWeapon.ammoLeft, SendMessageOptions.DontRequireReceiver);
				SendMessage("UpdateClips", secondaryWeapon.clips, SendMessageOptions.DontRequireReceiver);
				BroadcastMessage("AimSecondary", SendMessageOptions.DontRequireReceiver);
			}
		}

		void SetCrosshair()
		{
			if (crosshairObj != null)
			{
				weaponCam.GetComponent<SmartCrosshair>().SetCrosshair();
				SmartCrosshair.cObj = crosshairObj;
				SmartCrosshair.cSize = crosshairSize;
				SmartCrosshair.scl = scale;
				SmartCrosshair.sclRef = maxSpread;
				SmartCrosshair.ownTexture = true;
			}
			else
			{
				SendMessageUpwards("DefaultCrosshair");
			}
		}
		public void ActivatePrimary()
		{
			StartCoroutine(ActivatePrimaryRoutine());
		}
		IEnumerator ActivatePrimaryRoutine()
		{
			AlignToSharedAmmo();
			if (reloading)
				yield break;
			if (!gunActive)
			{
				secondaryWeapon.gunActive = false;
				secondaryActive = false;
				SmartCrosshair.crosshair = false;
				BroadcastMessage("ExitSecondary", exitSecondaryTime);
				yield return new WaitForSeconds(exitSecondaryTime);
				SmartCrosshair.crosshair = true;
				gunActive = true;
				SetCrosshair();
				SendMessage("UpdateAmmo", ammoLeft, SendMessageOptions.DontRequireReceiver);
				SendMessage("UpdateClips", clips, SendMessageOptions.DontRequireReceiver);
				BroadcastMessage("AimPrimary", SendMessageOptions.DontRequireReceiver);
			}
		}

		public void EjectShell()
		{
			StartCoroutine(EjectShellRoutine());
		}

		IEnumerator EjectShellRoutine()
		{
			yield return new WaitForSeconds(ejectDelay);
			GameObject instantiatedProjectile1 = Instantiate(shell, ejectorPosition.transform.position, ejectorPosition.transform.rotation) as GameObject;
		}

		void FireEffects()
		{
			bool scoped = transform.Find("AimObject").GetComponent<AimMode>().inScope;
			if (!scoped)
				BroadcastMessage("MuzzleFlash", isPrimaryWeapon, SendMessageOptions.DontRequireReceiver);

			if (fireSound == null)
				return;
			//Play Audio
			GameObject audioObj = new GameObject("GunShot");
			audioObj.transform.position = transform.position;
			audioObj.transform.parent = transform;
			audioObj.AddComponent<TimedObjectDestructorDB>().timeOut = fireSound.length + .1f;
			AudioSource aO = audioObj.AddComponent<AudioSource>();
			aO.clip = fireSound;
			aO.volume = fireVolume;
			aO.pitch = firePitch;
			aO.Play();
			aO.loop = false;
			aO.rolloffMode = AudioRolloffMode.Linear;
		}

		//Returns primary gunscript on this weapon
		GunScript GetPrimaryGunScript()
		{
			if (isPrimaryWeapon)
			{
				return this;
			}
			else
			{
				return primaryWeapon;
			}
		}
	}
}