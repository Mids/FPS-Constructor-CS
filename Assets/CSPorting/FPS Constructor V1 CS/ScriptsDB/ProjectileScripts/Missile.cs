using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	/// <summary>
	/// 투사체 계열은 하나의 베이스 클래스로 묶어도 무방할듯?
	/// </summary>
	public class Missile : MonoBehaviour
	{
		public float delay = 1.0f;
		public float timeOut = 1.0f;
		public bool detachChildren = false;
		public Transform explosion;
		public bool explodeAfterBounce = false;
		private bool hasCollided = false;
		private float explodeTime;
		private float initiateTime;
		public GameObject[] playerThings;
		public Transform t;
		public float turnSpeed;
		public float flySpeed;
		public float initiatedSpeed;
		public ParticleEmitter em;
		public bool soundPlaying = false;
		public GameObject lockObj;
		private Camera cam;

		//private bool  hasExploded = false;

		void Start()
		{
			explodeTime = Time.time + timeOut;
			initiateTime = Time.time + delay;
			cam = GameObject.FindWithTag("WeaponCamera").GetComponent<Camera>();
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
			LockOnMissile temp;
			temp = GameObject.FindWithTag("Missile").GetComponent<LockOnMissile>();
			t = temp.Target();
			if (t != null)
			{
				lockObj.transform.position = t.position;
				lockObj.transform.parent = null;
			}
		}

		public void DestroyNow()
		{
			if (detachChildren)
			{
				transform.DetachChildren();
			}
			if (lockObj != null)
				Destroy(lockObj);
			DestroyObject(gameObject);
			if (explosion)
				Instantiate(explosion, transform.position, new Quaternion(0, 0, 0, 0));
		}

		void LateUpdate()
		{
			if (lockObj != null)
			{
				if (t != null)
				{
					lockObj.GetComponentInChildren<Renderer>().enabled = true;
					lockObj.transform.position = t.position;
				}
				else
				{
					lockObj.GetComponentInChildren<Renderer>().enabled = false;
				}
				lockObj.transform.LookAt(cam.transform);
			}

			if (Time.time > initiateTime)
			{
				if (!soundPlaying)
				{
					GetComponent<AudioSource>().Play();
					soundPlaying = true;
				}
				if (t != null)
				{
					Quaternion temp;
					temp = Quaternion.LookRotation(t.position - transform.position, Vector3.up);
					transform.rotation = Quaternion.Slerp(transform.rotation, temp, Time.deltaTime * turnSpeed);
				}
				else
				{
					Destroy(lockObj);
				}
				GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.forward) * initiatedSpeed;
				em.emit = true;
			}
			else
			{
				GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.forward) * flySpeed;
				em.emit = false;
			}
			if (Time.time > explodeTime)
			{
				DestroyNow();
			}
		}
	}
}