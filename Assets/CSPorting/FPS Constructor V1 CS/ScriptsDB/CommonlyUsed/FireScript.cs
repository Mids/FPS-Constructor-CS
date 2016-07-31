using System;
using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;
using Object = UnityEngine.Object;

namespace ooparts.fpsctorcs
{
	public class FireScript : MonoBehaviour
	{
		private EffectsManager effectsManager;
		private int shotCountTracer;
		public int traceEvery = 1;

		public void Start()
		{
			effectsManager = GameObject.FindWithTag("Manager").GetComponent<EffectsManager>();
		}

		public void Fire(int penetration, float damage, float force, GameObject tracer, Vector3 direction, Vector3 firePosition)
		{
			//must pass in penetation level, damage, force, tracer object (optional), direction to fire in, and position top fire from.

			bool penetrate = true;
			int pVal = penetration;
			int layer2 = 1 << 2;
			int layerMask = layer2;
			layerMask = ~layerMask;
			RaycastHit[] hits;
			hits = Physics.RaycastAll(firePosition, direction, 100, layerMask);
			shotCountTracer += 1;
			if (tracer != null && traceEvery <= shotCountTracer)
			{
				shotCountTracer = 0;
				if (hits.Length > 0)
				{
					tracer.transform.LookAt(hits[0].point);
				}
				else
				{
					tracer.transform.LookAt((transform.position + 90 * direction));
				}
				tracer.GetComponent<ParticleEmitter>().Emit();
				tracer.GetComponent<ParticleEmitter>().Simulate(0.02f);
			}
			System.Array.Sort(hits, Comparison);
			//	 Did we hit anything?
			for (int i = 0; i < hits.Length; i++)
			{
				RaycastHit hit = hits[i];
				BulletPenetration BP = hit.transform.GetComponent<BulletPenetration>();
				if (penetrate)
				{
					if (BP == null)
					{
						penetrate = false;
					}
					else
					{
						if (pVal < BP.penetrateValue)
						{
							penetrate = false;
						}
						else
						{
							pVal -= BP.penetrateValue;
						}
					}
					//DAmage Array
					// TODO: VERY AWKWARD, needed to send pair or two parameter.
					Object[] sendArray = new Object[2];
					sendArray[0] = (Object) (object) damage;
					sendArray[1] = (Object) (object) false;
					// Send a damage message to the hit object			
					hit.collider.SendMessageUpwards("ApplyDamage", sendArray, SendMessageOptions.DontRequireReceiver);
					hit.collider.SendMessageUpwards("Direction", transform, SendMessageOptions.DontRequireReceiver);
					//And send a message to the decal manager, if the target uses decals
					if (hit.transform.gameObject.GetComponent<UseEffects>())
					{
						//The effectsManager needs five bits of information
						Quaternion hitRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
						int hitSet = hit.transform.gameObject.GetComponent<UseEffects>().setIndex;
						//Array hitInfo = new Array(hit.point, hitRotation, hit.transform, hit.normal, hitSet);
						// Changed
						HitEffectArray hitInfo = new HitEffectArray();
						hitInfo.hitPoint = hit.point;
						hitInfo.hitRotation = hitRotation;
						hitInfo.hitTransform = hit.transform;
						hitInfo.hitNormal = hit.normal;
						hitInfo.hitSet = hitSet;
						effectsManager.SendMessage("ApplyDecal", hitInfo, SendMessageOptions.DontRequireReceiver);
					}
					// Apply a force to the rigidbody we hit
					if (hit.rigidbody)
						hit.rigidbody.AddForceAtPosition(force * direction, hit.point);
				}
			}
			BroadcastMessage("MuzzleFlash", true, SendMessageOptions.DontRequireReceiver);
			GameObject audioObj = new GameObject("GunShot");
			audioObj.transform.position = transform.position;
			audioObj.transform.parent = transform;
			audioObj.AddComponent<TimedObjectDestructorDB>().timeOut = GetComponent<AudioSource>().clip.length + 0.1f;
			AudioSource aO = audioObj.AddComponent<AudioSource>();
			aO.clip = GetComponent<AudioSource>().clip;
			aO.volume = GetComponent<AudioSource>().volume;
			aO.pitch = GetComponent<AudioSource>().pitch;
			aO.Play();
			aO.loop = false;
			aO.rolloffMode = AudioRolloffMode.Linear;
		}

		public int Comparison(RaycastHit x, RaycastHit y)
		{
			float xDistance = x.distance;
			float yDistance = y.distance;

			//TODO: Why not float?
			return (int) (xDistance - yDistance);
		}
	}
}