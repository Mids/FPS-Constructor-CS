using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	public class GunChildAnimation : MonoBehaviour
	{

		public string fireAnim = "Fire";
		public string emptyFireAnim = "";
		public string reloadAnim = "Reload";
		public string emptyReloadAnim = "Reload";
		public string takeOutAnim = "TakeOut";
		public string putAwayAnim = "PutAway";
		public string enterSecondaryAnim = "EnterSecondary";
		public string exitSecondaryAnim = "ExitSecondary";
		public string reloadIn = "ReloadIn";
		public string reloadOut = "ReloadOut";

		public string walkAnimation = "Walk";
		public string secondaryWalkAnim = "";
		public string secondarySprintAnim = "";
		public float walkSpeedModifier = 20;
		public bool walkWhenAiming = false;
		public string sprintAnimation = "Sprint";
		public string nullAnim = "Null";
		public string secondaryNullAnim = "";
		public string idleAnim = "Idle";
		public string secondaryIdleAnim = "";
		public string chargeAnim = "Charge";
		private float stopAnimTime = 0;
		public bool aim = false;
		private CharacterMotorDB CM;
		private bool idle = false;
		private bool secondary = false;
		private string walkAnim = "";
		private string sprintAnim = "";
		private string nullAnimation = "";
		public bool hasSecondary = false;

		public string secondaryReloadAnim = "";
		public string secondaryReloadEmpty = "";
		public string secondaryFireAnim = "";
		public string secondaryEmptyFireAnim = "";

		//melee
		public int animCount = 2;
		public string[] fireAnims = new string[15];
		public string[] reloadAnims = new string[15];
		public int index = -1;
		public int lastIndex = -1;
		public bool melee = false;
		public bool random = false;
		public float lastSwingTime;
		public float resetTime;
		public GunScript gs;

		private Vector3 dir;
		private float moveWeight = 1;
		private float nullWeight = 1;
		private bool useStrafe = true;

		public void PlayAnim(string name)
		{
			idle = false;
			if (GetComponent<Animation>()[name] == null || !gs.gunActive)
			{
				return;
			}
			GetComponent<Animation>().Stop(name);
			GetComponent<Animation>().Rewind(name);
			GetComponent<Animation>().CrossFade(name, 0.2f);
			stopAnimTime = Time.time + GetComponent<Animation>()[name].length;
		}

		public void PlayAnim(string name, float time)
		{
			idle = false;
			if (GetComponent<Animation>()[name] == null || !gs.gunActive)
			{
				return;
			}
			GetComponent<Animation>().Stop(name);
			GetComponent<Animation>().Rewind(name);
			GetComponent<Animation>()[name].speed = (GetComponent<Animation>()[name].clip.length / time);
			GetComponent<Animation>().CrossFade(name, 0.2f);
			stopAnimTime = Time.time + GetComponent<Animation>()[name].length;
		}

		void Update()
		{
			if (gs != null)
			{
				if (!gs.gunActive)
				{
					return;
				}
			}
			if (GetComponent<Animation>()[nullAnim] == null)
				return;
			if (GetComponent<Animation>()[walkAnimation] == null)
				return;

			CharacterMotorDB CM = PlayerWeapons.CM;

			if (!CM.grounded)
			{
				nullWeight = Mathf.Lerp(nullWeight, 1, Time.deltaTime * 5);
				moveWeight = 0;
			}
			if (Time.time > stopAnimTime + .1)
			{
				moveWeight = Mathf.Lerp(moveWeight, 1, Time.deltaTime * 5);
				nullWeight = Mathf.Lerp(nullWeight, 1, Time.deltaTime * 5);
			}
			else
			{
				moveWeight = 0;
				nullWeight = 0;
			}

			GetComponent<Animation>()[nullAnim].weight = nullWeight;

			Vector3 veloc = PlayerWeapons.CM.movement.velocity;
			Transform trans = PlayerWeapons.player.transform;
			dir = Vector3.Lerp(dir, trans.InverseTransformDirection(veloc), Time.deltaTime * 6);
			Vector3 dirN = dir.normalized;

			float forwardWeight = dirN.z;
			float rightWeight = dirN.x;

			//Weight and speed from direction
			GetComponent<Animation>()[walkAnimation].weight = Mathf.Abs(forwardWeight) * moveWeight;
			GetComponent<Animation>()[walkAnimation].speed = dir.z / CM.movement.maxForwardSpeed;

			float strafeWeight = Mathf.Abs(rightWeight) * moveWeight;
			float strafeSpeed = dir.x / CM.movement.maxSidewaysSpeed * moveWeight;

			//Apply to strafe animation
			/* if(useStrafe){
				 animation[strafeRightAnimation].weight = strafeWeight;
				 animation[strafeRightAnimation].speed = strafeSpeed;
			 } else {*/
			//Handle if we don't have a strafe animation by applying to walk animation
			GetComponent<Animation>()[walkAnimation].weight = Mathf.Max(GetComponent<Animation>()[walkAnimation].weight, strafeWeight);
			if (Mathf.Abs(strafeSpeed) > Mathf.Abs(GetComponent<Animation>()[walkAnimation].speed))
			{
				GetComponent<Animation>()[walkAnimation].speed = strafeSpeed;
			}
			// 	}
		}

		/*void  LateUpdate (){
			if(gs)
				if(!gs.gunActive)
					return;
			if(animation[walkAnim] != null){
				bool  temp = animation[walkAnim].enabled;
			} else {
				temp = false;
			}
	
			if(animation[sprintAnim] != null){
				bool  temp2 = animation[sprintAnim].enabled;
			} else {
				temp2 = false;
			}

			/*if(!animation.IsPlaying(nullAnim))
				animation.CrossFade(nullAnim, .4);
		}*/

		public void ReloadAnim(float reloadTime)
		{
			idle = false;
			if (GetComponent<Animation>()[reloadAnim] == null)
			{
				return;
			}
			//animation.Stop(reloadAnim);
			GetComponent<Animation>().Rewind(reloadAnim);
			GetComponent<Animation>()[reloadAnim].speed = (GetComponent<Animation>()[reloadAnim].clip.length / reloadTime);
			GetComponent<Animation>().Play(reloadAnim);
			stopAnimTime = Time.time + reloadTime;
		}

		public void ReloadEmpty(float reloadTime)
		{
			idle = false;
			if (GetComponent<Animation>()[emptyReloadAnim] == null)
			{
				return;
			}
			GetComponent<Animation>().Rewind(emptyReloadAnim);
			GetComponent<Animation>()[emptyReloadAnim].speed = (GetComponent<Animation>()[emptyReloadAnim].clip.length / reloadTime);
			GetComponent<Animation>().Play(emptyReloadAnim);
			stopAnimTime = Time.time + reloadTime;
		}

		public void FireAnim()
		{
			idle = false;
			if (GetComponent<Animation>()[fireAnim] == null)
			{
				return;
			}
			GetComponent<Animation>().Rewind(fireAnim);
			GetComponent<Animation>().CrossFade(fireAnim, 0.05f);
			stopAnimTime = Time.time + GetComponent<Animation>()[fireAnim].clip.length;
		}

		public void SecondaryReloadEmpty(float reloadTime)
		{
			idle = false;
			if (GetComponent<Animation>()[secondaryReloadEmpty] == null)
			{
				return;
			}
			GetComponent<Animation>()[secondaryReloadEmpty].speed = (GetComponent<Animation>()[secondaryReloadEmpty].clip.length / reloadTime);
			GetComponent<Animation>().Rewind(secondaryReloadEmpty);
			GetComponent<Animation>().CrossFade(secondaryReloadEmpty, 0.2f);
			stopAnimTime = Time.time + reloadTime;
		}

		public void SecondaryReloadAnim(float reloadTime)
		{
			idle = false;
			if (GetComponent<Animation>()[secondaryReloadAnim] == null)
			{
				return;
			}
			GetComponent<Animation>()[secondaryReloadAnim].speed = (GetComponent<Animation>()[secondaryReloadAnim].clip.length / reloadTime);
			GetComponent<Animation>().Rewind(secondaryReloadAnim);
			GetComponent<Animation>().CrossFade(secondaryReloadAnim, 0.2f);
			stopAnimTime = Time.time + reloadTime;
		}

		public void SecondaryFireAnim()
		{
			idle = false;
			if (GetComponent<Animation>()[secondaryFireAnim] == null)
			{
				return;
			}
			GetComponent<Animation>().Rewind(secondaryFireAnim);
			GetComponent<Animation>().CrossFade(secondaryFireAnim, 0.2f);
			stopAnimTime = Time.time + GetComponent<Animation>()[secondaryFireAnim].clip.length;
		}

		public void TakeOutAnim(float takeOutTime)
		{
			idle = false;
			if (takeOutTime <= 0)
				return;
			if (GetComponent<Animation>()[takeOutAnim] == null)
			{
				return;
			}
			GetComponent<Animation>().Stop(putAwayAnim);
			GetComponent<Animation>().Stop(takeOutAnim);
			GetComponent<Animation>().Rewind(takeOutAnim);
			GetComponent<Animation>()[takeOutAnim].speed = (GetComponent<Animation>()[takeOutAnim].clip.length / takeOutTime);
			GetComponent<Animation>().Play(takeOutAnim);
			stopAnimTime = Time.time + takeOutTime;
		}

		public void PutAwayAnim(float putAwayTime)
		{
			idle = false;
			secondary = false;
			nullAnimation = nullAnim;
			if (putAwayTime <= 0)
				return;
			if (GetComponent<Animation>()[putAwayAnim] == null)
			{
				return;
			}
			GetComponent<Animation>().Stop(putAwayAnim);
			GetComponent<Animation>().Rewind(putAwayAnim);
			GetComponent<Animation>()[putAwayAnim].speed = (GetComponent<Animation>()[putAwayAnim].clip.length / putAwayTime);
			GetComponent<Animation>().CrossFade(putAwayAnim, 0.1f);
			stopAnimTime = Time.time + putAwayTime;
		}

		public void SingleFireAnim(float fireRate)
		{
			idle = false;
			if (GetComponent<Animation>()[fireAnim] == null)
			{
				return;
			}
			GetComponent<Animation>().Stop(fireAnim);
			GetComponent<Animation>()[fireAnim].speed = (GetComponent<Animation>()[fireAnim].clip.length / fireRate);
			GetComponent<Animation>().Rewind(fireAnim);
			GetComponent<Animation>().CrossFade(fireAnim, 0.05f);
			stopAnimTime = Time.time + fireRate;
		}

		public void EmptyFireAnim()
		{
			idle = false;
			if (GetComponent<Animation>()[emptyFireAnim] == null)
			{
				return;
			}
			GetComponent<Animation>().Stop(emptyFireAnim);
			GetComponent<Animation>().Rewind(emptyFireAnim);
			GetComponent<Animation>().CrossFade(emptyFireAnim, 0.05f);
			stopAnimTime = Time.time + GetComponent<Animation>()[emptyFireAnim].length;
		}

		public void SecondaryEmptyFireAnim()
		{
			idle = false;
			if (GetComponent<Animation>()[secondaryEmptyFireAnim] == null)
			{
				return;
			}
			GetComponent<Animation>().Stop(secondaryEmptyFireAnim);
			GetComponent<Animation>().Rewind(secondaryEmptyFireAnim);
			GetComponent<Animation>().CrossFade(secondaryEmptyFireAnim, 0.05f);
			stopAnimTime = Time.time + GetComponent<Animation>()[secondaryEmptyFireAnim].length;
		}

		public void EnterSecondary(float t)
		{
			if (GetComponent<Animation>()[secondaryNullAnim] != null)
			{
				nullAnimation = secondaryNullAnim;
			}
			idle = false;
			secondary = true;
			if (GetComponent<Animation>()[enterSecondaryAnim] == null)
			{
				return;
			}
			GetComponent<Animation>().Stop(enterSecondaryAnim);
			GetComponent<Animation>()[enterSecondaryAnim].speed = (GetComponent<Animation>()[enterSecondaryAnim].clip.length / t);
			GetComponent<Animation>().Rewind(enterSecondaryAnim);
			GetComponent<Animation>().CrossFade(enterSecondaryAnim, 0.2f);
			stopAnimTime = Time.time + t;
		}

		public void ExitSecondary(float t)
		{
			nullAnimation = nullAnim;
			idle = false;
			secondary = false;
			if (GetComponent<Animation>()[exitSecondaryAnim] == null)
			{
				return;
			}
			GetComponent<Animation>().Stop(exitSecondaryAnim);
			GetComponent<Animation>()[exitSecondaryAnim].speed = (GetComponent<Animation>()[exitSecondaryAnim].clip.length / t);
			GetComponent<Animation>().Rewind(exitSecondaryAnim);
			GetComponent<Animation>().CrossFade(exitSecondaryAnim, 0.2f);
			stopAnimTime = Time.time + t;
		}

		public void SingleSecFireAnim(float fireRate)
		{
			idle = false;
			if (GetComponent<Animation>()[secondaryFireAnim] == null)
			{
				return;
			}
			GetComponent<Animation>().Stop(secondaryFireAnim);
			GetComponent<Animation>()[secondaryFireAnim].speed = (GetComponent<Animation>()[secondaryFireAnim].clip.length / fireRate);
			GetComponent<Animation>().Rewind(secondaryFireAnim);
			GetComponent<Animation>().CrossFade(secondaryFireAnim, 0.05f);
			stopAnimTime = Time.time + fireRate;
		}

		public void ReloadIn(float reloadTime)
		{
			idle = false;
			if (GetComponent<Animation>()[reloadIn] == null)
			{
				return;
			}
			GetComponent<Animation>()[reloadIn].speed = (GetComponent<Animation>()[reloadIn].clip.length / reloadTime);
			GetComponent<Animation>().Rewind(reloadIn);
			GetComponent<Animation>().Play(reloadIn);
			stopAnimTime = Time.time + reloadTime;
		}

		public void ReloadOut(float reloadTime)
		{
			idle = false;
			if (GetComponent<Animation>()[reloadOut] == null)
			{
				return;
			}
			GetComponent<Animation>()[reloadOut].speed = (GetComponent<Animation>()[reloadOut].clip.length / reloadTime);
			GetComponent<Animation>().Rewind(reloadOut);
			GetComponent<Animation>().Play(reloadOut);
			stopAnimTime = Time.time + reloadTime;
		}

		public void IdleAnim()
		{
			StartCoroutine(IdleAnimRoutine());
		}
		IEnumerator IdleAnimRoutine()
		{
			if (GetComponent<Animation>()[idleAnim] == null || idle || Time.time < stopAnimTime)
			{
				yield break;
			}
			if (!PlayerWeapons.doesIdle)
			{
				idle = true;
				yield break;
			}
			idle = true;
			if (secondary)
			{
				GetComponent<Animation>().Stop(secondaryIdleAnim);
				GetComponent<Animation>().Rewind(secondaryIdleAnim);
				GetComponent<Animation>().CrossFade(secondaryIdleAnim, 0.2f);
				stopAnimTime = Time.time + GetComponent<Animation>()[secondaryIdleAnim].clip.length;
				yield break;
			}
			GetComponent<Animation>().Stop(idleAnim);
			GetComponent<Animation>().Rewind(idleAnim);
			GetComponent<Animation>().CrossFade(idleAnim, 0.2f);
			stopAnimTime = Time.time + GetComponent<Animation>()[idleAnim].clip.length;
			yield return new WaitForSeconds(GetComponent<Animation>()[idleAnim].clip.length);
			idle = false;
		}

		void Start()
		{
			idle = false;
			CM = PlayerWeapons.CM;
			stopAnimTime = 10;
			aim = false;
			nullAnimation = nullAnim;

			/*foreach( AnimationState s   in animation) {
				s.layer = 1;
			}*/

			if (GetComponent<Animation>()[nullAnim] != null)
			{
				GetComponent<Animation>()[nullAnim].layer = -2;
				GetComponent<Animation>()[nullAnim].enabled = true;
			}
			if (GetComponent<Animation>()[walkAnimation] != null)
			{
				GetComponent<Animation>()[walkAnimation].layer = -1;
				GetComponent<Animation>()[walkAnimation].enabled = true;
			}

			/*	if(animation[strafeRightAnimation] != null){
					animation[strafeRightAnimation].layer = -1;
					animation[strafeRightAnimation].enabled = true;
				} else {
					useStrafe = false;
				}*/

			if (GetComponent<Animation>()[sprintAnim] != null)
			{
				GetComponent<Animation>()[sprintAnim].layer = -1;
			}

			GetComponent<Animation>().SyncLayer(-1);

			stopAnimTime = -1;
		}

		public void Aiming()
		{
			idle = false;
			aim = true;
			bool temp = false;
			bool temp2 = false;
			if (GetComponent<Animation>()[walkAnim] != null && !walkWhenAiming)
			{
				GetComponent<Animation>().Stop(walkAnim);
			}
			if (GetComponent<Animation>()[sprintAnim] != null)
			{
				GetComponent<Animation>().Stop(sprintAnim);
			}
			if (GetComponent<Animation>()[nullAnim] != null)
				GetComponent<Animation>().CrossFade(nullAnimation, 0.2f);
		}

		public void StopAiming()
		{
			aim = false;
		}

		public void FireMelee(float fireRate)
		{
			string temp;
			if (random)
			{
				lastIndex = index;
				index = Mathf.RoundToInt(Random.Range(0, animCount - 1));
				if (index == lastIndex)
				{
					if (index == animCount - 1)
					{
						index = Mathf.Clamp(index - 1, 0, animCount - 1);
					}
					else
					{
						index += 1;
					}
				}
			}
			else
			{
				if (Time.time > lastSwingTime + resetTime)
				{
					index = 0;
				}
				else
				{
					index += 1;
				}
				if (index == animCount)
				{
					index = 0;
				}
				lastSwingTime = Time.time;
			}
			temp = fireAnims[index];

			idle = false;
			if (temp == "" || GetComponent<Animation>()[temp] == null)
			{
				return;
			}
			//animation.Stop(temp);
			GetComponent<Animation>()[temp].speed = (GetComponent<Animation>()[temp].clip.length / fireRate);
			//animation.Rewind(temp);
			GetComponent<Animation>().CrossFade(temp, 0.05f);
			stopAnimTime = Time.time + fireRate;
		}

		public void ReloadMelee(float fireRate)
		{
			string temp;
			temp = reloadAnims[index];
			idle = false;
			if (GetComponent<Animation>()[temp] == null)
			{
				return;
			}
			GetComponent<Animation>().Stop(fireAnims[index]);
			GetComponent<Animation>()[temp].speed = (GetComponent<Animation>()[temp].clip.length / fireRate);
			//animation.Rewind(temp);
			GetComponent<Animation>().CrossFadeQueued(temp, 0.05f);
			stopAnimTime = Time.time + fireRate;

		}

	}
}