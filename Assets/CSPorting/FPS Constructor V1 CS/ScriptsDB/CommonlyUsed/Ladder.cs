using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class Ladder : MonoBehaviour
	{
		public float sensitivity = .05f;
		public float moveSpeed = 1;
		public Transform exitPos;
		public Transform enterPos;
		public float minHeight = .1f;
		public float soundInterval;

		private bool ladder = false;
		private bool temp = false;
		private float soundAmt;
		private Transform player;
		private bool belowHeight;

		private float val = 0;

		public IEnumerator MoveTo(Transform what, Vector3 where, float time)
		{
			float i = 0;
			Vector3 pos = what.position;

			while (i <= 1)
			{
				i += Time.deltaTime / time;
				what.position = Vector3.Lerp(pos, where, Mathf.SmoothStep(0, 1, i));
				yield return new WaitForFixedUpdate();
			}
		}

		public IEnumerator RotateTo(Transform what, Vector3 where, float time, bool local)
		{
			float i = 0;
			Vector3 pos;
			if (local)
			{
				pos = what.localEulerAngles;
			}
			else
			{
				pos = what.eulerAngles;
			}

			while (i <= 1)
			{
				i += Time.deltaTime / time;
				if (local)
				{
					what.localEulerAngles = new Vector3(Mathf.LerpAngle(pos.x, where.x, Mathf.SmoothStep(0, 1, i)), Mathf.LerpAngle(pos.y, where.y, Mathf.SmoothStep(0, 1, i)), Mathf.LerpAngle(pos.z, where.z, Mathf.SmoothStep(0, 1, i)));
				}
				else
				{
					what.eulerAngles = new Vector3(Mathf.LerpAngle(pos.x, where.x, Mathf.SmoothStep(0, 1, i)), Mathf.LerpAngle(pos.y, where.y, Mathf.SmoothStep(0, 1, i)), Mathf.LerpAngle(pos.z, where.z, Mathf.SmoothStep(0, 1, i)));
				}
				yield return new WaitForFixedUpdate();
			}
			what.GetComponent<MouseLookDBJS>().UpdateIt();
		}

		public IEnumerator MoveToStart(Transform what, Vector3 where, float time)
		{
			float i = 0;
			Vector3 pos = what.position;

			while (i <= 1)
			{
				i += Time.deltaTime / time;
				Vector3 targetVect = Vector3.Lerp(pos, where, Mathf.SmoothStep(0, 1, i));
				what.position = new Vector3(targetVect.x, what.position.y, targetVect.z);
				yield return new WaitForFixedUpdate();
			}
		}

		void Update()
		{
			if (player == null)
				return;

			if (ladder)
			{
				if (Input.GetButtonDown("Jump"))
				{
					ReleasePlayer();
				}
				float lastVal = val;
				val += InputDB.GetAxis("Vertical") * sensitivity * Time.deltaTime;
				val = Mathf.Clamp01(val);
				soundAmt -= Mathf.Abs(lastVal - val);
				if (soundAmt <= 0)
				{
					soundAmt = soundInterval;
					GetComponent<AudioSource>().Play();
				}
				// player.position.y = Mathf.Lerp(transform.position.y, exitPos.position.y, val);
				player.position = new Vector3(player.position.x, Mathf.Lerp(transform.position.y, exitPos.position.y, val), player.position.z);

				if (val == 1 && !GunScript.takingOut && !GunScript.puttingAway)
					ExitLadder();

				if (val < minHeight && ((lastVal > val) || val == 0) && !GunScript.takingOut && !GunScript.puttingAway)
				{
					ReleasePlayer();
				}
			}
		}

		//Locks player to ladder
		public IEnumerator LockPlayer()
		{
			if (GunScript.takingOut || GunScript.puttingAway || ladder)
				yield break;

			if (PlayerWeapons.PW.weapons[PlayerWeapons.PW.selectedWeapon])
			{
				temp = PlayerWeapons.PW.weapons[PlayerWeapons.PW.selectedWeapon].GetComponent<GunScript>().gunActive;
			}
			else
			{
				temp = true;
			}

			player.SendMessage("StandUp");
			belowHeight = true;
			CharacterMotorDB.paused = true;
			SmartCrosshair.draw = false;
			PlayerWeapons.playerActive = false;
			DBStoreController.canActivate = false;
			PlayerWeapons.HideWeapon();
			val = (player.position.y - transform.position.y) / (exitPos.position.y - transform.position.y);

			RotateTo(player, enterPos.eulerAngles, moveSpeed, false);
			RotateTo(PlayerWeapons.weaponCam.transform, Vector3.zero, moveSpeed, true);
			player.GetComponent<MouseLookDBJS>().individualFreeze = true;
			PlayerWeapons.weaponCam.GetComponent<MouseLookDBJS>().individualFreeze = true;

			yield return MoveToStart(player, enterPos.position, moveSpeed);

			player.GetComponent<MouseLookDBJS>().LockIt(60, 60);
			PlayerWeapons.weaponCam.GetComponent<MouseLookDBJS>().LockItSpecific(-40, 80);
			player.GetComponent<MouseLookDBJS>().individualFreeze = false;
			PlayerWeapons.weaponCam.GetComponent<MouseLookDBJS>().individualFreeze = false;
			PlayerWeapons.canLook = true;

			ladder = true;
			soundAmt = soundInterval;
		}

		//Removes player from ladder to exit position
		public IEnumerator ExitLadder()
		{
			ladder = false;
			yield return MoveTo(player, exitPos.position, moveSpeed);
			ReleasePlayer();
		}

		//Reactivates player to normal function
		public void ReleasePlayer()
		{
			player.GetComponent<MouseLookDBJS>().UnlockIt();
			PlayerWeapons.weaponCam.GetComponent<MouseLookDBJS>().UnlockIt();
			ladder = false;
			SmartCrosshair.draw = true;
			DBStoreController.canActivate = true;
			PlayerWeapons.playerActive = true;
			CharacterMotorDB.paused = false;
			if (temp)
				PlayerWeapons.ShowWeapon();
		}

		void OnTriggerEnter(Collider other)
		{
			if (other.tag == "Player" && !GunScript.takingOut && !GunScript.puttingAway)
			{
				player = other.transform;
				LockPlayer();
			}
		}
	}
}