using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	public class FootStep : MonoBehaviour
	{

		public float footstepInterval = .5f;
		public float footstepVolume = .5f;
		private float distanceMoved = 0;
		private Vector3 lastStep;
		private bool landing = false;

		[HideInInspector]
		public EffectsManager effectsManager;
		[HideInInspector]
		public CharacterMotorDB characterMotor;
		[HideInInspector]
		public AudioSource source;
		[HideInInspector]
		public AudioClip soundClip;
		[HideInInspector]
		public int playDex = 0;
		[HideInInspector]
		public GameObject surface;

		void Awake()
		{
			effectsManager = GameObject.FindObjectOfType<EffectsManager>();
			characterMotor = gameObject.GetComponent<CharacterMotorDB>();
			source = gameObject.GetComponent<AudioSource>();
		}

		void Update()
		{
			if (!PlayerWeapons.playerActive)
				return;

			if (!characterMotor.grounded)
			{
				distanceMoved = footstepInterval;
				landing = true;
			}
			distanceMoved += Vector3.Distance(transform.position, lastStep);
			lastStep = transform.position;
			if (CharacterMotorDB.walking)
			{//|| (landing && characterMotor.grounded))){
				if (CharacterMotorDB.prone)
				{
					Crawl();
					landing = false;
				}
				else
				{
					Footstep();
					landing = false;
				}
			}
		}

		public void Airborne()
		{
			if (CharacterMotorDB.prone)
			{
				Crawl();
				landing = false;
			}
			else
			{
				Footstep();
				landing = false;
			}
		}

		public void Landed()
		{
			if (CharacterMotorDB.prone)
			{
				Crawl();
				landing = false;
			}
			else
			{
				Footstep();
				landing = false;
			}
		}

		public void Footstep()
		{
			if (distanceMoved >= footstepInterval)
			{
				GetClip();
				/*source.clip = soundClip;
				source.volume = footstepVolume;
				source.Play();*/
				if (soundClip != null)
				{
					GameObject audioObj = new GameObject("Footstep");
					audioObj.transform.position = transform.position;
					audioObj.transform.parent = transform;
					audioObj.AddComponent<TimedObjectDestructorDB>().timeOut = soundClip.length + .1f;
					AudioSource aO = audioObj.AddComponent<AudioSource>();
					aO.clip = soundClip;
					aO.volume = footstepVolume;
					aO.Play();
					aO.loop = false;
					aO.rolloffMode = AudioRolloffMode.Linear;
				}
				distanceMoved = 0;
			}
		}

		public void Crawl()
		{
			if (distanceMoved >= footstepInterval)
			{
				GetCrawlClip();
				/*source.clip = soundClip;
				source.volume = footstepVolume;
				source.Play();*/
				if (soundClip != null)
				{
					GameObject audioObj = new GameObject("Footstep");
					audioObj.transform.position = transform.position;
					audioObj.transform.parent = transform;
					audioObj.AddComponent<TimedObjectDestructorDB>().timeOut = soundClip.length + .1f;
					AudioSource aO = audioObj.AddComponent<AudioSource>();
					aO.clip = soundClip;
					aO.volume = footstepVolume;
					aO.Play();
					aO.loop = false;
					aO.rolloffMode = AudioRolloffMode.Linear;
				}
				distanceMoved = 0;
			}
		}

		//This function, called by Crawl, gets a random clip and sets soundClip to equal that clip
		public void GetCrawlClip()
		{
			RaycastHit hit;
			if (Physics.Raycast(transform.position, -Vector3.up, out hit, 100, ~(1 << PlayerWeapons.playerLayer)))
			{
				if (hit.transform.GetComponent<UseEffects>())
				{
					UseEffects effectScript = hit.transform.GetComponent<UseEffects>();
					int dex = effectScript.setIndex;
					if (effectsManager.setArray[0] != null)
					{
						if (effectsManager.setArray[dex].crawlSounds != null)
						{
							soundClip = effectsManager.setArray[dex].crawlSounds[playDex];
							if (playDex >= effectsManager.setArray[dex].lastCrawlSound - 1)
							{
								playDex = 0;
							}
							else
							{
								playDex++;
							}
						}
					}
				}
			}
		}

		//This function, called by Footstep, gets a random clip and sets soundClip to equal that clip
		public void GetClip()
		{
			RaycastHit hit;
			if (Physics.Raycast(transform.position, -Vector3.up, out hit, 100, ~(1 << PlayerWeapons.playerLayer)))
			{
				if (hit.transform.GetComponent<UseEffects>())
				{
					UseEffects effectScript = hit.transform.GetComponent<UseEffects>();
					int dex = effectScript.setIndex;
					if (effectsManager.setArray[0] != null)
					{
						if (effectsManager.setArray[dex].footstepSounds != null)
						{
							soundClip = effectsManager.setArray[dex].footstepSounds[playDex];
							if (playDex >= effectsManager.setArray[dex].lastFootstepSound - 1)
							{
								playDex = 0;
							}
							else
							{
								playDex++;
							}
						}
					}
				}
			}
		}
	}
}