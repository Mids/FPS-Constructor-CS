using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class WeaponLocking : MonoBehaviour
	{
		bool isLocked = false;

		void Lock()
		{
			isLocked = true;
		}

		void Unlock()
		{
			isLocked = false;
		}
	}
}