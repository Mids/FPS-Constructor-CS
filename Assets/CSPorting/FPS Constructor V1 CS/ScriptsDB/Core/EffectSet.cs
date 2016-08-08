using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class EffectSet : MonoBehaviour
	{
		public const int maxOfEach = 15; //You can increase this if desired
		public int setID = 0;
		public string setName = "New Set";
		public int localMax = 20;

		public GameObject blankGameObject;

		public GameObject[] bulletDecals = new GameObject[maxOfEach];
		public bool bulletDecalsFolded = false;
		public int lastBulletDecal = 0;

		public GameObject[] dentDecals = new GameObject[maxOfEach];
		public bool dentDecalsFolded = false;
		public int lastDentDecal = 0;

		public GameObject[] hitParticles = new GameObject[maxOfEach];
		public bool hitParticlesFolded = false;
		public int lastHitParticle = 0;

		public AudioClip[] footstepSounds = new AudioClip[maxOfEach];
		public bool footstepSoundsFolded = false;
		public int lastFootstepSound = 0;

		public AudioClip[] crawlSounds = new AudioClip[maxOfEach];
		public bool crawlSoundsFolded = false;
		public int lastCrawlSound = 0;

		public AudioClip[] bulletSounds = new AudioClip[maxOfEach];
		public bool bulletSoundsFolded = false;
		public int lastBulletSound = 0;

		public AudioClip[] collisionSounds = new AudioClip[maxOfEach];
		public bool collisionSoundsFolded = false;
		public int lastCollisionSound = 0;
	}
}