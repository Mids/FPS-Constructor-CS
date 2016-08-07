using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class Projectile : MonoBehaviour
	{
		public Rigidbody val;
		private Rigidbody cache;
		private bool applied = false;


		public void Apply(GunScript gScript)
		{
			cache = gScript.projectile;
			gScript.projectile = val;
		}

		public void Remove(GunScript gScript)
		{
			gScript.projectile = cache;
		}
	}
}