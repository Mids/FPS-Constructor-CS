using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class EnemyDrop : MonoBehaviour
	{
		public Transform drops;
		public int min = 0;
		public int max = 5;
		public float force = 5;

		public void Die()
		{
			int amt = Random.Range(min, max);
			int i = 0;
			GameObject t;
			Vector3 dir;
			while (i < amt)
			{
				t = ((GameObject) Instantiate(drops, transform.position + new Vector3(0, 0.5f, 0), transform.rotation));

				dir = Random.insideUnitSphere * force;
				t.GetComponent<Rigidbody>().AddForce(dir, ForceMode.Impulse);
				t.GetComponent<Rigidbody>().AddTorque(dir, ForceMode.Impulse);
				i++;
			}
		}
	}
}