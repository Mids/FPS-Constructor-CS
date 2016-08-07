using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class ChargeTest : MonoBehaviour
	{
		public GunScript gscript;
		public ParticleEmitter[] emitters;
		public bool emitting = false;
		public ParticleEmitter specialEmitter;
		public float minSpecial;

		public void LateUpdate()
		{
			this.GetComponentInChildren<Light>().range = gscript.chargeLevel * 10;
			if (gscript.chargeLevel > 0 || emitting)
			{
				gscript.gameObject.GetComponent<AudioSource>().pitch = gscript.chargeLevel;
				if (!emitting)
				{
					emitCharge(true);
				}
				else if (emitting)
				{
					emitCharge(false);
				}
			}
			else
			{
				gscript.gameObject.GetComponent<AudioSource>().pitch = gscript.firePitch;
				specialEmitter.emit = false;
			}
		}

		public void emitCharge(bool s)
		{
			for (int i = 0; i < emitters.Length; i++)
			{
				emitters[i].emit = s;
			}
			if (gscript.chargeLevel > minSpecial)
			{
				specialEmitter.emit = true;
			}
			else
			{
				specialEmitter.emit = false;
			}

			emitting = s;
		}
	}
}