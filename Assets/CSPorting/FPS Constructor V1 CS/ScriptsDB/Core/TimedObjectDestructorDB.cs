using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class TimedObjectDestructorDB : MonoBehaviour
	{
		public float timeOut = 1.0f;
		public bool detachChildren = false;

		void Awake()
		{
			Invoke("DestroyNow", timeOut);
		}

		public void DestroyNow()
		{
			if (detachChildren)
			{
				transform.DetachChildren();
			}
			DestroyObject(gameObject);
		}
	}
}