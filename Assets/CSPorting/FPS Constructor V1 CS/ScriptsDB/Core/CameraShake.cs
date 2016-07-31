using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	public class CameraShake : MonoBehaviour
	{
		public float multiplier = 1;
		private float curAmp;
		private Vector3 lastVal;
		private float timeVal;
		private float amplitude;
		private float r;

		//public static CameraShake[] shakers;
		public static List<CameraShake> tempShakers = new List<CameraShake>();
		public static bool builtin = false;

		void Awake()
		{
			tempShakers.Add(this);
		}

		void Start()
		{
			//이게 뭣 짓거리인지 모르겠다...
			/*
			if (!builtin)
			{
				shakers = tempShakers.ToArray();
				builtin = true;
			}*/
		}


		public static void ShakeCam(float a, float rT, float time)
		{
			foreach (CameraShake s in tempShakers)
			{
				s.Shake(a, rT, time);
			}
		}

		public void Shake(float a, float rT, float time)
		{
			amplitude = a * multiplier;
			curAmp = amplitude;
			timeVal = time;
			r = rT;
		}

		void LateUpdate()
		{
			//if(InputDB.GetButtonDown("Interact")){
			//	ShakeCam(.14, 10, .4);
			//}
			if (curAmp > 0)
			{
				Vector3 amt = Random.insideUnitSphere * curAmp;
				transform.localPosition -= lastVal;
				transform.localEulerAngles -= lastVal * r;
				transform.localEulerAngles += amt * r;
				transform.localPosition += amt;
				lastVal = amt;
				curAmp -= Time.deltaTime * amplitude / timeVal;
			}
		}
	}
}