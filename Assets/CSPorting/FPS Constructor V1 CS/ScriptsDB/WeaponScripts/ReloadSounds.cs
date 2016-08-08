using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class ReloadSounds : MonoBehaviour
	{
		public AudioClip sound1;
		public AudioClip sound2;
		public AudioClip sound3;
		public AudioClip sound4;
		public AudioClip sound5;
		public AudioClip sound6;

		public AudioSource audio;

		public void Start()
		{
			if (audio == null)
				audio = GetComponent<AudioSource>();
			if (audio == null)
				audio = gameObject.AddComponent<AudioSource>();
		}

		public void play1()
		{
			audio.clip = sound1;
			audio.Play();
		}

		public void play2()
		{
			audio.clip = sound2;
			audio.Play();
		}

		public void play3()
		{
			audio.clip = sound3;
			audio.Play();
		}

		public void play4()
		{
			audio.clip = sound4;
			audio.Play();
		}

		public void play5()
		{
			audio.clip = sound5;
			audio.Play();
		}

		public void play6()
		{
			audio.clip = sound6;
			audio.Play();
		}
	}
}