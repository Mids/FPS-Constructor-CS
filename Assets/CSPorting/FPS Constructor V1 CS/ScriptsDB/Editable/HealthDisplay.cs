using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
public class HealthDisplay : MonoBehaviour {

	public PlayerHealth playerHealth;

	void Start()
	{
		playerHealth = this.GetComponent<PlayerHealth>();
	}

	void OnGUI()
	{
		GUI.Box(new Rect(10, Screen.height - 30, 100, 20), "Health: " + Mathf.Round(playerHealth.health));
	}
}
}