using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class GrenadeCharged : MonoBehaviour
	{
		public float delay = 1.0f;
		public float timeOut = 1.0f;
		public bool detachChildren = false;
		public Transform explosion;
		public Transform explosionCharged;
		public bool explodeAfterBounce = false;
		private bool hasCollided = false;
		private float explodeTime;
		public GameObject[] playerThings;
		public float chargeVal;

		void Start()
		{
			explodeTime = Time.time + timeOut;
			playerThings = GameObject.FindGameObjectsWithTag("Player");
			for (int i = 0; i < playerThings.Length; i++)
			{
				if (playerThings[i].GetComponent<Collider>() != null)
				{
					Physics.IgnoreCollision(this.GetComponent<Collider>(), playerThings[i].GetComponent<Collider>());
				}
			}
		}

		IEnumerator HandleCollisionEnter(Collision collision)
		{
			if (hasCollided || !explodeAfterBounce)
				DestroyNow();
			yield return new WaitForSeconds(delay);
			hasCollided = true;
		}

		void OnCollisionEnter(Collision collision)
		{
			StartCoroutine(HandleCollisionEnter(collision));
		}

		public void ChargeLevel(float charge)
		{
			if (charge >= chargeVal)
			{
				this.GetComponent<Rigidbody>().useGravity = false;
				explosion = explosionCharged;
			}
		}

		public void DestroyNow()
		{
			if (detachChildren)
			{
				transform.DetachChildren();
			}
			DestroyObject(gameObject);
			if (explosion)
			{
				Instantiate(explosion, transform.position, transform.rotation);
			}
		}

		void Update()
		{
			Vector3 direction = transform.TransformDirection(Vector3.forward);
			if (Time.time > explodeTime)
			{
				DestroyNow();
			}
		}
	}
}