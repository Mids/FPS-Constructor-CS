using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class Laser : MonoBehaviour
	{
		float dps;
		float power;
		float forceRadius;
		float vFactor;

		void OnTriggerStay(Collider other)
		{
			Object[] sendArray = new Object[2];
			sendArray[0] = (Object) (object) (dps * Time.deltaTime);
			sendArray[1] = (Object) (object) true;
			other.SendMessageUpwards("ApplyDamage", sendArray, SendMessageOptions.DontRequireReceiver);
			if (other.GetComponent<Rigidbody>())
				other.GetComponent<Rigidbody>().AddExplosionForce(power, transform.position, forceRadius, vFactor);
		}

		void Finish()
		{
			Destroy(transform.parent);
		}

		void ChargeLevel()
		{
			transform.parent.position = GameObject.FindWithTag("Laser").transform.position;
		}
	}
}