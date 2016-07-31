using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	public class DrawRay : MonoBehaviour
	{
		public bool needsSelection = false;
		public bool negative = false;

		void OnDrawGizmosSelected()
		{
			if (needsSelection)
			{
				Gizmos.color = Color.white;
				Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 10));
				Gizmos.DrawWireSphere(pos, 0.05f);
			}
		}

		void OnDrawGizmos()
		{
			if (!needsSelection)
			{
				Gizmos.color = Color.white;
				Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 10));
				Gizmos.DrawWireSphere(pos, 0.05f);
			}
		}
	}
}