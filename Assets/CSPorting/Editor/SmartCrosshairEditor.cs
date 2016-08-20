using UnityEngine;
using UnityEditor;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	[CustomEditor(typeof(SmartCrosshair))]
	public class SmartCrosshairEditor : Editor
	{
		public SmartCrosshair Instance { get { return (SmartCrosshair)target; } }
		public override void OnInspectorGUI()
		{


			Instance.useTexture = EditorGUILayout.Toggle("  Use Custom:", Instance.useTexture);

			if (Instance.useTexture)
			{
				Instance.crosshairObj = (GameObject)EditorGUILayout.ObjectField(new GUIContent("  Crosshair Object: ", "The object with the crosshair texture"), Instance.crosshairObj, typeof(GameObject), true);
				Instance.crosshairSize = EditorGUILayout.FloatField(new GUIContent("  Crosshair Size: ", "The base scale of the crosshair object"), Instance.crosshairSize);
				Instance.scale = EditorGUILayout.Toggle(new GUIContent("  Scale Crosshair:", "Does the crosshair scale based on accuracy?"), Instance.scale);

				if (Instance.scale)
				{
					Instance.minimumSize = EditorGUILayout.FloatField(new GUIContent("  Min Size: ", "The minimum scale of custom crosshairs"), Instance.minimumSize);
					Instance.maximumSize = EditorGUILayout.FloatField(new GUIContent("  Max Size: ", "The maximum scale of custom crosshairs"), Instance.maximumSize);
				}
			}
			else
			{
				EditorGUILayout.BeginHorizontal();
				Vector2 temp1 = EditorGUILayout.Vector2Field("Crosshair Box Size: ", new Vector2(Instance.length1, Instance.width1));
				EditorGUILayout.EndHorizontal();

				Instance.length1 = temp1.y;
				Instance.width1 = temp1.x;
				Instance.colorFoldout = EditorGUILayout.Foldout(Instance.colorFoldout, "Crosshair Color:");
				if (Instance.colorFoldout)
				{
					Instance.colorDist = EditorGUILayout.FloatField(new GUIContent("  Color Distance: ", "How far can an object be to cause color change? Negative value means infinite"), Instance.colorDist);
					Instance.crosshairTexture = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("        Default: ", "This texture determines the color of the Smart Crosshair"), Instance.crosshairTexture, typeof(Texture), false);
					Instance.friendTexture = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("        Friend: ", "This texture determines the color of the Smart Crosshair when looking at friendly targets"), Instance.friendTexture, typeof(Texture), false);
					Instance.foeTexture = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("        Hostile: ", "This texture determines the color of the Smart Crosshair when looking at hostile targets"), Instance.foeTexture, typeof(Texture), false);
					Instance.otherTexture = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("        Other: ", "This texture determines the color of the Smart Crosshair when looking at 'other' targets"), Instance.otherTexture, typeof(Texture), false);

				}
			}


			Instance.hitEffectFoldout = EditorGUILayout.Foldout(Instance.hitEffectFoldout, "Hit Indicator:");
			if (Instance.hitEffectFoldout)
			{
				EditorGUILayout.BeginHorizontal();
				Instance.hitEffectTexture = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("        Texture: ", "The texture to be displayed over the crosshair when an enemy is shot"), Instance.hitEffectTexture, typeof(Texture), false);
				EditorGUILayout.BeginVertical();
				Vector2 temp = EditorGUILayout.Vector2Field("Size: ", new Vector2(Instance.hitLength, Instance.hitWidth));
				Instance.hitLength = temp.y;
				Instance.hitWidth = temp.x;
				Instance.hitEffectOffset = EditorGUILayout.Vector2Field("Hit Effect Offset: ", Instance.hitEffectOffset);
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Separator();
				Instance.hitSound = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("        Sound: ", "The sound to play with the hit indicator"), Instance.hitSound, typeof(AudioClip), false);
			}
			Instance.debug = EditorGUILayout.Toggle(new GUIContent("  Debug Mode: ", "When Debug Mode is on the crosshair does not disappear when aiming"), Instance.debug);
			EditorGUILayout.Separator();
			if (GUI.changed)
				EditorUtility.SetDirty(Instance);
		}
	}
}