using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class LockOnMissile : MonoBehaviour
	{
		public GunScript gscript;
		[HideInInspector] public Transform targetT;
		[HideInInspector] public Transform targetTwo;
		[HideInInspector] public Transform targetThree;
		[HideInInspector] public int targets = 0;
		public float lockRange;
		[HideInInspector] public bool lockedOn = false;
		public float lockTime;
		public float lockThreshold;
		[HideInInspector] public LineRenderer line;
		public GameObject[] lockObjs;
		public float delayTime;
		public float firedDelayTime;
		private float nextLockTime;
		private Camera cam;

		public void Start()
		{
			line = this.GetComponent<LineRenderer>();
			cam = GameObject.FindWithTag("WeaponCamera").GetComponent<Camera>();
			/*for(int i = 0; i < lockObjs.length; i ++){
				lockObjs[i].transform.parent = null;
			}*/
		}

		public Transform Target()
		{
			nextLockTime = Time.time + (firedDelayTime / gscript.burstCount);
			if (targets == 1)
			{
				targets--;
				lockedOn = false;
				return (targetT);
			}
			else if (targets == 2)
			{
				targets--;
				return (targetTwo);
			}
			else if (targets == 3)
			{
				targets--;
				return (targetThree);
			}
			return null;
		}

		public void Update()
		{
			if (!lockedOn)
			{
				targets = 0;
			}
			if (lockedOn || (targetT != null))
			{
				if ((targets == 0 && Random.value < lockTime) || (targets > 0 && targetT != targetThree && targetTwo != targetT))
				{
					lockObjs[0].transform.position = targetT.position;
					lockObjs[0].SetActive(true);
				}
				else
				{
					lockObjs[0].SetActive(false);
				}
				if ((targets == 1 && Random.value < lockTime) || (targets > 1 && targetTwo != targetThree))
				{
					lockObjs[1].transform.position = targetTwo.position;
					lockObjs[1].SetActive(true);
				}
				else
				{
					lockObjs[1].SetActive(false);
				}
				if ((targets == 2 && Random.value < lockTime) || targets > 2)
				{
					lockObjs[2].transform.position = targetThree.position;
					lockObjs[2].SetActive(true);
				}
				else
				{
					lockObjs[2].SetActive(false);
				}
			}
			else
			{
				lockObjs[0].SetActive(false);
				lockObjs[1].SetActive(false);
				lockObjs[2].SetActive(false);
			}

			if (gscript.chargeLevel > 0)
			{
				if (Random.value < lockTime || Random.value < .2)
				{
					line.enabled = true;
				}
				else
				{
					line.enabled = false;
				}
				LockOn();
			}
			else if (Time.time > nextLockTime)
			{
				lockedOn = false;
				targetT = null;
				lockTime = 0;
				line.enabled = false;
			}
			else
			{
				line.enabled = false;
				return;
			}
		}

		public void LockOn()
		{
			if (targets >= 3)
				return;
			if (Time.time < nextLockTime)
				return;
			if (lockTime > lockThreshold)
			{
				lockedOn = true;
				lockTime = 0;
				targets++;
				nextLockTime = Time.time + delayTime;
				gscript.burstCount = targets;
				//		audio.priority = 255;
				GetComponent<AudioSource>().Play();
				return;
			}
			if (targets == 0)
			{
				gscript.chargeLevel = 0.1f;
			}
			int layer1 = 1 << PlayerWeapons.playerLayer;
			int layer2 = 1 << 2;
			int layerMask = layer1 | layer2;
			layerMask = ~layerMask;
			RaycastHit hit;
			if (Physics.Raycast(transform.position, transform.TransformDirection(0, 0, 1), out hit, lockRange, layerMask))
			{
				if (targets == 0)
				{
					if (targetT != null)
					{
						if (targetT == hit.transform)
						{
							line.enabled = true;
							lockTime += Time.deltaTime;
						}
						else
						{
							lockTime = 0;
							targetT = null;
						}
					}
				}
				else if (targets == 1)
				{
					if (targetTwo != null)
					{
						if (targetTwo == hit.transform)
						{
							line.enabled = true;
							lockTime += Time.deltaTime;
						}
						else
						{
							lockTime = 0;
							targetTwo = null;
						}
					}
				}
				else if (targets == 2)
				{
					if (targetThree != null)
					{
						if (targetThree == hit.transform)
						{
							line.enabled = true;
							lockTime += Time.deltaTime;
						}
						else
						{
							lockTime = 0;
							targetThree = null;
						}
					}
				}
				if (hit.transform.GetComponent<EnemyDamageReceiver>() != null)
				{
					if (targets == 0)
					{
						targetT = hit.transform;
						targetTwo = null;
					}
					else if (targets == 1)
					{
						targetTwo = hit.transform;
						targetThree = null;
					}
					else if (targets == 2)
					{
						targetThree = hit.transform;
					}
				}
			}
			else
			{
				if (targets == 0)
				{
					targetT = null;
				}
				else if (targets == 1)
				{
					targetTwo = null;
				}
				else if (targets == 2)
				{
					targetThree = null;
				}
				lockTime = 0;
			}
		}
	}
}