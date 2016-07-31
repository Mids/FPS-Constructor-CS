using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	public class FadeLightInstant : MonoBehaviour
	{
		public float delay;
		public float fadeTime;

		private float fadeSpeed;
		private float intensity;
		public float startIntensity = 6;
		private Color color;
		private bool active1 = false;

		void Start()
		{
			if (GetComponent<Light>() == null)
			{
				Destroy(this);
				return;
			}
		}

		void Update()
		{
			if (!active1)
				return;
			if (delay > 0.0f)
			{
				delay -= Time.deltaTime;
			}
			else if (intensity > 0.0f)
			{
				intensity -= fadeSpeed * Time.deltaTime;
				GetComponent<Light>().intensity = intensity;
			}
		}

		public void Activate()
		{
			GetComponent<Light>().intensity = startIntensity;
			intensity = GetComponent<Light>().intensity;
			active1 = true;
			if (fadeTime > 0.0f)
			{
				fadeSpeed = intensity / fadeTime;
			}
			else
			{
				fadeSpeed = intensity;
			}
		}
	}
}