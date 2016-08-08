using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class UpgradePickup : MonoBehaviour
	{
		public bool apply = true;
		public bool own = true;
		public bool mustBeEquipped = true;
		public Upgrade upgrade;
		public bool destroys = false;
		private float nextTime = 0;
		public float delay = 1;
		public bool limited;
		public int limit;

		//Called via message
		//Gives Upgrade
		public void Interact()
		{
			if (Time.time > nextTime && (limit != 0 || !limited) && (upgrade.transform.parent.GetComponent<GunScript>().gunActive || !mustBeEquipped))
			{
				//if it has been long enough, and we are either not past our limit or not limited
				nextTime = Time.time + delay; //set next time
				if ((own && !upgrade.owned) || (apply && !upgrade.applied))
				{
					//if the upgrade isn't already applied
					if (own)
						upgrade.owned = true;
					if (apply)
						upgrade.ApplyUpgrade();
					if (GetComponent<AudioSource>())
					{
						GameObject audioObj = new GameObject("PickupSound");
						audioObj.transform.position = transform.position;
						audioObj.AddComponent<TimedObjectDestructorDB>().timeOut = GetComponent<AudioSource>().clip.length + 0.1f;
						;
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