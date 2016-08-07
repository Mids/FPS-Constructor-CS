using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class Laser : MonoBehaviour
	{
		public float dps;
		public float power;
		public float forceRadius;
		public float vFactor;

		public void OnTriggerStay(Collider other)
		{
			Object[] sendArray = new Object[2];
			sendArray[0] = (Object) (object) (dps * Time.deltaTime);
			sendArray[1] = (Object) (object) true;
			other.SendMessageUpwards("ApplyDamage", sendArray, SendMessageOptions.DontRequireReceiver);
			if (other.GetComponent<Rigidbody>())
				other.GetComponent<Rigidbody>().AddExplosionForce(power, transform.position, forceRadius, vFactor);
		}

		public void Finish()
		{
			Destroy(transform.parent);
		}

		public void ChargeLevel()
		{
			transform.parent.position = GameObject.FindWithTag("Laser").transform.position;
		}
	}
}