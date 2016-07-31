using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
public class HighlightWeapon : MonoBehaviour {

	private bool selected = false;
	private SelectableWeapon info;
	private bool equipped = false;

	void Start()
	{
		info = GetComponent<SelectableWeapon>();
	}

	public void HighlightOn()
	{
		equipped = PickupWeapon.CheckWeapons(gameObject);
		selected = true;
	}

	public void HighlightOff()
	{
		selected = false;
	}

	void OnGUI()
	{
		GUI.skin.box.wordWrap = true;
		if (selected && !DBStoreController.inStore)
		{
			string s = "(Tab) to Select";
			if (equipped)
			{
				s = "(Already Equipped)";
			}
			Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
			GUI.Box(new Rect(pos.x - 77.5f, Screen.height - pos.y - (Screen.height / 4) - 52.5f, 155, 105), info.WeaponInfo.gunName + "\n \n" + info.WeaponInfo.gunDescription + "\n" + s);
		}
	}
}
}