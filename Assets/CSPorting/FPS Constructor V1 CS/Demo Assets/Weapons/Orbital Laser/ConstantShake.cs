using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class ConstantShake : MonoBehaviour 
	{
		public float amplitude;
		public float time;

		public void Update()
		{
			CameraShake.ShakeCam(amplitude, 10, time);
		}
	}
}
