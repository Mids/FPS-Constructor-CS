using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	public class GunScript : MonoBehaviour
	{
		public float kickbackAngle; //Vertical kickback per shot (degrees)
		public float xKickbackFactor = .5f; //Horizontal kickback per shot (percent of vertical)
		public float maxKickback = 15; //Maximum vertical kickback (degrees)
		public float kickbackAim;
		public float crouchKickbackMod = .6f;
		public float proneKickbackMod = .35f;
		public float moveKickbackMod = 1.3f;

		private float curKickback;
		public float recoilDelay = .11f; //Delay between stopping firing and recoil decreasing

		/*Spread variables: Spread (between 0 and 1) determines the accuracy of the weapon.
		A spread of 0 means that the weapon will fire exactly towards the crosshair, and
		a spread of 1 means that it will fire anywhere within 90 degrees of the crosshair.
		*/
		public float standardSpread = .1f; //default spread of this weapon
		public float maxSpread = .25f; //Maximum spread of this weapon
		public float crouchSpreadModifier = .7f; //When crouching, spread is multiplied by this
		public float proneSpreadModifier = .4f; //When prone, spread is multiplied by this
		public float moveSpreadModifier = 1.5f; //When walking, spread is multiplied by this
		public float standardSpreadRate = .05f; //Standard increase in spread per shot
		public float aimSpreadRate = .01f; //Increase in spread per shot when aiming
		public float aimSpread = .01f; //Default spread when aiming
		public float spDecRate = .05f; //Speed at which spread decreases when not firing


		////////// Ammo variables //////////
		public float ammoLeft = 0; //Ammo left in the curent clip (ammo before next reload)
		public int ammoPerClip = 40; //Shots per clip
		public int ammoPerShot = 1; //Ammo used per shot
		public int clips = 20; //Number of spare clips (reloads) left
		public int maxClips = 20; //Maximum number of clips
		public bool infiniteAmmo = false; //Does this gun deplete clips whe reoading?
		public enum ammoTypes 
		{ 
			byClip, 
			byBullet 
		}
		public ammoTypes ammoType = ammoTypes.byClip; //Does this weapon conserve ammo when reloading? (e.g. if you reload after shooting one bullet, does it use a whole clip)


		////////// Fire Variables //////////
		public AudioClip fireSound; //Sound that plays when firing
		public float fireVolume = 1;
		public float firePitch = 1;//Pitch of fire sound
		public float fireRate = 0.05f; //Time in seconds between shots
		public bool autoFire; //Is this weapon automatic (can you hold down the fre button?)
		public bool fireAnim = false; //Does this weapon's fire animation scale to fit the fire rate (generally used for non-automatic weapons)
		public float delay = 0; //Delay between hitting fire button and actually firing (can be used to sync firing with animation)
		public AudioClip emptySound; //Sound that plays when firing
		public float emptyVolume = 1;
		public float emptyPitch = 1;//Pitch of fire sound

		//Burst fire
		//note: burst fire doesn't work well with automatic weapons
		public bool burstFire = false; //does this wepon fire in bursts
		public int burstCount = 1; //shots per burst
		public float burstTime = .5f; //time to fire full burst


		////////// Reloading variables //////////
		public float reloadTime = 0.5f;
		public float emptyReloadTime = .4f;
		public bool addOneBullet = false;
		public float waitforReload = 0.00f;

		/*Progressive Reload is a different kind of reloading where the reload is broken into stages.
		The first stage initializes the animation to get to the second stage. The second stage represents
		reloading one shot, and will repeat as many times as necessary to reach a full clip unless
		interrupted. Then the third stage returns the weapon to its standrad position. This is useful
		for weapons like shotguns that reload by shells.
		*/
		public bool progressiveReload = false; //Does this weapon use progressive reload?
		public bool progressiveReset = false; //Does this weapon's ammo reset to 0 when starting a reload? 
		//(e.g. a revolver where the shells are discarded and replaced)
		public float reloadInTime = 0.5f; //time in seconds for the first stage
		public float reloadOutTime = 0.5f; //time in seconds for the third stage
		//the time for the second stage is just reloadTime


		////////// Gun-Specific Variables //////////
		public float range = 100.0f; //Range of bullet raycast in meters
		public float force = 10.0f; //Force of bullet
		public float damage = 5.0f; //Damage per bullet
		public int shotCount = 6; //Bullets per shot
		public int penetrateVal = 1; //penetration level of bullet

		/* Damage falloff allows raycast weapons to do less damage at long distances
		*/
		public bool hasFalloff = false; //Does this weapon use damage falloff?
		public float minFalloffDist = 10; //Distance at which falloff begins to take effect
		public float maxFalloffDist = 100; //Distance at which falloff stops (minumum damage)
		public float falloffCoefficient = 1; //Coefficient for multiplying damage
		public float falloffDistanceScale = 4; //Scaling value to change speed of falloff


		////////// Launcher-Specific Variables //////////
		public Rigidbody projectile; //The object to launch. This can be anything, as long as it has a rigidbody.
		public float initialSpeed = 20.0f; //Initial speed of projectile, applied  forward
		public int projectileCount = 1; //Number of projectiles fired
		public GameObject launchPosition; //GameObject whose position the projectile is fired from (place this at the end of the weapon's barrel, generally)


		////////// Tracer related variables //////////
		/* Tracers are done using particle emitters.
		*/
		public GameObject tracer; //Tracer object. Must have a particle emitter attached
		public int traceEvery = 0; //Activate a tracer evey x shots.
		public float simulateTime = .02f; //How long to simulate tracer before it appears
		public float minDistForTracer = 2; //This isn't exposed, but can be tweaked if needed


		////////// Sway //////////
		public bool sway; //Does the weapon sway?
		public Vector2 moveSwayRate = new Vector2(2.5f, 5); //How fast does the weapon sway when walking? (xy)
		public Vector2 moveSwayAmplitude = new Vector2(.04f, .01f); //How much does the weapon sway when walking? (xy)
		public Vector2 runSwayRate = new Vector2(4.5f, .9f); //How fast does the weapon sway when sprinting? (xy)
		public Vector2 runAmplitude = new Vector2(.04f, .04f); //How much does the weapon sway when sprinting? (xy)
		public Vector2 idleSwayRate = new Vector2(2, 1); //How fast does the weapon sway when standing? (xy)
		public Vector2 idleAmplitude = new Vector2(.002f, .001f); //How much does the weapon sway when standing? (xy)


		////////// Secondary Weapons //////////
		public GunScript secondaryWeapon; //Gunscript of secondary weapon (additional weapon script on this object)
		public bool secondaryInterrupt = false; //Can primary and secondary weapon interrupt each other's actions
		public bool secondaryFire = false; //Is the secondary weapon fired with Mouse2?
		public float enterSecondaryTime = .5f; //How long does it take to switch to secondary (animation)?
		public float exitSecondaryTime = .5f; //How long does it take to switch from secondary (animation)?


		////////// Charge weapon variables //////////
		public float minCharge = 0; //Minimum charge value at ahich the weapon can fire
		public float maxCharge = 10; // Maximum charge value the weapon can have
		public float chargeLevel = 0; //current charge level of the weapon
		public bool forceFire = false; //Does this weapon have to fire when it hits max charge?
		public AudioClip chargeLoop; //Sound to play when charging
		public bool chargeAuto = false; //Does the weapon automatically start charging again after a forced release?


		//Specifically for hitscan charge weapons
		public float chargeCoefficient = 1.1f; //Damage multiplier as charge increases
		public float additionalAmmoPerCharge = 0; //Ammo change as charge increases (add this per 1 charge level)

		//////////Other variables//////////
		public float idleTime = 0; //Time in seconds that the player has been idle
		public float timeToIdle = 7; //Time in seconds of being idle which will cause the idle animation to play
		public float takeOutTime = .6f; //Time to take out (switch to) weapon
		public float putAwayTime = .6f; //Time to put away (switch from) weapon 

		//////////Z KickBack//////////
		public bool useZKickBack = true; //Does this weapon use z kickback?
		public float kickBackZ = 2; //Rate of z kickback when firing
		public float zRetRate = 1; //rate of return from z when not firing
		public float maxZ = .3f; //maximum z kickback

		//////////Avoidance//////////
		//Avoidance is by default handled globall by the Avoidance Component. This just overrides its values for this weapon.
		public bool overrideAvoidance = false; //Does this weapon override global object avoidance values
		public bool avoids = true;
		public Vector3 rot;
		public Vector3 pos;
		public float dist = 2;
		public float minDist = 1.5f;

		//Shell Ejection
		public bool shellEjection = false; //Does this weapon use shell ejection?
		public GameObject ejectorPosition; //If it does, this gameobject provides the position where shells are instantiated
		public float ejectDelay = 0;
		public GameObject shell; //The shell prefab to instantiate


		//Custom crosshair variables
		public bool scale = false; //Does the crosshair scale with accuracy?
		public GameObject crosshairObj; //Crosshair object to use
		public float crosshairSize; //Default scale of the crosshair object

		///////////////////////// END CHANGEABLE BY USER /////////////////////////



		///////////////////////// Internal Variables /////////////////////////
		/*These variables should not be modified directly, weither because it could compromise
		the functioning of the package, or because changes will be overwritten.
		*/


		public bool gunActive = false; // Is the weapon currently selected & activated


		//Status
		private bool interruptPutAway = false;
		private bool progressiveReloading = false;
		public bool inDelay = false;
		private int m_LastFrameShot = -1;
		public bool reloading = false;
		public float nextFireTime = 0.0f;
		public static bool takingOut = false;
		public static bool puttingAway = false;

		public bool secondaryActive = false;
		public static float crosshairSpread = 0;
		private float shotSpread;
		private float actualSpread;
		private float spreadRate = .05f;
		public bool isPrimaryWeapon = true;
		public bool aim = false;
		public bool aim2 = false;
		private float pReloadTime = 0;
		private bool stopReload = false;
		private Vector3 startPosition;
		public bool gunDisplayed;
		private float totalKickBack; //How far have we kicked back?

		//Components
		public AmmoDisplay ammo;
		public SprintDisplay sprint;
		public WeaponDisplay wepDis;
		public static GameObject mainCam;
		public static GameObject weaponCam;
		private GunScript primaryWeapon;
		private GameObject player;
		public AimMode aim1;
		public MouseLookDBJS mouseY;
		public MouseLookDBJS mouseX;
		public bool reloadCancel = false;


		////////// Spray //////////
		/* Spray weapons are meant to be a simple solution for something that can now be done better with a 
		charge weapon.
		*/
		private float tempAmmo = 1;
		public bool sprayOn = false;
		public GameObject sprayObj;
		public SprayScript sprayScript;
		public float deltaTimeCoefficient = 1;
		public float forceFalloffCoefficient = .99f;
		public AudioClip loopSound;
		public AudioClip releaseSound;
		public float ammoPerSecond;

		////////// Charge weapon variables //////////
		public bool chargeWeapon = false; //Is this weapon a charge weapon?
		public bool chargeReleased = false;
		public bool chargeLocked = false;

		//Gun Types
		public enum gunTypes 
		{
			hitscan,
			launcher,
			melee,
			spray 
		}
		public gunTypes gunType = gunTypes.hitscan;

		//Melee
		public bool hitBox = false;

		//Tracer related variables
		private int shotCountTracer = 0;

		//Ammo Sharing
		public bool sharesAmmo = false;
		public bool shareLoadedAmmo = false;
		public int ammoSetUsed = 0;
		public GameObject managerObject;
		public AmmoManager ammoManagerScript;

		//Effects
		public EffectsManager effectsManager;
		public CharacterMotorDB CM;

		//Inspector only variables
		public bool shotPropertiesFoldout = false;
		public bool firePropertiesFoldout = false;
		public bool accuracyFoldout = false;
		public bool altFireFoldout = false;
		public bool ammoReloadFoldout = false;
		public bool audioVisualFoldout = false;

		//Sway (Internal)
		public float swayStartTime = 0;
		public Vector2 swayRate;
		public Vector2 swayAmplitude;
		public bool overwriteSway = false;

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

		public void Aiming()
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

		public void Crouching()
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

		public void Prone()
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

		public void Walking()
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

		public void StopWalking()
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

		public void Landed()
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

		public void Airborne()
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

		public void StopAiming()
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
		public void Cooldown()
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

		public void Update()
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

		public void LateUpdate()
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

		public void FireAlt()
		{
			if (!isPrimaryWeapon)
			{
				AlignToSharedAmmo();
				gunActive = true;
				Fire();
				gunActive = false;
			}
		}

		public void AlignToSharedAmmo()
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

		public void ApplyToSharedAmmo()
		{
			if (sharesAmmo)
			{
				ammoManagerScript.clipsArray[ammoSetUsed] = clips;
				ammoManagerScript.maxClipsArray[ammoSetUsed] = maxClips;
				ammoManagerScript.infiniteArray[ammoSetUsed] = infiniteAmmo;
			}
		}

		public void Fire2()
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
		public void KickBackZ()
		{
			if (!useZKickBack)
				return;
			float amt = Time.deltaTime * kickBackZ;
			amt = Mathf.Min(amt, maxZ - totalKickBack);
			transform.localPosition = transform.localPosition - new Vector3(0, 0, amt);
			totalKickBack += amt;
		}

		//Reset Kickback function which moves the gun transform forwards when called
		public void ReturnKickBackZ()
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

		public void FireOneProjectile()
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

		public void FireOneBullet()
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

		public void FireSpray()
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

		public void ReleaseFire(int key)
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

		public int Comparison(RaycastHit x, RaycastHit y)
		{
			return x.distance > y.distance ? 1 : -1;
		}

		public Vector3 SprayDirection()
		{
			float vx = (1 - 2 * Random.value) * actualSpread;
			float vy = (1 - 2 * Random.value) * actualSpread;
			float vz = 1.0f;
			return weaponCam.transform.TransformDirection(new Vector3(vx, vy, vz));
		}

		public Vector3 SprayDirection(Vector3 dir)
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

		public void StopReloading()
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
		public void DeselectInstant()
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


		public void EditorSelect()
		{
			gunActive = true;
			Renderer[] gos = GetComponentsInChildren<Renderer>();
			foreach (Renderer go in gos)
			{
				go.enabled = true;
			}
		}

		public void EditorDeselect()
		{
			gunActive = false;
			Renderer[] gos = GetComponentsInChildren<Renderer>();
			foreach (Renderer go in gos)
			{
				go.enabled = false;
			}
		}

		public void WalkSway()
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

		public void ResetPosition()
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

		public void Sprinting()
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

		public void NormalSpeed()
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

		public void Kickback()
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

		public void SetCrosshair()
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

		public void FireEffects()
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
		public GunScript GetPrimaryGunScript()
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