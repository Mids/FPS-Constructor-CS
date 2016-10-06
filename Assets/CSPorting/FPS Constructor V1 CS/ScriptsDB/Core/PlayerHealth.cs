using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class PlayerHealth : MonoBehaviour
	{
		[HideInInspector]
		public float health = 100;
		public float maxHealth = 100;
		public float hitKickBack;
		public float hitKickBackX;
		public float kickMaxY;
		public float kickMaxX;
		public float kickInt;
		private float nextKickTime = 0;
		public Texture redWindow;
		public float redFlashTime;
		public float directionalFlashTime;
		private float hitEffectTime;
		private float dirEffectTime;
		private GameObject cam;
		public AudioClip[] hitSounds;
		public AudioClip[] landSounds;
		public float hitSoundInterval = .6f;
		public float hitSoundVolume = 1;
		private float nextSoundTime = 0;
		private PlayerWeapons pWeapons;
		private GameObject mainCam;
		public static bool dead = false;
		public AudioClip deathSound;
		public GameObject directionalTexture;

		public static PlayerHealth singleton;

		void Start()
		{
			singleton = this;
			dead = false;
			cam = PlayerWeapons.weaponCam;
			hitEffectTime = 0;
			mainCam = PlayerWeapons.mainCam;
			pWeapons = this.GetComponentInChildren<PlayerWeapons>();
			health = maxHealth;
		}

		public void ApplyFallDamage(float d)
		{
			if (dead)
				return;

			health = Mathf.Clamp(health - d, 0, health);
			HitEffects(d);

			if (health <= 0)
			{
				GetComponent<AudioSource>().clip = deathSound;
				GetComponent<AudioSource>().volume = hitSoundVolume;
				GetComponent<AudioSource>().Play();
				Die();
			}
		}

		public void ApplyDamage(float d)
		{
			if (dead)
				return;
			hitEffectTime = redFlashTime;

			//	float.TryParse(Arr[0], tempFloat);
			health = Mathf.Clamp(health - d, 0, health);
			HitEffects(d);

			if (health <= 0)
			{
				GetComponent<AudioSource>().clip = deathSound;
				GetComponent<AudioSource>().volume = hitSoundVolume;
				GetComponent<AudioSource>().Play();
				Die();
			}
		}

		public void ApplyDamage(object[] Arr)
		{
			if (dead)
				return;

			float tempFloat;
			tempFloat = (float)Arr[0];
			health = Mathf.Clamp(health - tempFloat, 0, health);
			HitEffects(tempFloat);

			if (health <= 0)
			{
				GetComponent<AudioSource>().clip = deathSound;
				GetComponent<AudioSource>().volume = hitSoundVolume;
				GetComponent<AudioSource>().Play();
				Die();
			}
		}

		void OnGUI()
		{
			hitEffectTime -= Time.deltaTime;
			if (hitEffectTime > 0)
			{
				GUI.color = new Color(1, 1, 1, hitEffectTime);
				GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), redWindow);
			}
		}

		public void Direction(Transform h)
		{
			GameObject temp = Instantiate(directionalTexture, transform.position, transform.rotation) as GameObject;
			temp.transform.parent = cam.transform;
			temp.GetComponent<HitDirectional>().Init(h, directionalFlashTime);
		}

		public static void ScreenDamage(float t)
		{
			singleton.hitEffectTime = t;
		}

		public static void ScreenDamage(float damage, Transform where)
		{
			singleton.Direction(where);
			singleton.HitEffects(damage);
		}

		public void HitEffects(float damage)
		{
			hitEffectTime = redFlashTime;
			if (Time.time > nextKickTime)
			{
				cam.GetComponent<MouseLookDBJS>().offsetY = Mathf.Clamp(hitKickBack * damage * Random.value, 0, kickMaxY);
				GameObject.FindWithTag("Player").GetComponent<MouseLookDBJS>().offsetX = Mathf.Clamp(hitKickBackX * damage * Random.value, 0, kickMaxX);
				nextKickTime = Time.time + kickInt;
			}
			if (Time.time > nextSoundTime)
			{
				nextSoundTime = Time.time + hitSoundInterval;
				int temp = Mathf.RoundToInt(Random.value * hitSounds.Length);
				if (temp == 0)
					temp = 1;
				GetComponent<AudioSource>().clip = hitSounds[temp - 1];
				GetComponent<AudioSource>().volume = hitSoundVolume;
				GetComponent<AudioSource>().Play();
			}
		}

		public void Die()
		{
			PlayerWeapons.HideWeapon();
			PlayerWeapons.playerActive = false;
			this.GetComponent<CharacterMotorDB>().canControl = false;
			BroadcastMessage("Death");
			BroadcastMessage("Freeze");
			LockCursor.HardUnlock();
			dead = true;
		}

		public void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.name != "Quad")
			{
				Debug.Log(other.gameObject.name);

			}
			if (other.gameObject.tag.Equals("MONSTERWEAPON"))
			{
				ApplyDamage(10);
			}

		}
	}
}