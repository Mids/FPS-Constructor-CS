using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class MoneyPickup : MonoBehaviour 
	{
		public int amount;
		public float delay;
		private float nextTime = 0;
		public bool limited;
		public int limit;
		private bool removed = false;
		public bool destroyAtLimit = false;

		//Called via message
		//Adds ammo to player
		public void Interact()
		{
			if (Time.time > nextTime && (limit != 0 || !limited))
			{
				//if it has been long enough, and we are either not past our limit or not limited

				nextTime = Time.time + delay; //set next use time
				DBStoreController.singleton.balance += amount; //add up to max
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
				removed = true;

				if (removed)
				{
					limit--;
					removed = false;
				}

				if (limit <= 0 && destroyAtLimit)
				{
					Destroy(gameObject);
				}
			}
		}
	}
}
