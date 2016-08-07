using UnityEngine;
using UnityEditor;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	[CustomEditor(typeof(GunScript))]
	public class GunScriptEditor : Editor
	{
		public GunScript Instance { get { return (GunScript)target; } }
		public bool displayGun = false;
		public bool gunDisplayed = false;
		public GunScript.gunTypes gunTipo = GunScript.gunTypes.hitscan;

		void OnInspectorGUI()
		{
			EditorGUIUtility.LookLikeInspector();
			//EditorGUILayout.BeginVertical();

			Instance.gunType = (GunScript.gunTypes)EditorGUILayout.EnumPopup(new GUIContent("  Gun Type: ", "The basic type of weapon - choose 'hitscan' for a basic bullet-based weapon"), Instance.gunType);
			gunTipo = Instance.gunType;


			if (gunTipo == GunScript.gunTypes.spray)
			{
				Instance.sprayObj = (GameObject)EditorGUILayout.ObjectField(new GUIContent("  Spray Object: ", "Spray weapons need an attached object with a particle collider and the script SprayScript"), Instance.sprayObj, typeof(GameObject), true);
			}
			else if (gunTipo == GunScript.gunTypes.launcher)
			{
				Instance.launchPosition = (GameObject)EditorGUILayout.ObjectField(new GUIContent("  Launch Position: ", "The projectile will be instantiated at the position of the object in this field"), Instance.launchPosition, typeof(GameObject), true);
			}

			EditorGUILayout.Separator();

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button(new GUIContent("Open All", "Open all the foldout menus"), "miniButton"))
			{
				Instance.shotPropertiesFoldout = true;
				Instance.firePropertiesFoldout = true;
				Instance.accuracyFoldout = true;
				Instance.altFireFoldout = true;
				Instance.ammoReloadFoldout = true;
				Instance.audioVisualFoldout = true;
			}

			if (GUILayout.Button(new GUIContent("Close All", "Close all the foldout menus"), "miniButton"))
			{
				Instance.shotPropertiesFoldout = false;
				Instance.firePropertiesFoldout = false;
				Instance.accuracyFoldout = false;
				Instance.altFireFoldout = false;
				Instance.ammoReloadFoldout = false;
				Instance.audioVisualFoldout = false;
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Separator();

			//Shot properties
			if (gunTipo != GunScript.gunTypes.melee)
			{
				EditorGUILayout.BeginVertical("toolbar");
				Instance.shotPropertiesFoldout = EditorGUILayout.Foldout(Instance.shotPropertiesFoldout, "Shot Properties (damage etc.):");
				EditorGUILayout.EndVertical();
			}
			else
			{
				Instance.shotPropertiesFoldout = false;
			}
			if (Instance.shotPropertiesFoldout)
			{
				EditorGUILayout.BeginVertical("textField");
				EditorGUILayout.Separator();
				if (gunTipo == GunScript.gunTypes.launcher)
				{
					Instance.projectile = (Rigidbody)EditorGUILayout.ObjectField(new GUIContent("  Projectile: ", "This is the actual GameObject to be instantiated from the launcher"), Instance.projectile, typeof(Rigidbody), false);
					Instance.initialSpeed = EditorGUILayout.FloatField(new GUIContent("  Initial Speed: ", "The initial speed each projectile will have when fired"), Instance.initialSpeed);
				}
				if (gunTipo == GunScript.gunTypes.hitscan || gunTipo == GunScript.gunTypes.spray)
				{
					Instance.range = EditorGUILayout.FloatField(new GUIContent("  Range: ", "The maximum range this gun can hit at"), Instance.range);
					Instance.force = EditorGUILayout.FloatField(new GUIContent("  Force: ", "The force which is applied to the object we hit"), Instance.force);
					if (gunTipo == GunScript.gunTypes.spray)
					{
						Instance.damage = EditorGUILayout.FloatField(new GUIContent("  Damage Per Second:  ", "The damage done by the weapon over each second of continous fire"), Instance.damage);
					}
					else
					{
						Instance.penetrateVal = EditorGUILayout.IntField(new GUIContent("  Penetration Level: ", "If your scene is set up to use bullet penetration, this determines the piercing ability of this weapon"), Instance.penetrateVal);
						Instance.damage = EditorGUILayout.FloatField(new GUIContent("  Damage: ", "The damage done per bullet by this weapon"), Instance.damage);
					}
					if (Instance.chargeWeapon)
					{
						Instance.chargeCoefficient = EditorGUILayout.FloatField(new GUIContent("  Charge Coefficient: ", "Multiply this weapon's damage by this number to the power of the current charge level (A value of 1.1f would lead to a 10% increase in damage after charging for one second, for example"), Instance.chargeCoefficient);
					}
					if (gunTipo == GunScript.gunTypes.spray)
					{
						Instance.hasFalloff = true;
					}
					else
					{
						Instance.hasFalloff = EditorGUILayout.Toggle(new GUIContent("  Use Damage Falloff: ", "Does this weapon's damage change with distance?"), Instance.hasFalloff);
					}
					if (Instance.hasFalloff)
					{
						float tempDamageDisplay = (Instance.damage * Mathf.Pow(Instance.falloffCoefficient, (Instance.maxFalloffDist - Instance.minFalloffDist) / Instance.falloffDistanceScale));
						EditorGUILayout.LabelField("  Damage at max falloff: ", "" + tempDamageDisplay);
						float tempForceDisplay = (Instance.force * Mathf.Pow(Instance.forceFalloffCoefficient, (Instance.maxFalloffDist - Instance.minFalloffDist) / Instance.falloffDistanceScale));
						EditorGUILayout.LabelField("  Force at max falloff: ", "" + tempForceDisplay);
						Instance.minFalloffDist = EditorGUILayout.FloatField(new GUIContent("  Min. Falloff Distance: ", "Falloff is not applied to hits closer than this distance"), Instance.minFalloffDist);
						Instance.maxFalloffDist = EditorGUILayout.FloatField(new GUIContent("  Max. Falloff Distance: ", "Falloff is not applied to hits past this distance"), Instance.maxFalloffDist);
						Instance.falloffCoefficient = EditorGUILayout.FloatField(new GUIContent("  Damage Coefficient: ", "The weapon's damage is multiplied by this for every unit of distance"), Instance.falloffCoefficient);
						Instance.forceFalloffCoefficient = EditorGUILayout.FloatField(new GUIContent("  Force Coefficient: ", "The force applied is multiplied by this every unit of distance"), Instance.forceFalloffCoefficient);
						Instance.falloffDistanceScale = EditorGUILayout.FloatField(new GUIContent("  Falloff Distance Scale: ", "This defines how many unity meters make up one 'unit' of distance in falloff calculations"), Instance.falloffDistanceScale);
					}
					if (gunTipo == GunScript.gunTypes.melee)
					{
						Instance.damage = 0;
						Instance.force = 0;
					}
					EditorGUILayout.Separator();
					EditorGUILayout.EndVertical();
				}
			}

			//Fire Properties
			if (gunTipo != GunScript.gunTypes.spray)
			{
				EditorGUILayout.BeginVertical("toolbar");
				Instance.firePropertiesFoldout = EditorGUILayout.Foldout(Instance.firePropertiesFoldout, "Fire Properties (fire rate etc.):");
				EditorGUILayout.EndVertical();
			}
			else
			{
				Instance.firePropertiesFoldout = false;
			}
			if (Instance.firePropertiesFoldout)
			{
				EditorGUILayout.BeginVertical("textField");
				EditorGUILayout.Separator();
				if (gunTipo == GunScript.gunTypes.melee)
				{
					Instance.shotCount = 0;
					Instance.fireRate = EditorGUILayout.FloatField(new GUIContent("  Attack Rate:  ", "Attacks per second"), Instance.fireRate);
					Instance.delay = EditorGUILayout.FloatField(new GUIContent("  Damage Delay:  ", "How long into the attack does the hitbox activate, in seconds. The hit box is active during this time"), Instance.delay);
					Instance.reloadTime = EditorGUILayout.FloatField(new GUIContent("  Recovery Time:  ", "The time, in seconds, after each attack before it is possible to attack again"), Instance.reloadTime);
				}

				if (gunTipo == GunScript.gunTypes.hitscan || gunTipo == GunScript.gunTypes.launcher)
				{
					if (gunTipo == GunScript.gunTypes.hitscan)
					{
						Instance.shotCount = EditorGUILayout.IntField(new GUIContent("  Shot Count: ", "The number of shots fired per pull of the trigger"), Instance.shotCount);
					}
					else
					{
						Instance.projectileCount = EditorGUILayout.IntField(new GUIContent("  Projectile Count: ", "The number of projectiles fired with every pull of the trigger"), Instance.projectileCount);
					}
					Instance.fireRate = EditorGUILayout.FloatField(new GUIContent("  Fire Rate: ", "The time in seconds after firing before the weapon can be fired again"), Instance.fireRate);
					Instance.chargeWeapon = EditorGUILayout.Toggle(new GUIContent("  Charge Weapon: ", "Charge weapons are weapons which can be 'charged up' by holding down fire. Charge is measured in an internal variable called 'chargeLevel'"), Instance.chargeWeapon);
					if (Instance.chargeWeapon)
					{
						Instance.autoFire = false;
						Instance.minCharge = EditorGUILayout.FloatField(new GUIContent("  Minimum Charge: ", "The weapon cannot fire unless it has charged up to at least this value"), Instance.minCharge);
						Instance.maxCharge = EditorGUILayout.FloatField(new GUIContent("  Maximum Charge: ", "The weapon cannot charge up beyond this value"), Instance.maxCharge);
						Instance.forceFire = EditorGUILayout.Toggle(new GUIContent("  Must Fire When Charged: ", "If this is checked, the weapon will be automatically discharged the moment it is fully charged"), Instance.forceFire);
						if (Instance.forceFire)
						{
							Instance.chargeAuto = EditorGUILayout.Toggle(new GUIContent("  Charge after force release: ", "When the weapon dischrages because it reaches maximum charge, and chargeAuto is enabled, the weapon will begin charging again immediately if able. If unchecked, the player will have to release the mouse and press it to start charging again"), Instance.chargeAuto);
						}
					}
					if (!Instance.autoFire)
					{
						Instance.burstFire = EditorGUILayout.Toggle(new GUIContent("  Burst Fire: ", "Does this weapon fire in bursts?"), Instance.burstFire);
					}
					if (Instance.burstFire)
					{
						Instance.burstCount = EditorGUILayout.IntField(new GUIContent("  Burst Count: ", "How many shots fired per burst?"), Instance.burstCount);
						Instance.burstTime = EditorGUILayout.FloatField(new GUIContent("  Burst Time: ", "How long does it take to fire the full burst?"), Instance.burstTime);
					}
					if (!Instance.burstFire && !Instance.chargeWeapon)
					{
						Instance.autoFire = EditorGUILayout.Toggle(new GUIContent("  Full Auto: ", "Is this weapon fully automatic?"), Instance.autoFire);
					}
				}
				EditorGUILayout.Separator();
				EditorGUILayout.EndVertical();
			}

			//Accuracy
			if (gunTipo != GunScript.gunTypes.melee)
			{
				EditorGUILayout.BeginVertical("toolbar");
				Instance.accuracyFoldout = EditorGUILayout.Foldout(Instance.accuracyFoldout, "Accuracy Properties:");
				EditorGUILayout.EndVertical();
			}
			else
			{
				Instance.accuracyFoldout = false;
			}

			if (Instance.accuracyFoldout)
			{
				EditorGUILayout.BeginVertical("textField");
				EditorGUILayout.Separator();
				if (gunTipo == GunScript.gunTypes.hitscan || gunTipo == GunScript.gunTypes.launcher)
				{
					Instance.standardSpread = EditorGUILayout.FloatField(new GUIContent("  Standard Spread: ", "The weapon's spread when firing from the hip and standing still. A spread of 1 means the shot could go 90 degrees in any direction; .5 would be 45 degrees"), Instance.standardSpread);
					Instance.standardSpreadRate = EditorGUILayout.FloatField(new GUIContent("  Spread Rate: ", "The rate at which the weapon's spread increases while firing from the hip. This value is added to the spread after every shot"), Instance.standardSpreadRate);
					Instance.spDecRate = EditorGUILayout.FloatField(new GUIContent("  Spread Decrease Rate: ", "The rate at which the spread returns to normal."), Instance.spDecRate);
					Instance.maxSpread = EditorGUILayout.FloatField(new GUIContent("  Maximum Spread: ", "The weapon's spread will not naturally exceed this value."), Instance.maxSpread);
					EditorGUILayout.Separator();
					Instance.aimSpread = EditorGUILayout.FloatField(new GUIContent("  Aim Spread: ", "The weapon's spread when firing in aim mode.  A spread of 1 means the shot could go 90 degrees in any direction; .5 would be 45 degrees"), Instance.aimSpread);
					Instance.aimSpreadRate = EditorGUILayout.FloatField(new GUIContent("  Aim Spread Rate: ", "The rate at which the weapon's spread increases while firing in aim mode. This value is added to the spread after every shot"), Instance.aimSpreadRate);
					Instance.crouchSpreadModifier = EditorGUILayout.FloatField(new GUIContent("  Crouch Spread Modifier: ", "How the weapon's spread is modified while crouching. The spread while crouching is multiplied by this number"), Instance.crouchSpreadModifier);
					Instance.proneSpreadModifier = EditorGUILayout.FloatField(new GUIContent("  Prone Spread Modifier: ", "How the weapon's spread is modified while prone. The spread while prone is multiplied by this number"), Instance.proneSpreadModifier);
					Instance.moveSpreadModifier = EditorGUILayout.FloatField(new GUIContent("  Move Spread Modifier: ", "How the weapon's spread is modified while moving. The spread while moving is multiplied by this number"), Instance.moveSpreadModifier);
					EditorGUILayout.Separator();
				}
				if (gunTipo != GunScript.gunTypes.melee)
				{
					Instance.kickbackAngle = EditorGUILayout.FloatField(new GUIContent("  Vertical Recoil (Angle): ", "The maximum vertical angle per shot which the user's view will be incremented by"), Instance.kickbackAngle);
					Instance.xKickbackFactor = EditorGUILayout.FloatField(new GUIContent("  Horizontal Recoil (Factor): ", "Factor relative to vertical recoil which horizontal recoil will use"), Instance.xKickbackFactor);
					Instance.maxKickback = EditorGUILayout.FloatField(new GUIContent("  Maximum Recoil: ", "The maximum TOTAL angle on the x axis the weapon can recoil"), Instance.maxKickback);
					Instance.recoilDelay = EditorGUILayout.FloatField(new GUIContent("  Recoil Delay: ", "The time before the weapon returns to its normal position from recoil"), Instance.recoilDelay);

					Instance.kickbackAim = EditorGUILayout.FloatField(new GUIContent("  Aim Recoil (Angle): ", "Aim recoil in degrees"), Instance.kickbackAim);
					Instance.crouchKickbackMod = EditorGUILayout.FloatField(new GUIContent("  Crouch Recoil (Multi): ", "Crouch recoil multiplier"), Instance.crouchKickbackMod);
					Instance.proneKickbackMod = EditorGUILayout.FloatField(new GUIContent("  Prone Recoil (Multi): ", "Prone recoil multiplier"), Instance.proneKickbackMod);
					Instance.moveKickbackMod = EditorGUILayout.FloatField(new GUIContent("  Move Recoil (Multi): ", "Move recoil multiplier"), Instance.moveKickbackMod);
				}
				EditorGUILayout.Separator();
				EditorGUILayout.EndVertical();
			}


			//Alt-Fire
			EditorGUILayout.BeginVertical("toolbar");
			Instance.altFireFoldout = EditorGUILayout.Foldout(Instance.altFireFoldout, "Alt-Fire Properties:");
			EditorGUILayout.EndVertical();

			if (Instance.altFireFoldout)
			{
				EditorGUILayout.BeginVertical("textField");
				EditorGUILayout.Separator();
				Instance.isPrimaryWeapon = EditorGUILayout.Toggle(new GUIContent("  Primary Weapon:  ", "Is this a primary weapon? Uncheck only if this gunscript is for an alt-fire"), Instance.isPrimaryWeapon);
				if (Instance.isPrimaryWeapon)
				{
					Instance.secondaryWeapon = (GunScript)EditorGUILayout.ObjectField(new GUIContent("  Secondary Weapon: ", "Optional alt-fire for weapon"), Instance.secondaryWeapon, typeof(GunScript), true);
					if (Instance.secondaryWeapon != null)
					{
						Instance.secondaryInterrupt = EditorGUILayout.Toggle(new GUIContent("  Secondary Interrupt: ", "Can you interrupt the firing animation to switch to alt-fire mode?"), Instance.secondaryInterrupt);
						Instance.secondaryFire = EditorGUILayout.Toggle(new GUIContent("  Instant Fire: ", "Does this alt-fire fire immediately? If unchecked, it will have to be switched to"), Instance.secondaryFire);
						if (Instance.secondaryFire == false)
						{
							Instance.enterSecondaryTime = EditorGUILayout.FloatField(new GUIContent("  Enter Secondary Time: ", "The time in seconds to transition to alt-fire mode"), Instance.enterSecondaryTime);
							Instance.exitSecondaryTime = EditorGUILayout.FloatField(new GUIContent("  Exit Secondary Time: ", "The time in seconds to transition out of alt-fire mode"), Instance.exitSecondaryTime);
						}
					}

				}
				EditorGUILayout.Separator();
				EditorGUILayout.EndVertical();
			}

			//Ammo + Reload
			if (gunTipo != GunScript.gunTypes.melee)
			{
				EditorGUILayout.BeginVertical("toolbar");
				Instance.ammoReloadFoldout = EditorGUILayout.Foldout(Instance.ammoReloadFoldout, "Ammo + Reloading:");
				EditorGUILayout.EndVertical();
			}
			else
			{
				Instance.ammoReloadFoldout = false;
			}

			if (Instance.ammoReloadFoldout)
			{
				EditorGUILayout.BeginVertical("textField");
				EditorGUILayout.Separator();
				if (gunTipo == GunScript.gunTypes.hitscan || gunTipo == GunScript.gunTypes.launcher)
				{
					if (Instance.chargeWeapon)
					{
						Instance.additionalAmmoPerCharge = EditorGUILayout.FloatField(new GUIContent("  Ammo Cost Per Charge: ", "For each charge level, increase the ammo cost of firing the weapon by this amount. Works on integer intervals of charge level only."), Instance.additionalAmmoPerCharge);
					}
				}
				else if (gunTipo == GunScript.gunTypes.melee)
				{
					Instance.ammoPerClip = 1;
					Instance.infiniteAmmo = true;
					Instance.sharesAmmo = false;
				}
				if (gunTipo != GunScript.gunTypes.melee)
				{
					if (!Instance.progressiveReload)
					{
						Instance.ammoType = (GunScript.ammoTypes)EditorGUILayout.EnumPopup(new GUIContent("  Ammo Type: ", "Does the ammo count refer to the number of clips remaining, or the number of individual shots remaining?"), Instance.ammoType);
					}
					Instance.ammoPerClip = EditorGUILayout.IntField(new GUIContent("  Ammo Per Clip: ", "The number of shots that can be fired before needing to reload"), Instance.ammoPerClip);
					if (gunTipo != GunScript.gunTypes.spray)
					{
						Instance.ammoPerShot = EditorGUILayout.IntField(new GUIContent("  Ammo Used Per Shot: ", "The amout of ammo used every time the gun is fired"), Instance.ammoPerShot);
					}
					else
					{
						Instance.ammoPerShot = EditorGUILayout.IntField(new GUIContent("  Ammo Used Per Tick: ", "The amount of ammo drained every time ammo is drained. By default, ammo is drained once per second"), Instance.ammoPerShot);
						Instance.deltaTimeCoefficient = EditorGUILayout.FloatField(new GUIContent("  Drain Coefficient: ", "By default, ammo is drained every second. The rate at which ammo is drained is multiplied by this value"), Instance.deltaTimeCoefficient);
					}
					Instance.sharesAmmo = EditorGUILayout.Toggle(new GUIContent("  Shares Ammo:  ", "If checked, this gun will be able to have a shared ammo reserve with one or more other weapons"), Instance.sharesAmmo);
					if (!Instance.sharesAmmo)
					{
						Instance.infiniteAmmo = EditorGUILayout.Toggle(new GUIContent("  Infinite Ammo: ", "If checked, this weapon will have infinite ammo"), Instance.infiniteAmmo);
						Instance.clips = EditorGUILayout.IntField(new GUIContent("  Clips: ", "The amount of ammo for this weapon that the player has - either clips or bullets, depending on settings"), Instance.clips);
						Instance.maxClips = EditorGUILayout.IntField(new GUIContent("  Max Clips: ", "The maximum amount of ammo for this weapon that the player can carry"), Instance.maxClips);
					}
					else
					{
						Instance.managerObject = GameObject.FindWithTag("Manager");
						AmmoManager ammoManager = Instance.managerObject.GetComponent<AmmoManager>();
						string[] popupContent = ammoManager.namesArray;
						int tempAmmoSet = Instance.ammoSetUsed;
						if (ammoManager.namesArray[0] == name)
						{
							System.Array.Copy(ammoManager.tempNamesArray, ammoManager.namesArray, ammoManager.tempNamesArray.Length);//.ToBuiltin(string);
						}
						Instance.ammoSetUsed = EditorGUILayout.Popup("  Ammo Set Used:  ", tempAmmoSet, popupContent);
						ammoManager.namesArray[Instance.ammoSetUsed] = EditorGUILayout.TextField(new GUIContent("  Rename Ammo Set:", "Type a new name for the ammo set"), ammoManager.namesArray[Instance.ammoSetUsed]);
						ammoManager.infiniteArray[Instance.ammoSetUsed] = EditorGUILayout.Toggle(new GUIContent("  Infinite Ammo: ", "If checked, this set will have infinite ammo"), ammoManager.infiniteArray[Instance.ammoSetUsed]);
						ammoManager.clipsArray[Instance.ammoSetUsed] = EditorGUILayout.IntField(new GUIContent("  Clips: ", "The amount of ammo the player has in this set - either clips or bullets, depending on settings"), ammoManager.clipsArray[Instance.ammoSetUsed]);
						ammoManager.maxClipsArray[Instance.ammoSetUsed] = EditorGUILayout.IntField(new GUIContent("  Max Clips: ", "The maximum amount of this type of ammo that the player can carry"), ammoManager.maxClipsArray[Instance.ammoSetUsed]);
					}
					EditorGUILayout.Separator();
					Instance.reloadTime = EditorGUILayout.FloatField(new GUIContent("  Reload Time: ", "The time it takes to load the weapon if the user presses the reload key"), Instance.reloadTime);
					Instance.progressiveReset = EditorGUILayout.Toggle(new GUIContent("  Clear Reload: ", "If enabled, the gun will always start reloading at 0 rounds loaded, rather than the amount remaining in the clip"), Instance.progressiveReset);
					Instance.progressiveReload = EditorGUILayout.Toggle(new GUIContent("  Progressive Reloading: ", "Do you reload this weapon one bullet/shell/whatever at a time?"), Instance.progressiveReload);
					if (Instance.progressiveReload)
					{
						Instance.reloadInTime = EditorGUILayout.FloatField(new GUIContent("  Enter Reload Time: ", "The time it takes to start the reload cycle"), Instance.reloadInTime);
						Instance.reloadOutTime = EditorGUILayout.FloatField(new GUIContent("  Exit Reload Time: ", "The time it takes to exit the reload cycle"), Instance.reloadOutTime);
						Instance.addOneBullet = false;
					}
					if (!Instance.progressiveReload)
					{
						Instance.addOneBullet = EditorGUILayout.Toggle(new GUIContent("  Partial Reload Bonus: ", "If enabled, the player will retain an additional round in the chamber when manually reloading"), Instance.addOneBullet);
					}
					//if(Instance.addOneBullet){
					Instance.emptyReloadTime = EditorGUILayout.FloatField(new GUIContent("  Empty Reload Time:  ", "The time it takes to reload the weapon if the user has completely emptied the weapon. This can be the same as the Reload Time"), Instance.emptyReloadTime);
					//} else {
					//	Instance.emptyReloadTime = Instance.reloadTime;
					//}
					Instance.waitforReload = EditorGUILayout.FloatField(new GUIContent("  Wait For Reload: ", "The time between pressing the reload key, and actually starting to reload"), Instance.waitforReload);
					EditorGUILayout.Separator();
				}
				EditorGUILayout.EndVertical();
			}

			//Audio/Visual
			EditorGUILayout.BeginVertical("toolbar");
			Instance.audioVisualFoldout = EditorGUILayout.Foldout(Instance.audioVisualFoldout, "Audio + Visual:");
			EditorGUILayout.EndVertical();

			if (Instance.audioVisualFoldout)
			{
				EditorGUILayout.BeginVertical("textField");
				EditorGUILayout.Separator();
				if (gunTipo == GunScript.gunTypes.hitscan || gunTipo == GunScript.gunTypes.launcher)
				{
					Instance.delay = EditorGUILayout.FloatField(new GUIContent("  Delay: ", "The delay between when the fire animation starts and when the gun actually fires"), Instance.delay);
					EditorGUILayout.Separator();
					if (gunTipo != GunScript.gunTypes.launcher)
					{
						Instance.tracer = (GameObject)EditorGUILayout.ObjectField(new GUIContent("  Tracer: ", "This optional field takes game object with a particle emitter to be used for tracer fire"), Instance.tracer, typeof(GameObject), true);
						Instance.traceEvery = EditorGUILayout.IntField(new GUIContent("  Shots Per Tracer: ", "How many shots before displaying the tracer effect?"), Instance.traceEvery);
						Instance.simulateTime = EditorGUILayout.FloatField(new GUIContent("  Simulate Tracer for: ", "The amount of time to simulate your tracer particle system before rendering the particles. Useful if your tracers have to start being emit from behind the muzzle of the gun"), Instance.simulateTime);
						EditorGUILayout.Separator();
					}
					if (gunTipo != GunScript.gunTypes.hitscan)
					{
						Instance.shellEjection = false;
					}
					else
					{
						Instance.shellEjection = EditorGUILayout.Toggle(new GUIContent("  Shell Ejection: ", "Does this weapon have shell ejection?"), Instance.shellEjection);
					}
					if (Instance.shellEjection)
					{
						EditorGUILayout.BeginVertical("textfield");
						Instance.shell = (GameObject)EditorGUILayout.ObjectField(new GUIContent("  Shell: ", "The GameObject to be instantiated when a shell is ejected"), Instance.shell, typeof(GameObject), false);
						Instance.ejectorPosition = (GameObject)EditorGUILayout.ObjectField(new GUIContent("  Ejector Position: ", "Shells will be instantiated from the position of this GameObject"), Instance.ejectorPosition, typeof(GameObject), true);
						Instance.ejectDelay = EditorGUILayout.FloatField("Delay", Instance.ejectDelay);
						EditorGUILayout.EndVertical();
						EditorGUILayout.Separator();
					}
				}
				Instance.sway = EditorGUILayout.Toggle(new GUIContent("  Sway: ", "Does this weapon use coded weapon sway?"), Instance.sway);
				if (Instance.sway)
				{
					EditorGUILayout.BeginVertical("textField");
					Instance.overwriteSway = EditorGUILayout.Toggle(new GUIContent("  Override Rates: ", "Does this weapon override the global sway rates?"), Instance.overwriteSway);

					if (Instance.overwriteSway)
						Instance.moveSwayRate = EditorGUILayout.Vector2Field("  Move Sway Rate:  ", Instance.moveSwayRate);
					Instance.moveSwayAmplitude = EditorGUILayout.Vector2Field("  Move Sway Amplitude:  ", Instance.moveSwayAmplitude);

					if (Instance.overwriteSway)
						Instance.runSwayRate = EditorGUILayout.Vector2Field("  Run Sway Rate:  ", Instance.runSwayRate);
					Instance.runAmplitude = EditorGUILayout.Vector2Field("  Run Sway Amplitude:  ", Instance.runAmplitude);

					if (Instance.overwriteSway)
						Instance.idleSwayRate = EditorGUILayout.Vector2Field("  Idle Sway Rate:  ", Instance.idleSwayRate);
					Instance.idleAmplitude = EditorGUILayout.Vector2Field("  Idle Sway Amplitude:  ", Instance.idleAmplitude);
					EditorGUILayout.EndVertical();
					EditorGUILayout.Separator();
				}

				Instance.useZKickBack = EditorGUILayout.Toggle(new GUIContent("  Use Z KickBack: ", "Does the gun move along the z axis when firing"), Instance.useZKickBack);
				if (Instance.useZKickBack)
				{
					EditorGUILayout.BeginVertical("textfield");
					Instance.kickBackZ = EditorGUILayout.FloatField(new GUIContent("  Z Kickback: ", "The rate at which the gun moves backwards when firing"), Instance.kickBackZ);
					Instance.zRetRate = EditorGUILayout.FloatField(new GUIContent("  Z Return: ", "The rate at which the gun returns to position when not"), Instance.zRetRate);
					Instance.maxZ = EditorGUILayout.FloatField(new GUIContent("  Z Max: ", "The maximum amount the gun can kick back along the z axis"), Instance.maxZ);
					EditorGUILayout.EndVertical();
				}

				Instance.timeToIdle = EditorGUILayout.FloatField(new GUIContent("  Idle Time: ", "The amount of time the player must be idle to start playing the idle animation"), Instance.timeToIdle);
				Instance.overrideAvoidance = EditorGUILayout.Toggle(new GUIContent("  Override Avoidance: ", "Does this weapon override the global object avoidance settings"), Instance.overrideAvoidance);
				if (Instance.overrideAvoidance)
				{
					EditorGUILayout.BeginVertical("textField");
					Instance.avoids = EditorGUILayout.Toggle(new GUIContent("  Use Avoidance: ", "Does this weapon use object avoidance"), Instance.avoids);
					if (Instance.avoids)
					{
						Instance.dist = EditorGUILayout.FloatField(new GUIContent("  Avoid Start Dist: ", "The distance from an object at which object avoidance will begin"), Instance.dist);
						Instance.minDist = EditorGUILayout.FloatField(new GUIContent("  Avoid Closest Dist: ", "The distance form an object at which avoidance will be maximized"), Instance.minDist);
						Instance.pos = EditorGUILayout.Vector3Field("  Avoid Position: ", Instance.pos);
						Instance.rot = EditorGUILayout.Vector3Field("  Avoid Rotation: ", Instance.rot);
					}
					EditorGUILayout.EndVertical();
				}
				/*
				bool  overrideAvoidance = false; //Does this weapon override global object avoidance values
				bool  avoids = true;
				Vector3 rot;
				Vector3 pos;
				float dist = 2;
				float minDist = 1.5f;
				*/
				Instance.takeOutTime = EditorGUILayout.FloatField(new GUIContent("  Take Out Time: ", "The time it takes to take out the weapon"), Instance.takeOutTime);
				Instance.putAwayTime = EditorGUILayout.FloatField(new GUIContent("  Put Away Time: ", "The time it takes to put away the weapon"), Instance.putAwayTime);
				if (gunTipo == GunScript.gunTypes.hitscan || gunTipo == GunScript.gunTypes.launcher)
				{
					if (!Instance.autoFire && !Instance.burstFire)
					{
						Instance.fireAnim = EditorGUILayout.Toggle(new GUIContent("  Morph Fire Anim to Fit: ", "Maches the fire animation's speed to the time it takes to fire"), Instance.fireAnim);
					}
				}
				EditorGUILayout.Separator();
				Instance.fireSound = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("  Fire Sound: ", "The sound to play when each shot is fired"), Instance.fireSound, typeof(AudioClip), false);
				if (gunTipo == GunScript.gunTypes.spray)
				{
					Instance.loopSound = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("  Looping Fire Sound: ", "The sound to loop while the weapon is firing"), Instance.loopSound, typeof(AudioClip), false);
					Instance.releaseSound = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("  Stop Firing Sound: ", "The sound to play when the weapon stops firing"), Instance.releaseSound, typeof(AudioClip), false);
				}
				if (gunTipo == GunScript.gunTypes.hitscan || gunTipo == GunScript.gunTypes.launcher)
				{
					if (Instance.chargeWeapon)
					{
						Instance.chargeLoop = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("  Charging Sound: ", "The sound to be looped while the weapon is charging"), Instance.chargeLoop, typeof(AudioClip), false);
					}
				}
				EditorGUILayout.LabelField(new GUIContent("  Sound Pitch: ", "The pitch to play the sound clip at. A value of 1 will play the sound at its natural pitch"));
				Instance.firePitch = EditorGUILayout.Slider(Instance.firePitch, -3, 3);
				EditorGUILayout.LabelField(new GUIContent("   Sound Volume", "Volume of Fire Sound"));
				Instance.fireVolume = EditorGUILayout.Slider(Instance.fireVolume, 0, 1);//EditorGUILayout.FloatField(new GUIContent("  Sound Volume: ","The colume to play the sound clip at. A value of 1 will play the sound at max volume"), Instance.fireVolume);
				EditorGUILayout.Separator();

				if (gunTipo != GunScript.gunTypes.melee)
				{
					Instance.emptySound = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("  Empty Sound: ", "The sound to play when sry firing"), Instance.emptySound, typeof(AudioClip), false);
					if (Instance.emptySound != null)
					{
						EditorGUILayout.LabelField(new GUIContent("  Sound Pitch: ", "The pitch to play the sound clip at. A value of 1 will play the sound at its natural pitch"));
						Instance.emptyPitch = EditorGUILayout.Slider(Instance.emptyPitch, -3, 3);
						EditorGUILayout.LabelField(new GUIContent("   Sound Volume", "Volume of Empty Sound"));
						Instance.emptyVolume = EditorGUILayout.Slider(Instance.emptyVolume, 0, 1);//EditorGUILayout.FloatField(new GUIContent("  Sound Volume: ","The colume to play the sound clip at. A value of 1 will play the sound at max volume"), Instance.fireVolume);
					}
				}

				EditorGUILayout.Separator();
				Instance.crosshairObj = (GameObject)EditorGUILayout.ObjectField(new GUIContent("  Crosshair Plane: ", "Only use this if you are using a custom crosshair. Refer to documentation if needed"), Instance.crosshairObj, typeof(GameObject), true);
				if (Instance.crosshairObj != null)
				{
					Instance.crosshairSize = EditorGUILayout.FloatField(new GUIContent("  Crosshair Size: ", "The size of the default crosshair"), Instance.crosshairSize);
					Instance.scale = EditorGUILayout.Toggle(new GUIContent("  Scale Crosshair: ", "Does the crosshair scale with accuracy? If disabled, the crosshair will always be a fixed size"), Instance.scale);
				}
				EditorGUILayout.Separator();
				EditorGUILayout.EndVertical();
			}


			EditorGUILayout.Separator();

			//		EditorGUILayout.Separator();
			string tempText;
			if (Instance.gunDisplayed)
			{
				tempText = "Deactivate Weapon";
			}
			else
			{
				tempText = "Activate Weapon";
			}
			if (GUILayout.Button(new GUIContent(tempText, "Toggle whether or not the gun is active"), "miniButton"))
			{
				if (!Instance.gunDisplayed)
				{
					Instance.gunDisplayed = true;
					Instance.EditorSelect();
				}
				else if (Instance.gunDisplayed)
				{
					Instance.gunDisplayed = false;
					Instance.EditorDeselect();
				}
			}
			if (GUI.changed)
				EditorUtility.SetDirty(Instance);
			EditorGUILayout.Separator();
		}
	}
}