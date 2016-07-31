using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	public class MuzzleFlash : MonoBehaviour
	{
		public bool isPrimary = true;

		public void MuzzleFlash(bool temp)
		{
			BroadcastMessage("Activate", SendMessageOptions.DontRequireReceiver);
			if (temp != isPrimary)
				return;
			var emitters = this.GetComponentsInChildren<ParticleEmitter>();
			for (int i = 0; i < emitters.Length; i++)
			{
				ParticleEmitter p = emitters[i] as ParticleEmitter;
				p.Emit();
			}
		}
	}
}