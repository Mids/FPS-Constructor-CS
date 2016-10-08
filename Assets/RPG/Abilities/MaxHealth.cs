using UnityEngine;
using System.Collections;
using System;
using ooparts.fpsctorcs;

public class MaxHealth : Ability
{
	private PlayerHealth _player;
	public int amount = 20;

	public override void Enhance()
	{
		if (_player == null)
		{
			_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
		}
		_player.maxHealth += amount;
	}
}
