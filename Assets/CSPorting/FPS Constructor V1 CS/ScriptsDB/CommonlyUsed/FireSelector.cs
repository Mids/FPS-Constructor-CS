using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class FireSelector : MonoBehaviour
	{
		public GunScript gscript;
		public int state = 0;
		public AudioClip sound;
		public float soundVolume = 1;
		public string anim;

		void Start()
		{
			gscript.autoFire = (state == 0);
			gscript.burstFire = (state == 1);
		}

		void Cycle()
		{
			// changed gscript -> GunScript, because the variable is static.
			if (!gscript.gunActive || AimMode.sprintingPublic || LockCursor.unPaused || GunScript.reloading)
				return;
			GetComponent<AudioSource>().PlayOneShot(sound, soundVolume);
			if (anim != "")
			{
				BroadcastMessage("PlayAnim", anim);
			}
			state++;
			if (state == 3)
				state = 0;

			gscript.autoFire = (state == 0);
			gscript.burstFire = (state == 1);
		}

		void Update()
		{
			if (InputDB.GetButtonDown("Fire2"))
			{
				Cycle();
			}
		}
	}
}