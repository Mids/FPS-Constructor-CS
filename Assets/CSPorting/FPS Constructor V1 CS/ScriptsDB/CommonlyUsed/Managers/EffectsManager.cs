using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class EffectsManager : MonoBehaviour
	{
		public const int maxSets = 75; //The maximum number of decal sets. This is an arbitrary number, and can be increased if required for some reason.
		public static int maxDecals = 75; //The maximum number of decals that can exist in the world. This is editable in the inspector

		public EffectSet[] setArray = new EffectSet[maxSets]; //This is an array which stores all the effects sets we have created.
		public string[] setNameArray = new string[1]; //The names of each set are stored in a separate array from the actual effects sets
		public int selectedSet = 0; //The set we are currently viewing in the inspector
		public int highestSet = 0; //Because we use a builtin array for our setArray, the size of the array can never change. This variable allows 
		//us to know how many items in our setArray are actually in use, so we don't display any null entries
		public GameObject audioEvent; //When we instantiate an audio event in the script we will store it here.

		public GameObject[] decalArray = new GameObject[maxDecals]; //Every time we instantiate a decal effect in the world we will track it here
		public int currentDecal = 0; //Just like highestSet, this tracks the highest non-null index in the decalArray

		public GameObject thePlayer; //We find the player object in update and store its info here

		//These variables are used to make sure that multiple bullet sounds are not all played at once.
		public float lastSoundTime = 0;
		public float soundCooldown = .005f;
		public bool canPlay = true; //If this is false, we can't play a sound yet

		public static string DELETED = "deleted"; //When the user deletes an effects set, we have to move every effects set after it to a new location
		//in the array. A deleted set is renamed "deleted" so the system knows to compact the array.
		public static EffectsManager manager; //static access variable so other scripts can access this one.

		void Awake()
		{
			thePlayer = GameObject.FindWithTag("Player");
			manager = this;
		}

		void Update()
		{
			//We want to limit the number of sounds that can be played in a short timeframe, so weapons such as shotguns do not
			//play a hit sound for every single bullet
			if (canPlay == false)
			{
				if (Time.time > lastSoundTime + soundCooldown)
				{
					canPlay = true;
				}
			}
		}

		//This function is called whenever the user adds a new effects set. The new name has to be added to the name array
		public void RebuildNameArray(string str)
		{
			List<string> tempArr = new List<string>();
			bool abort = false;

			if (setNameArray.Length == 0)
			{
				setNameArray = new string[1];
			}

			if (setNameArray[0] == null)
			{
				setNameArray[0] = str;
			}
			else
			{
				for (int i = 0; i < setNameArray.Length; i++)
				{
					tempArr.Add(setNameArray[i]);
				}

				if (abort == false)
				{
					tempArr.Add(str);
					setNameArray = tempArr.ToArray();
				}
			}
		}

		//Called when the user creates a set
		public void CreateSet()
		{
			setArray[highestSet] = new EffectSet();
			RebuildNameArray("Set " + highestSet);
			selectedSet = highestSet;
			highestSet++;
			TrimSetArray();
		}

		//Whenever the user deletes a set, this is called
		public void DeleteSet(int index)
		{
			setArray[index] = null;
			setNameArray[index] = "deleted";
			CompactNameArray();
			CompactSetArray();
			highestSet = setNameArray.Length;
		}

		//This is called inside of DeleteSet. It rebuilds the setNameArray to not include the deleted set.
		public void CompactNameArray()
		{
			List<string> tempArr = new List<string>();
			for (int i = 0; i < setNameArray.Length; i++)
			{
				if (setNameArray[i] != DELETED)
				{
					tempArr.Add(setNameArray[i]);
				}
			}
			setNameArray = tempArr.ToArray();
		}

		//We shouldn't have any sets at indices greater than 'highestSet.' This function just makes sure of that
		public void TrimSetArray()
		{
			for (int i = highestSet; i < maxSets; i++)
			{
				setArray[i] = null;
			}
		}

		//This function is called inside of DeleteSet. It reuilds our array of decal sets to not include the deleted set
		public void CompactSetArray()
		{
			EffectSet[] tmpSetArray = new EffectSet[maxSets];
			int n = 0;
			for (int i = 0; i < maxSets; i++)
			{
				if (setArray[i] != null)
				{
					tmpSetArray[n] = setArray[i];
					n++;
				}
			}
			setArray = tmpSetArray;
		}

		//Renames a Decal Set
		public void Rename(string str)
		{
			bool illegal = false;
			for (int i = 0; i < setNameArray.Length; i++)
			{
				if (setNameArray[i] == str)
				{
					illegal = true;
					Debug.LogWarning("There is already an effects set named " + str + "! Please choose a different name");
				}
			}

			if (!illegal)
			{
				setArray[selectedSet].setName = str;
				setNameArray[selectedSet] = str;
			}
		}

		//Applies hit effects like sparks. Called from either ApplyDecal or ApplyDent
		public void ApplyHitEffect(HitEffectArray info)
		{
			//The info array has hit.point, hitRotation, hit.transform, hit.normal, hitSet
			if (setArray[info.hitSet].hitParticles[0] != null)
			{
				Vector3 tempVector = info.hitPoint;
				Quaternion tempQuat1 = info.hitRotation;
				Transform tempTrans2 = info.hitTransform;
				Vector3 tempVector3 = info.hitNormal;
				int tempInt4 = info.hitSet;

				int toApply = Random.Range(0, setArray[tempInt4].lastHitParticle);
				GameObject clone = Instantiate(setArray[tempInt4].hitParticles[toApply], tempVector, tempQuat1) as GameObject;
				clone.transform.localPosition = clone.transform.localPosition + .01f * tempVector3;
				clone.transform.LookAt(thePlayer.transform.position);
			}
		}

		//This function is called whenever a weapon is fired (so in 'GunScript' for players and 'Fire' for non-players).
		//It is responsible for applying any required decals, hit effects, and hit sounds.
		public void ApplyDecal(HitEffectArray info)
		{
			//The info array has hit.point, hitRotation, hit.transform, hit.normal, hitSet
			if (setArray[0] != null)
			{
				if (setArray[info.hitSet].bulletDecals[0] != null)
				{
					Vector3 tempVector = info.hitPoint;
					Quaternion tempQuat1 = info.hitRotation;
					Transform tempTrans2 = info.hitTransform;
					Vector3 tempVector3 = info.hitNormal;
					int tempInt4 = info.hitSet;

					int toApply = Random.Range(0, setArray[tempInt4].lastBulletDecal);
					GameObject clone = Instantiate(setArray[tempInt4].bulletDecals[toApply], tempVector, tempQuat1) as GameObject;
					clone.transform.localPosition = clone.transform.localPosition + .05f * tempVector3;
					clone.transform.parent = tempTrans2;
					if (currentDecal >= maxDecals)
					{
						currentDecal = 0;
					}
					if (decalArray[currentDecal] != null)
					{
						Destroy(decalArray[currentDecal]);
					}
					decalArray[currentDecal] = clone;
					currentDecal++;
				}
				ApplyHitEffect(info);
				BulletSound(info);
			}
			else
			{
				Debug.LogWarning("EffectsManager: You have at least one object with the UseDecals script attached, but no decal sets. Please create a decal set");
			}
		}

		//Some weapons may apply 'dents' instead of 'decals' - for example, hitting a sheet of metal with a pipe would not make a round bullet hole appear.
		//This functions identically to ApplyDecal, only it is used in cases where dents are applied instead of the default decal.
		public void ApplyDent(HitEffectArray info)
		{
			//The info array has hit.point, hitRotation, hit.transform, hit.normal, hitSet

			if (setArray[0] != null)
			{
				if (setArray[info.hitSet].dentDecals[0] != null)
				{
					Vector3 tempVector = info.hitPoint;
					Quaternion tempQuat1 = info.hitRotation;
					Transform tempTrans2 = info.hitTransform;
					Vector3 tempVector3 = info.hitNormal;
					int tempInt4 = info.hitSet;

					int toApply = Random.Range(0, setArray[tempInt4].lastBulletDecal);
					GameObject clone = Instantiate(setArray[tempInt4].bulletDecals[toApply], tempVector, tempQuat1) as GameObject;
					clone.transform.localPosition += .05f * tempVector3;
					clone.transform.parent = tempTrans2;
					if (currentDecal >= maxDecals)
					{
						currentDecal = 0;
					}
					if (decalArray[currentDecal] != null)
					{
						Destroy(decalArray[currentDecal]);
					}
					decalArray[currentDecal] = clone;
					currentDecal++;
				}
				ApplyHitEffect(info);
				CollisionSound(info);
			}
			else
			{
				Debug.LogWarning("EffectsManager: You have at least one object with the UseDecals script attached, but no decal sets. Please create a decal set");
			}
		}

		//Called by BulletSound and CollisionSound; actually does the legwork of creating an audio event and playing the correct sound
		public void CreateAudioEvent(AudioClip clipToPlay, HitEffectArray info)
		{
			audioEvent = new GameObject("Audio Event");
			Transform t = info.hitTransform;
			audioEvent.transform.position = t.position;
			audioEvent.AddComponent<AudioSource>();

			AudioSource source = audioEvent.GetComponent<AudioSource>();
			//source.rolloffMode = AudioRolloffMode.Linear;
			source.clip = clipToPlay;
			source.Play();

			audioEvent.AddComponent<TimedObjectDestructorDB>();
			audioEvent.GetComponent<TimedObjectDestructorDB>().timeOut = clipToPlay.length + .5f;
			canPlay = false;
			lastSoundTime = Time.time;
		}

		//Called from either ApplyDecal or ApplyDent.
		public void BulletSound(HitEffectArray info)
		{
			//The info array has hit.point, hitRotation, hit.transform, hit.normal, hitSet
			if (setArray[info.hitSet].bulletSounds[0] != null && canPlay)
			{
				int toPlay = Random.Range(0, setArray[info.hitSet].lastBulletSound);
				CreateAudioEvent(setArray[info.hitSet].bulletSounds[toPlay], info);
			}
		}

		//Called from either ApplyDecal or ApplyDent; same as BulletSound except it plays the set's collision sound instead of its bullet sound
		public void CollisionSound(HitEffectArray info)
		{
			//The info array has hit.point, hitRotation, hit.transform, hit.normal, hitSet
			if (setArray[info.hitSet].collisionSounds[0] != null && canPlay)
			{
				int toPlay = Random.Range(0, setArray[info.hitSet].lastCollisionSound);
				CreateAudioEvent(setArray[info.hitSet].collisionSounds[toPlay], info);
			}
		}
	}
}

public class HitEffectArray
{
	public Vector3 hitPoint;
	public Quaternion hitRotation;
	public Transform hitTransform;
	public Vector3 hitNormal;
	public int hitSet;
}