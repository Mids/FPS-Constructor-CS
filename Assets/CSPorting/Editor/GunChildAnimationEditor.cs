using UnityEngine;
using UnityEditor;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	[CustomEditor(typeof(GunChildAnimation))]
	public class GunChildAnimationEditor : Editor
	{
		public GunChildAnimation Instance { get { return (GunChildAnimation)target; } }

		void OnInspectorGUI()
		{
			EditorGUIUtility.LookLikeInspector();
			EditorGUILayout.BeginVertical();
			EditorGUILayout.Separator();
			Instance.gs = (GunScript)EditorGUILayout.ObjectField(new GUIContent("  Gun Script: ", "This is the GunScript of this weapon"), Instance.gs, typeof(GunScript), true);
			Instance.hasSecondary = EditorGUILayout.Toggle("  Has Secondary: ", Instance.hasSecondary);
			Instance.melee = EditorGUILayout.Toggle("  Is Melee: ", Instance.melee);
			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("Animations", "Animation Name");
			EditorGUILayout.Separator();
			if (!Instance.melee)
			{
				Instance.reloadAnim = EditorGUILayout.TextField("  Reload: ", Instance.reloadAnim);
				Instance.emptyReloadAnim = EditorGUILayout.TextField("  Empty Reload: ", Instance.emptyReloadAnim);
				Instance.fireAnim = EditorGUILayout.TextField("  Fire: ", Instance.fireAnim);
				Instance.emptyFireAnim = EditorGUILayout.TextField("  Dry Fire: ", Instance.emptyFireAnim);
			}
			Instance.takeOutAnim = EditorGUILayout.TextField("  Take Out: ", Instance.takeOutAnim);
			Instance.putAwayAnim = EditorGUILayout.TextField("  Put Away: ", Instance.putAwayAnim);

			if (!Instance.melee)
			{
				Instance.reloadIn = EditorGUILayout.TextField("  Enter Reload: ", Instance.reloadIn);
				Instance.reloadOut = EditorGUILayout.TextField("  Exit Reload: ", Instance.reloadOut);
			}
			Instance.walkAnimation = EditorGUILayout.TextField("  Walk: ", Instance.walkAnimation);

			Instance.sprintAnimation = EditorGUILayout.TextField("  Sprint: ", Instance.sprintAnimation);

			Instance.walkSpeedModifier = EditorGUILayout.FloatField("  Walk Speed Modifier: ", Instance.walkSpeedModifier);
			Instance.walkWhenAiming = EditorGUILayout.Toggle("  Walk When Aiming: ", Instance.walkWhenAiming);
			Instance.nullAnim = EditorGUILayout.TextField("  Null: ", Instance.nullAnim);

			Instance.idleAnim = EditorGUILayout.TextField("  Idle: ", Instance.idleAnim);

			if (Instance.hasSecondary)
			{
				EditorGUILayout.Separator();
				EditorGUILayout.LabelField("Secondary Animations", "Animation Name");
				EditorGUILayout.Separator();
				Instance.secondaryReloadAnim = EditorGUILayout.TextField("  Reload: ", Instance.secondaryReloadAnim);
				Instance.secondaryReloadEmpty = EditorGUILayout.TextField("  Empty Reload: ", Instance.secondaryReloadEmpty);
				Instance.secondaryFireAnim = EditorGUILayout.TextField("  Fire: ", Instance.secondaryFireAnim);
				Instance.secondaryEmptyFireAnim = EditorGUILayout.TextField("  Dry Fire: ", Instance.secondaryEmptyFireAnim);
				Instance.enterSecondaryAnim = EditorGUILayout.TextField("  Enter Secondary: ", Instance.enterSecondaryAnim);
				Instance.exitSecondaryAnim = EditorGUILayout.TextField("  Exit Secondary: ", Instance.exitSecondaryAnim);
				Instance.secondaryWalkAnim = EditorGUILayout.TextField("  Walk: ", Instance.secondaryWalkAnim);
				Instance.secondarySprintAnim = EditorGUILayout.TextField("  Sprint: ", Instance.secondarySprintAnim);
				Instance.secondaryNullAnim = EditorGUILayout.TextField("  Null: ", Instance.secondaryNullAnim);
				Instance.secondaryIdleAnim = EditorGUILayout.TextField("  Idle: ", Instance.secondaryIdleAnim);
			}
			if (Instance.melee)
			{
				EditorGUILayout.Separator();
				Instance.animCount = EditorGUILayout.IntField("  Animations: ", Instance.animCount);
				Instance.random = EditorGUILayout.Toggle("  Random: ", Instance.random);
				if (!Instance.random)
				{
					Instance.resetTime = EditorGUILayout.FloatField("  Chain Reset Time: ", Instance.resetTime);
				}
				for (int i = 0; i < Instance.animCount; i++)
				{
					Instance.fireAnims[i] = EditorGUILayout.TextField("    Attack: ", Instance.fireAnims[i]);
					Instance.reloadAnims[i] = EditorGUILayout.TextField("    Return: ", Instance.reloadAnims[i]);
					EditorGUILayout.Separator();
				}
			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.Separator();
			if (GUI.changed)
				EditorUtility.SetDirty(Instance);
		}

	}
}