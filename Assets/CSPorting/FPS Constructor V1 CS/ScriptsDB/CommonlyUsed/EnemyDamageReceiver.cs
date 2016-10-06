using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class EnemyDamageReceiver : MonoBehaviour
	{
		public float hitPoints = 100.0f;
		public Transform deathEffect;
		public float effectDelay = 0.0f;
		private GameObject[] gos;
		public float multiplier = 1;
		public Rigidbody deadReplacement;
		[HideInInspector] public GameObject playerObject;
		public bool useHitEffect = true;

		[HideInInspector] public bool isEnemy = false;

		public void ApplyDamage(Object[] Arr)
		{
			Debug.Log("1 : "+Arr[1].ToString() + ", 2 : " + Arr[0].ToString());
			//Info array contains damage and value of fromPlayer bool ( true if the player caused the damage)
			//Find the player if we haven't
			if (Arr[1] == true)
			{
				if (!playerObject)
				{
					playerObject = GameObject.FindWithTag("Player");
				}
				if (useHitEffect)
				{
					playerObject.BroadcastMessage("HitEffect");
				}
			}

			// We already have less than 0 hitpoints, maybe we got killed already?
			if (hitPoints <= 0.0f)
				return;
			float tempFloat;
			if (!float.TryParse(Arr[0].ToString(), out tempFloat))
			{
				Debug.LogError("Float parse error!!");
			}

			hitPoints -= tempFloat * multiplier;
			if (hitPoints <= 0.0f)
			{
				// Start emitting particles
				ParticleEmitter emitter = GetComponentInChildren<ParticleEmitter>();
				if (emitter)
					emitter.emit = true;

				Invoke("DelayedDetonate", effectDelay);
			}
		}

		public void ApplyDamagePlayer(float damage)
		{
			Debug.Log("adp : " + damage);
			//Info array contains damage and value of fromPlayer bool ( true if the player caused the damage)
			//Find the player if we haven't
			if (!playerObject)
			{
				playerObject = GameObject.FindWithTag("Player");
			}
			if (useHitEffect)
			{
				playerObject.BroadcastMessage("HitEffect");
			}

			// We already have less than 0 hitpoints, maybe we got killed already?
			if (hitPoints <= 0.0f)
				return;
			//float.TryParse(Arr[0], tempFloat);
			hitPoints -= damage * multiplier;
			if (hitPoints <= 0.0f)
			{
				// Start emitting particles
				ParticleEmitter emitter = GetComponentInChildren<ParticleEmitter>();
				if (emitter)
					emitter.emit = true;

				Invoke("DelayedDetonate", effectDelay);
			}

			BroadcastMessage("GotHit", SendMessageOptions.DontRequireReceiver);
		}

		public void ApplyDamage(float damage)
		{
			//Info array contains damage and value of fromPlayer bool ( true if the player caused the damage)
			//Find the player if we haven't

			// We already have less than 0 hitpoints, maybe we got killed already?
			if (hitPoints <= 0.0f)
				return;

			//float.TryParse(Arr[0], tempFloat);
			hitPoints -= damage * multiplier;
			if (hitPoints <= 0.0f)
			{
				// Start emitting particles
				ParticleEmitter emitter = GetComponentInChildren<ParticleEmitter>();
				if (emitter)
					emitter.emit = true;

				Invoke("DelayedDetonate", effectDelay);
			}
		}

		public void DelayedDetonate()
		{
			BroadcastMessage("Detonate");
		}

		public void Detonate()
		{
			if (isEnemy)
				EnemyMovement.enemies--;
			// Create the deathEffect
			if (deathEffect)
				Instantiate(deathEffect, transform.position, transform.rotation);

			// If we have a dead replacement then replace ourselves with it!
			if (deadReplacement)
			{
				Rigidbody dead = Instantiate(deadReplacement, transform.position, transform.rotation) as Rigidbody;

				// For better effect we assign the same velocity to the exploded gameObject
				dead.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;
				dead.angularVelocity = GetComponent<Rigidbody>().angularVelocity;
			}

			// If there is a particle emitter stop emitting and detach so it doesnt get destroyed right away
			ParticleEmitter emitter = GetComponentInChildren<ParticleEmitter>();
			if (emitter)
			{
				emitter.emit = false;
				emitter.transform.parent = null;
			}
			BroadcastMessage("Die", SendMessageOptions.DontRequireReceiver);
//			Destroy(gameObject);
		}
	}
}