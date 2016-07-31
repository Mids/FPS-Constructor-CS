using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class HitBox : MonoBehaviour
	{
		public GunScript GunScript;
		public float damage;
		public float force;
		private bool isActive = false;
		private EffectsManager effectsManager;

		void Start()
		{
			effectsManager = GameObject.FindWithTag("Manager").GetComponent<EffectsManager>();
		}

		void Update()
		{
			transform.localPosition = Vector3.zero;
			if (GunScript.hitBox)
			{
				isActive = true;
				this.GetComponent<BoxCollider>().isTrigger = false;
			}
			else
			{
				isActive = false;
				this.GetComponent<BoxCollider>().isTrigger = true;
			}
		}

		void OnCollisionEnter(Collision c)
		{
			if (isActive)
			{
				Object[] sendArray = new Object[2];
				sendArray[0] = damage as object as Object;
				sendArray[1] = true as object as Object;
				c.collider.SendMessageUpwards("ApplyDamage", sendArray, SendMessageOptions.DontRequireReceiver);
				if (c.gameObject.GetComponent<UseEffects>())
				{
					int layer1 = 1 << PlayerWeapons.playerLayer;
					int layer2 = 1 << 2;
					int layerMask = layer1 | layer2;
					layerMask = ~layerMask;
					RaycastHit hit;
					if (Physics.Raycast(GunScript.gameObject.transform.position, GunScript.gameObject.transform.forward, out hit, Mathf.Infinity, layerMask))
					{
						//The effectsManager needs five bits of information
						Quaternion hitRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
						int hitSet = c.gameObject.GetComponent<UseEffects>().setIndex;
						// FIXME_VAR_TYPE hitInfo = new Array(hit.point, hitRotation, c.transform, hit.normal, hitSet);
						// Changed
						HitEffectArray hitInfo = new HitEffectArray();
						hitInfo.hitPoint = hit.point;
						hitInfo.hitRotation = hitRotation;
						hitInfo.hitTransform = hit.transform;
						hitInfo.hitNormal = hit.normal;
						hitInfo.hitSet = hitSet;
						effectsManager.SendMessage("ApplyDent", hitInfo, SendMessageOptions.DontRequireReceiver);
					}
				}
				if (c.collider.GetComponent<Rigidbody>() != null)
				{
					c.collider.GetComponent<Rigidbody>().AddForce(c.relativeVelocity * force);
				}
				GunScript.hitBox = false;
				isActive = false;
				GetComponent<AudioSource>().loop = false;
			}
		}
	}
}