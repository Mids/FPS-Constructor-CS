using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class HealthDisplay : MonoBehaviour
	{
		public PlayerHealth playerHealth;

		void Start()
		{
			playerHealth = this.GetComponent<PlayerHealth>();
		}

		void OnGUI()
		{
			GUI.skin.box.fontSize = 25;
			GUI.Box(new Rect(10, Screen.height - 50, 200, 40), "Health: " + Mathf.Round(playerHealth.health));
		}
	}
}