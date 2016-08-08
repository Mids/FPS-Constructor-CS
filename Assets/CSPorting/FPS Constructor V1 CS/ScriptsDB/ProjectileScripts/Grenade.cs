using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class Grenade : MonoBehaviour
	{
		public float delay = 1.0f;
		public float timeOut = 1.0f;
		public bool detachChildren = false;
		public Transform explosion;
		public bool explodeAfterBounce = false;
		private bool hasCollided = false;
		private float explodeTime;
		public GameObject[] playerThings;

		void Start()
		{
			explodeTime = Time.time + timeOut;
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
			RaycastHit hit;
			if (Time.time > explodeTime)
			{
				DestroyNow();
			}
		}
	}
}