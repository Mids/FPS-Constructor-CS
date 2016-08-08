using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class EnemyAttack : MonoBehaviour
	{
		private Transform target;
		private float nextAttackTime;
		public float damage;
		public float attackTime;

		public void Start()
		{
			target = PlayerWeapons.weaponCam.transform;
		}

		public void Attack()
		{
			if (Time.time < nextAttackTime)
				return;
			Object[] sendArray = new Object[2];
			sendArray[0] = (Object) (object) damage;
			sendArray[1] = (Object) (object) false;
			target.SendMessageUpwards("ApplyDamage", sendArray, SendMessageOptions.DontRequireReceiver);
			target.SendMessageUpwards("Direction", transform, SendMessageOptions.DontRequireReceiver);
			nextAttackTime = Time.time + attackTime;
			GetComponent<Animation>().Play();
		}
	}
}