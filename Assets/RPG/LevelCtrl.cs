using UnityEngine;
using System.Collections;

public class LevelCtrl : MonoBehaviour
{
	public int Level = 1;
	public int Exp = 0;

	private bool _levelUpGUI = false;

	public void Update()
	{
		if (_levelUpGUI)
			if (Input.GetButton("Choose1"))
			{
				_levelUpGUI = false;
			}
			else if (Input.GetButton("Choose2"))
			{
				_levelUpGUI = false;
			}
			else if (Input.GetButton("Choose3"))
			{
				_levelUpGUI = false;
			}
	}

	void OnGUI()
	{
		GUI.skin.box.fontSize = 25;
		GUI.Box(new Rect(10, Screen.height - 100, 200, 40), "Level: " + Level + " / " + Exp + "%");

		if (_levelUpGUI)
		{
			GUI.skin.box.fontSize = 25;
			GUI.Box(new Rect(10, 10, 300, 120), "1. Move Speed\n2. Attack Damage\n3. Max Health");
		}
	}

	public void GainEXP(int exp)
	{
		Exp += exp;
		if (Exp >= 100)
		{
			while (Exp >= 100)
			{
				Level++;
				Exp -= 100;
				_levelUpGUI = true;
			}
		}
	}
}