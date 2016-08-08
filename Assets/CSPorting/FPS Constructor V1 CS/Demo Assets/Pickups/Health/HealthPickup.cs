using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class HealthPickup : MonoBehaviour
	{
		public int amount;
		private PlayerHealth p;
		public float delay;
		public bool destroys = false;
		private float nextTime = 0;
		public bool limited;
		public int limit;

		//Called via message
		//Adds health to player
		public void Interact()
		{
			if (Time.time > nextTime && (limit != 0 || !limited))
			{
				//if it has been long enough, and we are either not past our limit or not limited
				nextTime = Time.time + delay; //set next time
				p = PlayerWeapons.player.GetComponent<PlayerHealth>();
				if (p.health < p.maxHealth)
				{
					//if we aren't at full health already
					p.health = Mathf.Clamp(p.health + amount, p.health, p.maxHealth); //add health up to max
					if (GetComponent<AudioSource>())
					{
						GameObject audioObj = new GameObject("PickupSound");
						audioObj.transform.position = transform.position;
						audioObj.AddComponent<TimedObjectDestructorDB>().timeOut = GetComponent<AudioSource>().clip.length + 0.1f;
						AudioSource aO = audioObj.AddComponent<AudioSource>(); //play sound
						aO.clip = GetComponent<AudioSource>().clip;
						aO.volume = GetComponent<AudioSource>().volume;
						aO.pitch = GetComponent<AudioSource>().pitch;
						aO.Play();
						aO.loop = false;
						aO.rolloffMode = AudioRolloffMode.Linear;
					}
					limit--; //decrement limit
				}
			}
			if (limit <= 0 && destroys)
				Destroy(gameObject);
		}
	}
}