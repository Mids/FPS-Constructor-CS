using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class EffectsAndPenetrationWizard : MonoBehaviour
	{
		public enum wizardScripts
		{
			UseEffects,
			BulletPenetration
		}

		public wizardScripts selectedScript = wizardScripts.UseEffects;
		public EffectsManager effectsManager;
	}
}