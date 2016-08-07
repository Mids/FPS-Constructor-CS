using UnityEngine;
using UnityEditor;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	[CustomEditor(typeof(GlobalUpgrade))]

	class GlobalUpgradeEditor : Editor
	{
		public GlobalUpgrade Instance { get { return (GlobalUpgrade)target; } }
		//PlayerWeapons player;
		void OnInspectorGUI()
		{
			EditorGUIUtility.LookLikeInspector();

			EditorGUILayout.Separator();
			Instance.upgrade = (Upgrade)EditorGUILayout.ObjectField(new GUIContent("  Upgrade: ", "Upgrade object to be applied globally"), Instance.upgrade, typeof(Upgrade), true);
			EditorGUILayout.Separator();
			EditorGUILayout.BeginVertical("textField");
			EditorGUILayout.LabelField("Applicable Classes");
			EditorGUILayout.Separator();

			weaponClasses[] ws = (weaponClasses[])System.Enum.GetValues(typeof(weaponClasses));

			if (Instance.classesAllowed == null)
				UpdateArray();

			if (Instance.classesAllowed.Length < ws.Length)
				UpdateArray();

			for (int i = 0; i < ws.Length; i++)
			{
				weaponClasses w = ws[i];
				if (w == weaponClasses.Null) break;
				string className = w.ToString().Replace("_", " ");
				Instance.classesAllowed[i] = EditorGUILayout.Toggle(className, Instance.classesAllowed[i]);
			}
			EditorGUILayout.Separator();

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Enable All", EditorStyles.miniButtonLeft))
			{
				for (int i = 0; i < Instance.classesAllowed.Length; i++)
				{
					Instance.classesAllowed[i] = true;
				}
			}
			if (GUILayout.Button("Disable All", EditorStyles.miniButtonRight))
			{
				for (int i = 0; i < Instance.classesAllowed.Length; i++)
				{
					Instance.classesAllowed[i] = false;
				}
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Separator();
			EditorGUILayout.EndVertical();

		}

		void UpdateArray()
		{
			bool[] tempArray = Instance.classesAllowed;

			Instance.classesAllowed = new bool[weaponClasses.GetValues(typeof(weaponClasses)).Length];
			for (int i = 0; i < tempArray.Length; i++)
			{
				Instance.classesAllowed[i] = tempArray[i];
			}

			if (GUI.changed)
				EditorUtility.SetDirty(Instance);
		}
	}
}