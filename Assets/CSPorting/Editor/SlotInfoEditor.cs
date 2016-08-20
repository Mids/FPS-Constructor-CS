using UnityEngine;
using UnityEditor;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	[CustomEditor(typeof(SlotInfo))]
	public class SlotInfoEditor : Editor
	{
		public SlotInfo Instance { get { return (SlotInfo)target; } }
		PlayerWeapons player;
		bool[] foldoutState;
		int[] tmpAllowed;

		void Awake()
		{
			player = FindObjectOfType(typeof(PlayerWeapons)) as PlayerWeapons;
			foldoutState = new bool[player.weapons.Length];

		}
		void OnEnable()
		{
		}
		public override void OnInspectorGUI()
		{

			//If our allowed array is the wrong Length, we must correct it
			if (player.weapons.Length != Instance.allowed.Length)
			{
				//Create an array of the proper Length
				tmpAllowed = new int[player.weapons.Length];
				//Now iterate through and copy values
				int upperBound = Mathf.Min(Instance.allowed.Length, player.weapons.Length);
				for (int j = 0; j < upperBound; j++)
				{
					tmpAllowed[j] = Instance.allowed[j];
				}
				Instance.allowed = tmpAllowed;
			}

			//If our slotName array is the wrong Length, we must correct it
			if (player.weapons.Length != Instance.slotName.Length)
			{
				//Create an array of the proper Length
				string[] tmpAllowedS = new string[player.weapons.Length];
				//Now iterate through and copy values
				int upperBound = Mathf.Min(Instance.slotName.Length, player.weapons.Length);
				for (int j = 0; j < upperBound; j++)
				{
					tmpAllowedS[j] = Instance.slotName[j];
				}
				Instance.slotName = tmpAllowedS;
			}

			player = FindObjectOfType(typeof(PlayerWeapons)) as PlayerWeapons;
			EditorGUIUtility.LookLikeInspector();
			for (int i = 0; i < player.weapons.Length; i++)
			{
				if (string.IsNullOrEmpty(Instance.slotName[i]))
				{
					Instance.slotName[i] = "Slot " + (i + 1);
				}
				Instance.slotName[i] = EditorGUILayout.TextField("Slot Name:", Instance.slotName[i]);
				foldoutState[i] = EditorGUILayout.Foldout(foldoutState[i], "Allowed Weapon Classes");
				if (foldoutState[i])
				{
					foreach (WeaponInfo.weaponClasses w in WeaponInfo.weaponClasses.GetValues(typeof(WeaponInfo.weaponClasses)))
					{
						if (w == WeaponInfo.weaponClasses.Null) break;
						string className = w.ToString().Replace("_", " ");
						bool allowed = Instance.isWCAllowed(i, w);
						bool toggleState;
						toggleState = GUILayout.Toggle(allowed, className);
						if (toggleState != allowed)
						{
							Instance.setAllowed(i, w, toggleState);
							toggleState = allowed;
						}
					}
				}
			}
			if (GUI.changed)
				EditorUtility.SetDirty(Instance);
		}
	}

}