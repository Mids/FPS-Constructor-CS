using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class Target : MonoBehaviour
	{
		public float health;
		private float curHealth;
		private bool dead = false;
		public float dieTime;
		public float minimum;

		public void Start()
		{
			curHealth = health;
		}

		public IEnumerator ApplyDamagePlayer(float damage)
		{
			if (dead)
				yield break;

			float tempFloat;
			//	float.TryParse(Arr[0], tempFloat);
			curHealth -= damage;
			if (curHealth <= 0)
			{
				dead = true;
				this.GetComponent<HingeJoint>().useSpring = false;
				JointLimits jointLimits = GetComponent<HingeJoint>().limits;
				jointLimits.min = -90;
				GetComponent<HingeJoint>().limits = jointLimits;
				yield return new WaitForSeconds(dieTime);
				curHealth = health;
				this.GetComponent<HingeJoint>().useSpring = true;
//				this.GetComponent<HingeJoint>().limits.min = minimum;
				jointLimits = GetComponent<HingeJoint>().limits;
				jointLimits.min = minimum;
				GetComponent<HingeJoint>().limits = jointLimits;
				dead = false;
			}
		}
	}
}