using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class EnemyShootProjectile : MonoBehaviour
	{
		private float nextAttackTime;
		public Transform pos;
		public Rigidbody projectile;
		public float initialSpeed = 50;
		public float fireRate = 1;
		public float actualSpread = 0.2f;
		public ParticleEmitter emitter;
		public float backForce = 10;

		public void Attack()
		{
			if (Time.time < nextAttackTime)
				return;
			nextAttackTime = Time.time + fireRate;
			//void  Fire ( int penetration ,   float damage ,   float force ,   GameObject tracer ,   Vector3 direction ,   Vector3 firePosition  ){
			FireProjectile();
		}

		public void FireProjectile()
		{
			Vector3 direction = SprayDirection();
			Quaternion convert = Quaternion.LookRotation(direction + new Vector3(0, 0.04f, 0));

			Rigidbody instantiatedProjectile;
			instantiatedProjectile = ((GameObject) Instantiate(projectile, pos.position, convert)).GetComponent<Rigidbody>();
			instantiatedProjectile.velocity = instantiatedProjectile.transform.TransformDirection(new Vector3(0, 0, initialSpeed));
			Physics.IgnoreCollision(instantiatedProjectile.GetComponent<Collider>(), transform.root.GetComponent<Collider>());
			emitter.Emit();
			transform.root.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, -backForce), ForceMode.Impulse);
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