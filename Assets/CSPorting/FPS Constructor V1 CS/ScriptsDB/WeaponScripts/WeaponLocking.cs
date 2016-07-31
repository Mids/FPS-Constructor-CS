using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class WeaponLocking : MonoBehaviour
	{
		public bool isLocked = false;

		public void Lock()
		{
			isLocked = true;
		}

		public void Unlock()
		{
			isLocked = false;
		}
	}
}