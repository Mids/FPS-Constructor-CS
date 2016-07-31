using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	public class LockCursor : MonoBehaviour
	{
		public static bool canLock = true;
		public static bool mobile;
		public static bool unPaused = false;

		void Awake()
		{
#if UNITY_IPHONE
	mobile = true;
#elif UNITY_ANDROID
	mobile = true;
#else
			mobile = false;
#endif
		}
		void Start()
		{
			if (!mobile)
			{
				SetPause(true);
				canLock = true;
				PlayerWeapons.playerActive = false;
			}
			else
			{
				SetPause(false);
				canLock = false;
				PlayerWeapons.playerActive = true;
			}

		}

		public void OnApplicationQuit()
		{
			Time.timeScale = 1;
		}

		public static void SetPause(bool pause)
		{
			GameObject player = GameObject.FindWithTag("Player");
			if (mobile)
			{
				return;
			}

			InputDB.ResetInputAxes();

			if (pause)
			{
				PlayerWeapons.playerActive = false;
				//Screen.lockCursor = false;
				Time.timeScale = 0;
				player.BroadcastMessage("Freeze", SendMessageOptions.DontRequireReceiver);

			}
			else
			{
				unPaused = true;
				Time.timeScale = 1;
				Screen.lockCursor = true;
				PlayerWeapons.playerActive = true;
				player.BroadcastMessage("UnFreeze", SendMessageOptions.DontRequireReceiver);
			}
		}

		public static void HardUnlock()
		{
			canLock = false;
			Screen.lockCursor = false;
		}

		public static void HardLock()
		{
			canLock = false;
			Screen.lockCursor = true;
		}

		private bool wasLocked = false;

		void Update()
		{
			if (!canLock)
				return;

			if (Input.GetMouseButton(0) && Screen.lockCursor == false)
			{
				SetPause(false);
			}

			if (InputDB.GetButton("Escape"))
			{
				SetPause(true);
			}

			// Did we lose cursor locking?
			// eg. because the user pressed escape
			// or because he switched to another application
			// or because some script set Screen.lockCursor = false;
			if (!Screen.lockCursor && wasLocked)
			{
				wasLocked = false;
				SetPause(true);
			}
			// Did we gain cursor locking?
			else if (Screen.lockCursor && !wasLocked)
			{
				wasLocked = true;
				SetPause(false);
			}
		}

		void LateUpdate()
		{
			unPaused = false;
		}
	}
}