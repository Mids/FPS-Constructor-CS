using UnityEngine;
using System.Collections;

public class LevelCtrl : MonoBehaviour
{
	public int Level = 1;
	public int Exp = 0;

    public Ability[] PossibleAbilities;
    private Ability[] _availableOptions = new Ability[3];

    public int dummy = 1;

	private bool _levelUpGUI = false;

	public void Update()
	{
		if (_levelUpGUI)
			if (Input.GetButton("Choose1"))
			{
                _availableOptions[0].Enhance();
				_levelUpGUI = false;
			}
			else if (Input.GetButton("Choose2"))
            {
                _availableOptions[1].Enhance();
                _levelUpGUI = false;
			}
			else if (Input.GetButton("Choose3"))
            {
                _availableOptions[2].Enhance();
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
			GUI.Box(new Rect(10, 10, 300, 120), "1. " +_availableOptions[0].Name+"\n2. "+_availableOptions[1].Name+ "\n3. " + _availableOptions[2].Name);
		}
	}

	public void GainEXP(int exp)
	{
		Exp += exp;
		if (Exp >= 100)
		{
			while (Exp >= 100)
			{
                for(int i=0;i<_availableOptions.Length;i++)
                {
                    _availableOptions[i] = PossibleAbilities[Random.Range(0, PossibleAbilities.Length)];
                }
				Level++;
				Exp -= 100;
				_levelUpGUI = true;
			}
		}
	}
}