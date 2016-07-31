using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	public class Explosion_Other : MonoBehaviour
	{

		public float explosionRadius = 5.0f;
		public float explosionPower = 10.0f;
		public float explosionDamage = 100.0f;
		public float explosionTimeout = 2.0f;
		public float vFactor = 3;
		public float shakeFactor = 1.5f;
		public float minShake = .07f;

		public int highestParent = 0;
		public GameObject[] parentArray;

		public bool AlreadyHit(GameObject GO)
		{  //if this function returns true, we have already hit another child of this object's highest parent
			GameObject toCompare = FindTopParent(GO);
			bool toReturn = false;
			for (int i = 0; i < highestParent; i++)
			{
				if (parentArray[i] == toCompare)
				{
					toReturn = true;
					break;
				}
			}
			if (toReturn == false)
			{
				parentArray[highestParent] = toCompare;
				highestParent++;
			}
			return toReturn;
		}
		//Finds the top parent, *OR* the first parent with EnemyDamageReceiver
		//If the top parent has no EnemyDamageReceiver, it returns the object passed in instead, as if there was no parent
		public GameObject FindTopParent(GameObject GO)
		{
			Transform tempTransform;
			GameObject returnObj;
			bool keepLooping = true;
			if (GO.transform.parent != null)
			{
				tempTransform = GO.transform;
				while (keepLooping)
				{
					if (tempTransform.parent != null)
					{
						tempTransform = tempTransform.parent;
						if (tempTransform.GetComponent<EnemyDamageReceiver>())
						{
							keepLooping = false;
						}
					}
					else
					{
						keepLooping = false;
					}
				}
				if (tempTransform.GetComponent<EnemyDamageReceiver>())
				{
					returnObj = tempTransform.gameObject;
				}
				else
				{
					returnObj = GO;
				}
			}
			else
			{
				returnObj = GO;
			}
			return returnObj;
		}

		IEnumerator Start()
		{
			parentArray = new GameObject[128]; //Arbitrary array size; can be increased
			highestParent = 0;

			Vector3 explosionPosition = transform.position;

			// Apply damage to close by objects first
			Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);
			foreach (var hit in colliders)
			{
				if (AlreadyHit(hit.gameObject) == false)
				{
					// Calculate distance from the explosion position to the closest point on the collider
					Vector3 closestPoint = hit.ClosestPointOnBounds(explosionPosition);
					float distance = Vector3.Distance(closestPoint, explosionPosition);

					// The hit points we apply fall decrease with distance from the explosion point
					float hitPoints = 1.0f - Mathf.Clamp01(distance / explosionRadius);
					if (hit.gameObject.layer == PlayerWeapons.playerLayer)
					{
						CameraShake.ShakeCam(Mathf.Max(hitPoints * shakeFactor, minShake), 10, Mathf.Max(hitPoints * shakeFactor, .3f));
					}
					hitPoints *= explosionDamage;

					// Tell the rigidbody or any other script attached to the hit object how much damage is to be applied!
					if (hit.gameObject.layer != 2)
					{
						object[] sendArray = new object[] { hitPoints, false };
						hit.SendMessageUpwards("ApplyDamage", sendArray, SendMessageOptions.DontRequireReceiver);
						hit.SendMessageUpwards("Direction", transform, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
			// Apply explosion forces to all rigidbodies
			// This needs to be in two steps for ragdolls to work correctly.
			// (Enemies are first turned into ragdolls with ApplyDamage then we apply forces to all the spawned body parts)
			colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);
			foreach (var hit in colliders)
			{
				if (hit.GetComponent<Rigidbody>() && hit.gameObject.layer != LayerMask.NameToLayer("Player"))
					hit.GetComponent<Rigidbody>().AddExplosionForce(explosionPower, explosionPosition, explosionRadius, vFactor);
			}
			// stop emitting particles
			if (GetComponent<ParticleEmitter>())
			{
				GetComponent<ParticleEmitter>().emit = true;
				yield return new WaitForSeconds(0.5f);
				GetComponent<ParticleEmitter>().emit = false;
			}
			// destroy the explosion after a while
			Destroy(gameObject, explosionTimeout);
		}
	}
}