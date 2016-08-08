using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class FadeLight : MonoBehaviour
	{
		public float delay;
		public float fadeTime;

		private float fadeSpeed;
		private float intensity;
		private Color color;

		public void Start()
		{
			if (GetComponent<Light>() == null)
			{
				Destroy(this);
				return;
			}

			intensity = GetComponent<Light>().intensity;


			fadeTime = Mathf.Abs(fadeTime);

			if (fadeTime > 0.0f)
			{
				fadeSpeed = intensity / fadeTime;
			}
			else
			{
				fadeSpeed = intensity;
			}
			//alpha = 1.0f;
		}

		public void Update()
		{
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
	}
}