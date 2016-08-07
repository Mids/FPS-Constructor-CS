using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class EjectedShell : MonoBehaviour
	{
		public Vector3 force;
		public float randomFactorForce;
		//float gravity;
		public Vector3 torque;
		public float randomFactorTorque;

		public void Start()
		{
			GetComponent<Rigidbody>().AddRelativeForce(force * Random.Range(1, randomFactorForce));
			GetComponent<Rigidbody>().AddRelativeTorque(torque * Random.Range(-randomFactorTorque, randomFactorTorque));
		}
	}
}