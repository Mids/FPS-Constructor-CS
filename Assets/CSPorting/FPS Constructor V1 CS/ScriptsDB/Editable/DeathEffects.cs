using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	public class DeathEffects : MonoBehaviour
	{

		public AnimationClip deathAnim;
		private bool dead = false;
		public Texture deadTexture;
		public float menuDelay;
		private float menuTime;
		public float menuSpeed;

		public void Death()
		{
			Destroy(this.GetComponent<GunChildAnimation>());
			GetComponent<Animation>().clip = deathAnim;
			GetComponent<Animation>().Play();
			dead = true;
			menuTime = Time.time + menuDelay;
		}

		void OnGUI()
		{
			if (!dead)
			{
				return;
			}
			float temp = Mathf.Clamp(Time.time - menuTime, 0, 1 / menuSpeed);
			GUI.color = new Color(1, 1, 1, temp * menuSpeed);
			if (deadTexture != null)
				GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), deadTexture);
			if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 20, 200, 40), "Try Again?") && temp >= .8 / menuSpeed)
			{
				PlayerWeapons.player.BroadcastMessage("UnFreeze");
				Application.LoadLevel(0);
			}
		}
	}
}