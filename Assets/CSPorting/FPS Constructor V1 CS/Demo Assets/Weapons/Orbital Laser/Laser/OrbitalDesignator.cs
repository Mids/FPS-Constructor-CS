using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class OrbitalDesignator : MonoBehaviour
	{
		public GunScript gscript;
		public Transform designator;
		private Light desigLight;
		public Transform laser;
		public float lockTime = 0;
		public float lockRange;
		public float targetError;
		private bool lockedOn = false;
		public float lockMax;
		public LineRenderer line;

		public void Start()
		{
			desigLight = designator.GetComponentInChildren<Light>();
			line = this.GetComponent<LineRenderer>();
		}

		public void Update()
		{
			if (Random.value < lockTime / lockMax)
			{
				line.enabled = true;
				desigLight.enabled = true;
				desigLight.transform.GetComponent<AudioSource>().volume = lockTime / lockMax;
				desigLight.transform.GetComponent<AudioSource>().pitch = lockTime / lockMax * 3;
				if (!desigLight.transform.GetComponent<AudioSource>().isPlaying)
				{
					desigLight.transform.GetComponent<AudioSource>().Play();
				}
			}
			else
			{
				line.enabled = false;
				desigLight.enabled = false;
			}
			if (lockTime <= 0)
				desigLight.transform.GetComponent<AudioSource>().Stop();

			if (gscript.chargeLevel <= 0)
			{
				lockTime = Mathf.Clamp(lockTime - Time.deltaTime, 0, lockMax);
				lockedOn = false;
				//desigLight.enabled = false;
				line.enabled = false;
				return;
			}
			if (lockTime >= lockMax || lockedOn)
			{
				lockedOn = true;
				if (gscript.chargeLevel < gscript.minCharge)
				{
					gscript.chargeLevel = gscript.minCharge;
				}
				lockTime = 0;
				return;
			}
			if (!lockedOn)
			{
				gscript.chargeLevel = 0.1f;
			}
			if (lockTime > 0)
			{
				Quaternion temp = Quaternion.LookRotation(designator.position - transform.position);
				float tAngle = Quaternion.Angle(transform.rotation, temp);
				if (tAngle <= targetError)
				{
					lockTime = Mathf.Clamp(lockTime + Time.deltaTime, 0, lockMax);
				}
				else
				{
					lockTime = Mathf.Clamp(lockTime - Time.deltaTime, 0, lockMax);
				}
			}
			else
			{
				int layer1 = 1 << PlayerWeapons.playerLayer;
				int layer2 = 1 << 2;
				int layerMask = layer1 | layer2;
				layerMask = ~layerMask;
				RaycastHit hit;
				if (Physics.Raycast(transform.position, transform.TransformDirection(0, 0, 1), out hit, lockRange, layerMask))
				{
					if (lockTime <= 0)
					{
						designator.position = hit.point;
						lockTime = Mathf.Clamp(lockTime + Time.deltaTime, 0, lockMax);
					}
				}
			}
		}
	}
}