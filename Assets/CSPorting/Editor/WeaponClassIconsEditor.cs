using UnityEngine;
using UnityEditor;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	[CustomEditor(typeof(WeaponClassIcons))]
	public class WeaponClassIconsEditor : Editor
	{
		public WeaponClassIcons Instance { get { return (WeaponClassIcons)target; } }

		public override void OnInspectorGUI()
		{
			EditorGUIUtility.LookLikeInspector();
			EditorGUILayout.BeginVertical();
			foreach (WeaponInfo.weaponClasses w in WeaponInfo.weaponClasses.GetValues(typeof(WeaponInfo.weaponClasses)))
			{
				if (w == WeaponInfo.weaponClasses.Null) break; // hide the Null Weapon				
				Instance.weaponClassTextures[(int)w] = (Texture)EditorGUILayout.ObjectField(w.ToString().Replace("_", " "), Instance.weaponClassTextures[(int)w], typeof(Texture), false);
			}
			EditorGUILayout.EndVertical();
		}
	}
}