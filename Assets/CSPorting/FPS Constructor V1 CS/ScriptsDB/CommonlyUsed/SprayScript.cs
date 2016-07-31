using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class SprayScript : MonoBehaviour
	{
		[HideInInspector] public GunScript gunScript;
		private float trueDamage = 0;
		private float trueForce = 0;
		[HideInInspector] public bool isActive = false;
		private ParticleEmitter[] emitters;
		private int i = 0;

		void Awake()
		{
			gunScript = transform.parent.GetComponent<GunScript>();
			emitters = gameObject.GetComponentsInChildren<ParticleEmitter>();
			isActive = false;
		}

		void OnParticleCollision(GameObject hitObj)
		{
			float dist = Vector3.Distance(hitObj.transform.position, transform.position);
			trueDamage = gunScript.damage;
			if (dist > gunScript.maxFalloffDist)
			{
				trueDamage = gunScript.damage * Mathf.Pow(gunScript.falloffCoefficient, (gunScript.maxFalloffDist - gunScript.minFalloffDist) / gunScript.falloffDistanceScale) * Time.deltaTime;
			}
			else if (dist < gunScript.maxFalloffDist && dist > gunScript.minFalloffDist)
			{
				trueDamage = gunScript.damage * Mathf.Pow(gunScript.falloffCoefficient, (dist - gunScript.minFalloffDist) / gunScript.falloffDistanceScale) * Time.deltaTime;
			}
			Object[] sendArray = new Object[2];
			sendArray[0] = trueDamage as object as Object;
			sendArray[1] = true as object as Object;
			hitObj.SendMessageUpwards("ApplyDamage", sendArray, SendMessageOptions.DontRequireReceiver);
			trueForce = gunScript.force * Mathf.Pow(gunScript.forceFalloffCoefficient, dist);
			if (hitObj.GetComponent<Rigidbody>())
			{
				Rigidbody rigid = hitObj.GetComponent<Rigidbody>();
				Vector3 vectorForce = -Vector3.Normalize(transform.position - hitObj.transform.position) * trueForce;
				rigid.AddForce(vectorForce);
			}
		}

		public void ToggleActive(bool activate)
		{
			if (activate == false)
			{
				isActive = false;
				foreach (ParticleEmitter emitter in emitters)
				{
					emitter.emit = false;
				}
			}
			else
			{
				isActive = true;
				foreach (ParticleEmitter emitter in emitters)
				{
					emitter.emit = true;
				}
			}
		}
	}
}