using UnityEngine;
using UnityEditor;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	[CustomEditor(typeof(AimMode))]
	public class AimModeEditor : Editor
	{
		public AimMode Instance { get { return (AimMode)target; } }
		bool foldout;
		bool foldout1;

		public override void OnInspectorGUI()
		{
			EditorGUIUtility.LookLikeInspector();
			EditorGUILayout.BeginVertical();

			Instance.aim = EditorGUILayout.Toggle("  Weapon Aims: ", Instance.aim);

			if (Instance.aim)
			{
				if (!Instance.sightsZoom1)
					Instance.scoped1 = EditorGUILayout.Toggle("  Has Scope: ", Instance.scoped1);
				if (!Instance.scoped1)
				{
					Instance.sightsZoom1 = EditorGUILayout.Toggle("  Zoom Down Sights: ", Instance.sightsZoom1);
					Instance.crosshairWhenAiming = EditorGUILayout.Toggle("  Show Crosshair: ", Instance.crosshairWhenAiming);
				}
				else
				{
					Instance.scopeTexture = (Texture)EditorGUILayout.ObjectField("  Scope Texture: ", Instance.scopeTexture, typeof(Texture), false);
					Instance.st169 = (Texture)EditorGUILayout.ObjectField("  Scope Texture 16:9: ", Instance.st169, typeof(Texture), false);
					Instance.st1610 = (Texture)EditorGUILayout.ObjectField("  Scope Texture 16:10: ", Instance.st1610, typeof(Texture), false);
					Instance.st54 = (Texture)EditorGUILayout.ObjectField("  Scope Texture 5:4: ", Instance.st54, typeof(Texture), false);
					Instance.st43 = (Texture)EditorGUILayout.ObjectField("  Scope Texture 4:3: ", Instance.st43, typeof(Texture), false);

				}
				if (Instance.scoped1 || Instance.sightsZoom1)
					Instance.zoomFactor1 = EditorGUILayout.FloatField("  Zoom Factor: ", Instance.zoomFactor1);
			}
			Instance.aimRate = EditorGUILayout.FloatField("  Aim Rate: ", Instance.aimRate);
			Instance.sprintRate = EditorGUILayout.FloatField("  Sprint Rate: ", Instance.sprintRate);
			Instance.retRate = EditorGUILayout.FloatField("  Return Rate: ", Instance.retRate);

			EditorGUILayout.Separator();

			Instance.overrideSprint = EditorGUILayout.Toggle("  Override Sprint: ", Instance.overrideSprint);
			if (Instance.overrideSprint)
			{
				Instance.sprintDuration = EditorGUILayout.IntField("  Sprint Duration: ", Instance.sprintDuration);
				Instance.sprintAddStand = EditorGUILayout.FloatField("  Standing Sprint Return Rate: ", Instance.sprintAddStand);
				Instance.sprintAddWalk = EditorGUILayout.FloatField("  Walking Sprint Return Rate: ", Instance.sprintAddWalk);
				Instance.sprintMin = EditorGUILayout.FloatField("  Sprint Minimum: ", Instance.sprintMin);
				Instance.recoverDelay = EditorGUILayout.FloatField("  Sprint Recovery Delay: ", Instance.recoverDelay);
				Instance.exhaustedDelay = EditorGUILayout.FloatField("  Exhausted Recovery Delay: ", Instance.exhaustedDelay);
			}

			EditorGUILayout.Separator();

			Instance.hasSecondary = EditorGUILayout.Toggle("  Has Secondary: ", Instance.hasSecondary);
			Instance.secondaryAim = EditorGUILayout.Toggle("  Weapon Aims: ", Instance.secondaryAim);

			if (Instance.secondaryAim && Instance.hasSecondary)
			{
				if (!Instance.sightsZoom2)
					Instance.scoped2 = EditorGUILayout.Toggle("  Has Scope: ", Instance.scoped2);
				if (!Instance.scoped2)
					Instance.sightsZoom2 = EditorGUILayout.Toggle("  Zoom Down Sights: ", Instance.sightsZoom2);
				if (Instance.scoped2 || Instance.sightsZoom2)
					Instance.zoomFactor2 = EditorGUILayout.FloatField("  Zoom Factor: ", Instance.zoomFactor2);
			}


			EditorGUILayout.EndVertical();

			EditorGUILayout.Separator();
			foldout = EditorGUILayout.Foldout(foldout, "Configure Primary Weapon Positions");
			if (foldout)
			{
				EditorGUILayout.BeginVertical();
				EditorGUILayout.BeginHorizontal();

				if (GUILayout.Button(new GUIContent("Move to Hip Position", "Move Weapon to Hip Position"), "miniButton"))
				{
					Instance.transform.localPosition = Instance.hipPosition1;
					Instance.transform.localEulerAngles = Instance.hipRotation1;
				}
				if (GUILayout.Button(new GUIContent("Configure Hip Position", "Set Hip Position to Current Position"), "miniButton"))
				{
					Instance.hipPosition1 = Instance.transform.localPosition;
					Instance.hipRotation1 = Instance.transform.localEulerAngles;
					EditorUtility.SetDirty(Instance);
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginVertical("textField");
				Instance.hipPosition1 = EditorGUILayout.Vector3Field("hipPosition", Instance.hipPosition1);
				Instance.hipRotation1 = EditorGUILayout.Vector3Field("hipRotation", Instance.hipRotation1);
				EditorGUILayout.EndVertical();



				EditorGUILayout.Separator();


				if (Instance.aim)
				{
					EditorGUILayout.BeginHorizontal();
					if (GUILayout.Button(new GUIContent("Move to Aim Position", "Move Weapon to Aim Position"), "miniButton"))
					{
						Instance.transform.localPosition = Instance.aimPosition1;
						Instance.transform.localEulerAngles = Instance.aimRotation1;
					}
					if (GUILayout.Button(new GUIContent("Configure Aim Position", "Set Aim Position to Current Position"), "miniButton"))
					{
						Instance.aimPosition1 = Instance.transform.localPosition;
						Instance.aimRotation1 = Instance.transform.localEulerAngles;
						EditorUtility.SetDirty(Instance);
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginVertical("textField");
					Instance.aimPosition1 = EditorGUILayout.Vector3Field("aimPosition", Instance.aimPosition1);
					Instance.aimRotation1 = EditorGUILayout.Vector3Field("aimRotation", Instance.aimRotation1);
					EditorGUILayout.EndVertical();
				}



				EditorGUILayout.Separator();



				EditorGUILayout.BeginHorizontal();

				if (GUILayout.Button(new GUIContent("Move to Sprint Position", "Move Weapon to Sprint Position"), "miniButton"))
				{
					Instance.transform.localPosition = Instance.sprintPosition1;
					Instance.transform.localEulerAngles = Instance.sprintRotation1;
				}
				if (GUILayout.Button(new GUIContent("Configure Sprint Position", "Set Sprint Position to Position"), "miniButton"))
				{
					Instance.sprintPosition1 = Instance.transform.localPosition;
					Instance.sprintRotation1 = Instance.transform.localEulerAngles;
					EditorUtility.SetDirty(Instance);
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginVertical("textField");
				Instance.sprintPosition1 = EditorGUILayout.Vector3Field("sprintPosition", Instance.sprintPosition1);
				Instance.sprintRotation1 = EditorGUILayout.Vector3Field("sprintRotation", Instance.sprintRotation1);
				EditorGUILayout.EndVertical();

				EditorGUILayout.EndVertical();
			}

			///****************************
			///****************************

			if (Instance.hasSecondary)
			{
				foldout1 = EditorGUILayout.Foldout(foldout1, "Configure Secondary Weapon Positions");
				if (foldout1)
				{
					EditorGUILayout.BeginVertical();
					EditorGUILayout.BeginHorizontal();

					if (GUILayout.Button(new GUIContent("Move to Hip Position", "Move Weapon to Hip Position"), "miniButton"))
					{
						Instance.transform.localPosition = Instance.hipPosition2;
						Instance.transform.localEulerAngles = Instance.hipRotation2;
					}
					if (GUILayout.Button(new GUIContent("Configure Hip Position", "Set Hip Position to Current Position"), "miniButton"))
					{
						Instance.hipPosition2 = Instance.transform.localPosition;
						Instance.hipRotation2 = Instance.transform.localEulerAngles;
						EditorUtility.SetDirty(Instance);
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginVertical("textField");
					Instance.hipPosition2 = EditorGUILayout.Vector3Field("hipPosition", Instance.hipPosition2);
					Instance.hipRotation2 = EditorGUILayout.Vector3Field("hipRotation", Instance.hipRotation2);
					EditorGUILayout.EndVertical();



					EditorGUILayout.Separator();


					if (Instance.secondaryAim)
					{
						EditorGUILayout.BeginHorizontal();

						if (GUILayout.Button(new GUIContent("Move to Aim Position", "Move Weapon to Aim Position"), "miniButton"))
						{
							Instance.transform.localPosition = Instance.aimPosition2;
							Instance.transform.localEulerAngles = Instance.aimRotation2;
						}
						if (GUILayout.Button(new GUIContent("Configure Aim Position", "Set Aim Position to Current Position"), "miniButton"))
						{
							Instance.aimPosition2 = Instance.transform.localPosition;
							Instance.aimRotation2 = Instance.transform.localEulerAngles;
							EditorUtility.SetDirty(Instance);
						}
						EditorGUILayout.EndHorizontal();

						EditorGUILayout.BeginVertical("textField");
						Instance.aimPosition2 = EditorGUILayout.Vector3Field("aimPosition", Instance.aimPosition2);
						Instance.aimRotation2 = EditorGUILayout.Vector3Field("aimRotation", Instance.aimRotation2);
						EditorGUILayout.EndVertical();
					}

					EditorGUILayout.Separator();

					EditorGUILayout.BeginHorizontal();

					if (GUILayout.Button(new GUIContent("Move to Sprint Position", "Move Weapon to Sprint Position"), "miniButton"))
					{
						Instance.transform.localPosition = Instance.sprintPosition2;
						Instance.transform.localEulerAngles = Instance.sprintRotation2;
					}
					if (GUILayout.Button(new GUIContent("Configure Sprint Position", "Set Sprint Position to Position"), "miniButton"))
					{
						Instance.sprintPosition2 = Instance.transform.localPosition;
						Instance.sprintRotation2 = Instance.transform.localEulerAngles;
						EditorUtility.SetDirty(Instance);
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginVertical("textField");
					Instance.sprintPosition2 = EditorGUILayout.Vector3Field("sprintPosition", Instance.sprintPosition2);
					Instance.sprintRotation2 = EditorGUILayout.Vector3Field("sprintRotation", Instance.sprintRotation2);
					EditorGUILayout.EndVertical();

					EditorGUILayout.EndVertical();
				}
			}
			if (GUI.changed)
				EditorUtility.SetDirty(Instance);
		}
	}
}