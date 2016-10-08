using UnityEngine;
using System.Collections;
using System;
using ooparts.fpsctorcs;

public class MaxClip : Ability
{
	GunScript weapon = PlayerWeapons.PW.weapons[PlayerWeapons.PW.selectedWeapon].GetComponent<GunScript>(); //currently equipped weapon	

	public override void Enhance()
	{
		weapon.maxClips++;
	}
}
