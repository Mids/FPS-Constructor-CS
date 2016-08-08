using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class EnemyShoot : MonoBehaviour
	{
		private Transform target;
		private float nextAttackTime;
		public float damage;
		public float force;
		public float fireRate;
		public Fire fire;
		public GameObject tracer;
		public Transform shootPos;
		public float actualSpread;

		public void Start()
		{
			target = PlayerWeapons.weaponCam.transform;
		}

		public void Attack()
		{
			if (Time.time < nextAttackTime)
				return;
			nextAttackTime = Time.time + fireRate;
			//void  Fire ( int penetration ,   float damage ,   float force ,   GameObject tracer ,   Vector3 direction ,   Vector3 firePosition  ){
			fire.Fire(0, damage, force, tracer, SprayDirection(), shootPos.position);
		}

		public Vector3 SprayDirection()
		{
			float vx = (1 - 2 * Random.value) * actualSpread;
			float vy = (1 - 2 * Random.value) * actualSpread;
			float vz = 1.0f;
			return transform.TransformDirection(new Vector3(vx, vy, vz));
		}
	}
}