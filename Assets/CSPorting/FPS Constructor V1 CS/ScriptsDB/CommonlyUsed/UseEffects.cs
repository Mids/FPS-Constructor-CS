using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class UseEffects : MonoBehaviour
	{
		public int setIndex;
		public EffectsManager effectsManagerScript;

		void Awake()
		{
			effectsManagerScript = EffectsManager.manager;
		}
	}
}