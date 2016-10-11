using UnityEngine;
using System.Collections;
using System;
using ooparts.fpsctorcs;

public class MaxClip : Ability
{
	private GunScript weapon;

	public override void Enhance()
	{
		if (weapon == null)
		{
			weapon = PlayerWeapons.PW.weapons[PlayerWeapons.PW.selectedWeapon].GetComponent<GunScript>(); //currently equipped weapon	
		}
		weapon.maxClips+= weapon.ammoPerClip;
	}
}
