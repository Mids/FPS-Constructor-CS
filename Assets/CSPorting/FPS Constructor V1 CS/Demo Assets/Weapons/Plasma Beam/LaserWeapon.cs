using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class LaserWeapon : MonoBehaviour
	{
		public GunScript gscript;
		public ParticleEmitter[] emitters;
		public ParticleRenderer laser;
		public GameObject laserScript;
		private bool emitting = false;
		public float range = 50;
		private GameObject targetObj;
		private Transform lastHit;
		private bool hasTarget = true;
		public float loseAngle = 7;
		private bool hitEnemy = false;
		public float randomAngle = 0.01f;
		public float dps;
		public float overheatTime;
		public float force;
		[HideInInspector] public float curHeat;
		private bool display = false;
		private float timeOnTarget;
		public float powerTime;
		public float damageMultiplier;


		public void Update()
		{
			if (gscript.chargeLevel > 0.05f)
			{
				gscript.idleTime = 0;

				if (curHeat >= overheatTime)
				{
					gscript.chargeLevel = gscript.maxCharge;
					curHeat = 0;
					return;
				}

				gscript.chargeLevel = 1;
				FindTarget();
				curHeat = Mathf.Clamp(curHeat + Time.deltaTime, 0, overheatTime);

				if (!emitting)
				{
					EmitCharge(true);
				}
			}
			else if (gscript.chargeLevel > 0)
			{
				GetComponent<AudioSource>().Play();
				FindTarget();
				EmitHit(false);
				EmitCharge(false);
			}
			else
			{
				timeOnTarget = 0;
				curHeat = Mathf.Clamp(curHeat - (Time.deltaTime * (((overheatTime - curHeat) / overheatTime))) * 2, 0, overheatTime);
				EmitCharge(false);
				EmitHit(false);
				lastHit = null;
			}
		}

		public void EmitCharge(bool s)
		{
			laser.enabled = s;
			laserScript.SendMessage("EmitCharge", s);
			emitting = s;
		}

		public void EmitHit(bool s)
		{
			for (int i = 0; i < emitters.Length; i++)
			{
				emitters[i].emit = s;
			}

			hasTarget = s;
		}

		public void FindTarget()
		{
			if (targetObj == null)
			{
				targetObj = new GameObject();
				laserScript.SendMessage("Target", targetObj.transform);
			}

			int layer1 = 1 << PlayerWeapons.playerLayer;
			int layer2 = 1 << 2;
			int layerMask = layer1 | layer2;
			layerMask = ~layerMask;
			RaycastHit hit;
			float tempAngle = 0;

			if (lastHit != null && hitEnemy)
			{
				Quaternion temp = Quaternion.LookRotation(targetObj.transform.position - transform.position);
				tempAngle = Quaternion.Angle(transform.rotation, temp);
			}
			else
			{
				tempAngle = loseAngle + 1;
			}

			if (lastHit == null) lastHit = this.transform;

			if (Physics.Raycast(PlayerWeapons.weaponCam.transform.position, SprayDirection(randomAngle), out hit, range, layerMask))
			{
				if (tempAngle >= loseAngle || lastHit == hit.transform)
				{
					if (lastHit != hit.transform)
					{
						timeOnTarget = 0;
						if (hit.transform.GetComponent<EnemyDamageReceiver>() != null)
						{
							hitEnemy = true;
						}
						else
						{
							hitEnemy = false;
						}
					}
					else
					{
						timeOnTarget = Mathf.Clamp(timeOnTarget + Time.deltaTime, 0, powerTime);
					}
					lastHit = hit.transform;
					targetObj.transform.position = hit.point;
					targetObj.transform.parent = hit.transform;
					SendDamage(hit);
				}
				else
				{
					timeOnTarget = Mathf.Clamp(timeOnTarget + Time.deltaTime, 0, powerTime);
					SendDamage(hit);
				}

				if (!hasTarget)
				{
					EmitHit(true);
				}
			}
			else if (tempAngle < loseAngle)
			{
				timeOnTarget = Mathf.Clamp(timeOnTarget + Time.deltaTime, 0, powerTime);
				SendDamage(hit);
			}
			else
			{
				lastHit = null;
				if (hasTarget)
				{
					EmitHit(false);
				}
				targetObj.transform.parent = null;
				targetObj.transform.position = transform.position + SprayDirection(randomAngle) * range;
			}
		}

		public void SendDamage(RaycastHit hit)
		{
			Object[] sendArray = new Object[2];
			sendArray[0] = (Object) (object) ((dps + ((timeOnTarget / powerTime) * damageMultiplier)) * Time.deltaTime);
			sendArray[1] = (Object) (object) true;
			if (hit.collider == null)
				return;
			hit.collider.SendMessageUpwards("ApplyDamage", sendArray, SendMessageOptions.DontRequireReceiver);
			if (hit.rigidbody && hit.transform.gameObject.layer != LayerMask.NameToLayer("Player"))
				hit.rigidbody.AddForceAtPosition(force * SprayDirection(randomAngle), hit.point);
		}

		public Vector3 SprayDirection(float c)
		{
			/*
				FIXME_VAR_TYPE vx= (1 - 2 * Random.value) * c;
				FIXME_VAR_TYPE vy= (1 - 2 * Random.value) * c;
				FIXME_VAR_TYPE vz= 1.0f;
				return PlayerWeapons.weaponCam.transform.TransformDirection(Vector3(vx,vy,vz));
				*/
			return PlayerWeapons.weaponCam.transform.TransformDirection(Vector3.forward);
		}

		public void OnGUI()
		{
			if (display)
			{
				GUI.Box(new Rect(Screen.width - 130, Screen.height - 50, 120, 40), "Heat: " + Mathf.Round(curHeat / overheatTime * 100) + "%" + "\n" + "Power: " + Mathf.Round((dps + ((timeOnTarget / powerTime) * damageMultiplier)) / (dps + damageMultiplier) * 100) + "%");
			}
		}

		public void SelectWeapon()
		{
			display = true;
		}

		public void DeselectWeapon()
		{
			display = false;
		}
	}
}