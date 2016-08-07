using UnityEngine;
using UnityEditor;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	[CustomEditor(typeof(GunLook))]
	class GunLookEditor : Editor
	{
		public GunLook Instance { get { return (GunLook)target; } }
		void OnInspectorGUI()
		{
			EditorGUIUtility.LookLikeInspector();
			EditorGUILayout.Separator();

			EditorGUILayout.BeginVertical("toolbar");
			Instance.lookMotionOpen = EditorGUILayout.Foldout(Instance.lookMotionOpen, "Look Motion");
			EditorGUILayout.EndVertical();

			if (Instance.lookMotionOpen)
			{
				EditorGUILayout.BeginVertical("textField");

				Instance.useLookMotion = EditorGUILayout.Toggle("Use Look Motion", Instance.useLookMotion);

				if (Instance.useLookMotion)
				{
					EditorGUILayout.Separator();
					EditorGUILayout.BeginVertical("toolbar");
					EditorGUILayout.LabelField("Standard");
					EditorGUILayout.EndVertical();

					Instance.sensitivityStandardX = EditorGUILayout.FloatField(new GUIContent("X Sensitivity", "Sensitivity for x look movement"), Instance.sensitivityStandardX);
					Instance.sensitivityStandardY = EditorGUILayout.FloatField(new GUIContent("Y Sensitivity", "Sensitivity for y look movement"), Instance.sensitivityStandardY);
					Instance.sensitivityStandardZ = EditorGUILayout.FloatField(new GUIContent("Z Sensitivity", "Sensitivity for z look movement"), Instance.sensitivityStandardZ);
					Instance.retSensitivity = EditorGUILayout.FloatField(new GUIContent("Return Sensitivity", "Speed at which weapon returns to standard position"), Instance.retSensitivity);

					EditorGUILayout.Separator();

					Instance.xRange = EditorGUILayout.FloatField(new GUIContent("X Range", "Range of motion in degrees for x motion"), Instance.xRange);
					Instance.yRange = EditorGUILayout.FloatField(new GUIContent("Y Range", "Range of motion in degrees for y motion"), Instance.yRange);
					Instance.zRange = EditorGUILayout.FloatField(new GUIContent("Z Range", "Range of motion in degrees for z motion"), Instance.zRange);

					EditorGUILayout.Separator();

					EditorGUILayout.BeginVertical("toolbar");
					EditorGUILayout.LabelField("Aim");
					EditorGUILayout.EndVertical();

					Instance.sensitivityAimingX = EditorGUILayout.FloatField(new GUIContent("X Sensitivity", "Sensitivity for x look movement when aiming"), Instance.sensitivityAimingX);
					Instance.sensitivityAimingY = EditorGUILayout.FloatField(new GUIContent("Y Sensitivity", "Sensitivity for y look movement when aiming"), Instance.sensitivityAimingY);
					Instance.sensitivityAimingZ = EditorGUILayout.FloatField(new GUIContent("Z Sensitivity", "Sensitivity for z look movement when aiming"), Instance.sensitivityAimingZ);

					EditorGUILayout.Separator();

					Instance.xRangeAim = EditorGUILayout.FloatField(new GUIContent("X Range", "Range of motion in degrees for x motion when aiming"), Instance.xRangeAim);
					Instance.yRangeAim = EditorGUILayout.FloatField(new GUIContent("Y Range", "Range of motion in degrees for y motion when aiming"), Instance.yRangeAim);
					Instance.zRangeAim = EditorGUILayout.FloatField(new GUIContent("Z Range", "Range of motion in degrees for z motion when aiming"), Instance.zRangeAim);
				}

				EditorGUILayout.EndVertical();
			}

			EditorGUILayout.BeginVertical("toolbar");
			Instance.walkMotionOpen = EditorGUILayout.Foldout(Instance.walkMotionOpen, "Walk Motion");
			EditorGUILayout.EndVertical();

			if (Instance.walkMotionOpen)
			{
				EditorGUILayout.BeginVertical("textField");

				Instance.useWalkMotion = EditorGUILayout.Toggle("Use Walk Motion", Instance.useWalkMotion);

				if (Instance.useWalkMotion)
				{
					/*EditorGUILayout.Separator();
					EditorGUILayout.BeginVertical("toolbar");
						EditorGUILayout.LabelField("Rotation");
					EditorGUILayout.EndVertical();
					
					Instance.zMoveRange = EditorGUILayout.FloatField(new GUIContent("X Range","Range of angle in degrees through which weapon will move for z motion"), Instance.zMoveRange);
					Instance.zMoveSensitivity = EditorGUILayout.FloatField(new GUIContent("X Sensitivity","Determines how much the weapons move based on z movement"), Instance.zMoveSensitivity);
					Instance.zMoveAdjustSpeed = EditorGUILayout.FloatField(new GUIContent("X Speed","Determines how quickly the weapons move"), Instance.zMoveAdjustSpeed);
					EditorGUILayout.Separator();	
					
					Instance.xMoveRange = EditorGUILayout.FloatField(new GUIContent("Z Range","Range of angle in degrees through which weapon will move for x motion"), Instance.xMoveRange);
					Instance.xMoveSensitivity = EditorGUILayout.FloatField(new GUIContent("Z Sensitivity","Determines how much the weapons move based on x movement"), Instance.xMoveSensitivity);
					Instance.xMoveAdjustSpeed = EditorGUILayout.FloatField(new GUIContent("Z Speed","Determines how quickly the weapons move"), Instance.xMoveAdjustSpeed);
					EditorGUILayout.Separator();	
					
					Instance.xAirMoveRange = EditorGUILayout.FloatField(new GUIContent("Z Air Range","Range of angle in degrees through which weapon will move when airborne"), Instance.xAirMoveRange);
					Instance.xAirMoveSensitivity = EditorGUILayout.FloatField(new GUIContent("Z Air Sensitivity","Determines how much the weapons move whe airborne"), Instance.xAirMoveSensitivity);
					Instance.xAirAdjustSpeed = EditorGUILayout.FloatField(new GUIContent("Z Air Speed","Determines how quickly the weapons move"), Instance.xAirAdjustSpeed);
					EditorGUILayout.Separator();
					*/
					EditorGUILayout.BeginVertical("toolbar");
					EditorGUILayout.LabelField("Position");
					EditorGUILayout.EndVertical();

					Instance.zPosMoveRange = EditorGUILayout.FloatField(new GUIContent("Z Range", "Range of distance through which weapon will move for z motion"), Instance.zPosMoveRange);
					Instance.zPosMoveSensitivity = EditorGUILayout.FloatField(new GUIContent("Z Sensitivity", "Determines how much the weapons move based on z movement"), Instance.zPosMoveSensitivity);
					Instance.zPosAdjustSpeed = EditorGUILayout.FloatField(new GUIContent("Z Speed", "Determines how quickly the weapons move"), Instance.zPosAdjustSpeed);
					EditorGUILayout.Separator();

					Instance.xPosMoveRange = EditorGUILayout.FloatField(new GUIContent("X Range", "Range of distance through which weapon will move for x motion"), Instance.xPosMoveRange);
					Instance.xPosMoveSensitivity = EditorGUILayout.FloatField(new GUIContent("X Sensitivity", "Determines how much the weapons move based on z movement"), Instance.xPosMoveSensitivity);
					Instance.xPosAdjustSpeed = EditorGUILayout.FloatField(new GUIContent("X Speed", "Determines how quickly the weapons move"), Instance.xPosAdjustSpeed);
				}

				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.Separator();

		}
	}
}