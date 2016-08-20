using UnityEngine;
using UnityEditor;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	[CustomEditor(typeof(CamSway))]
	public class CamSwayEditor : Editor
	{
		public CamSway Instance { get { return (CamSway)target; } }
		public override void OnInspectorGUI()
		{
			/*	if(Instance.CM == null){
					Instance.CM = Instance.gameObject.GetComponent<"CharacterMotorDB">();
				}*/
			EditorGUILayout.Separator();

			//Move Sway
			EditorGUILayout.BeginVertical("toolbar");
			EditorGUILayout.LabelField("Move Sway");
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical("textField");
			Instance.moveSwayRate = EditorGUILayout.Vector2Field("   Move Sway Rate: ", Instance.moveSwayRate);
			Instance.moveSwayAmplitude = EditorGUILayout.Vector2Field("   Move Sway Amplitude: ", Instance.moveSwayAmplitude);
			EditorGUILayout.EndVertical();

			//Run Sway
			EditorGUILayout.BeginVertical("toolbar");
			EditorGUILayout.LabelField("Run Sway");
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical("textField");
			Instance.runSwayRate = EditorGUILayout.Vector2Field("   Run Sway Rate: ", Instance.runSwayRate);
			Instance.runSwayAmplitude = EditorGUILayout.Vector2Field("   Run Sway Amplitude: ", Instance.runSwayAmplitude);
			EditorGUILayout.EndVertical();

			//Idle Sway
			EditorGUILayout.BeginVertical("toolbar");
			EditorGUILayout.LabelField("Idle Sway");
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical("textField");
			Instance.idleSwayRate = EditorGUILayout.Vector2Field("   Idle Sway Rate: ", Instance.idleSwayRate);
			Instance.idleAmplitude = EditorGUILayout.Vector2Field("   Idle Sway Amplitude: ", Instance.idleAmplitude);
			EditorGUILayout.EndVertical();
			EditorGUILayout.Separator();

			if (GUI.changed)
				EditorUtility.SetDirty(Instance);
		}
	}
}