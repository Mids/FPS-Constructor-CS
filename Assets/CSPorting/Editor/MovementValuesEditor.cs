using UnityEngine;
using UnityEditor;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	[CustomEditor(typeof(MovementValues))]
	public class MovementValuesEditor : Editor
	{
		public MovementValues Instance { get { return (MovementValues)target; } }
	void  OnInspectorGUI (){
		if(Instance.CM == null){
			Instance.CM = Instance.gameObject.GetComponent<CharacterMotorDB>();
		}
	
		Instance.defaultFoldout = EditorGUILayout.Foldout(Instance.defaultFoldout, "Standard Movement");
		if(Instance.defaultFoldout){
			EditorGUILayout.BeginVertical("textField");
			Instance.defaultForwardSpeed = EditorGUILayout.FloatField(new GUIContent("  Forward Speed: ", "Speed in forward direction for normal movement"), Instance.defaultForwardSpeed);
			Instance.defaultSidewaysSpeed = EditorGUILayout.FloatField(new GUIContent("  Sideways Speed: ", "Speed moving left or right for normal movement"), Instance.defaultSidewaysSpeed);
			Instance.defaultBackwardsSpeed = EditorGUILayout.FloatField(new GUIContent("  Backwards Speed: ", "Speed moving left or right for normal movement"), Instance.defaultBackwardsSpeed);
			EditorGUILayout.EndVertical();
			EditorGUILayout.Separator();
		}

		Instance.crouchFoldout = EditorGUILayout.Foldout(Instance.crouchFoldout, "Crouched Movement");
		if(Instance.crouchFoldout){
			EditorGUILayout.BeginVertical("textField");
			Instance.maxCrouchSpeed = EditorGUILayout.FloatField(new GUIContent("  Forward Speed: ", "Speed in forward direction while crouching"), Instance.maxCrouchSpeed);
			Instance.crouchSidewaysSpeed = EditorGUILayout.FloatField(new GUIContent("  Sideways Speed: ", "Speed moving left or right while crouching"), Instance.crouchSidewaysSpeed);
			Instance.crouchBackwardsSpeed = EditorGUILayout.FloatField(new GUIContent("  Backwards Speed: ", "Speed moving backwards while crouching"), Instance.crouchBackwardsSpeed);
			EditorGUILayout.EndVertical();
			EditorGUILayout.Separator();
		}
		
		Instance.proneFoldout = EditorGUILayout.Foldout(Instance.proneFoldout, "Prone Movement");
		if(Instance.proneFoldout){
			EditorGUILayout.BeginVertical("textField");
			Instance.maxProneSpeed = EditorGUILayout.FloatField(new GUIContent("  Forward Speed: ", "Speed in forward direction while prone"), Instance.maxProneSpeed);
			Instance.proneSidewaysSpeed = EditorGUILayout.FloatField(new GUIContent("  Sideways Speed: ", "Speed moving left or right while prone"), Instance.proneSidewaysSpeed);
			Instance.proneBackwardsSpeed = EditorGUILayout.FloatField(new GUIContent("  Backwards Speed: ", "Speed moving backwards while prone"), Instance.proneBackwardsSpeed);
			EditorGUILayout.EndVertical();
			EditorGUILayout.Separator();
		}
		
		Instance.aimFoldout = EditorGUILayout.Foldout(Instance.aimFoldout, "Aiming Movement");
		if(Instance.aimFoldout){
			EditorGUILayout.BeginVertical("textField");
			Instance.maxAimSpeed = EditorGUILayout.FloatField(new GUIContent("  Forward Speed: ", "Speed in forward direction while aiming"), Instance.maxAimSpeed);
			Instance.aimSidewaysSpeed = EditorGUILayout.FloatField(new GUIContent("  Sideways Speed: ", "Speed moving left or right while aiming"), Instance.aimSidewaysSpeed);
			Instance.aimBackwardsSpeed = EditorGUILayout.FloatField(new GUIContent("  Backwards Speed: ", "Speed moving backwards while aiming"), Instance.aimBackwardsSpeed);
			EditorGUILayout.EndVertical();
			EditorGUILayout.Separator();
		}
		
		Instance.sprintFoldout = EditorGUILayout.Foldout(Instance.sprintFoldout, "Sprint Movement");
		if(Instance.sprintFoldout){
			EditorGUILayout.BeginVertical("textField");
			Instance.maxSprintSpeed = EditorGUILayout.FloatField(new GUIContent("  Forward Speed: ", "Speed in forward direction while sprinting"), Instance.maxSprintSpeed);
			Instance.minSprintSpeed = EditorGUILayout.FloatField(new GUIContent("  Minimum Speed: ", "Minimum speed to remain sprinting"), Instance.minSprintSpeed);
			Instance.sprintSidewaysSpeed = EditorGUILayout.FloatField(new GUIContent("  Sideways Speed: ", "Speed moving left or right while sprinting"), Instance.sprintSidewaysSpeed);
			EditorGUILayout.Separator();
			Instance.sprintDuration = EditorGUILayout.IntField(new GUIContent("  Sprint Duration: ", "How long can the player sprint (in seconds)?"), Instance.sprintDuration);
			Instance.sprintAddStand = EditorGUILayout.FloatField(new GUIContent("  Standing Sprint Return: ", "How quickly does sprint return when standing?"), Instance.sprintAddStand);
			Instance.sprintAddWalk = EditorGUILayout.FloatField(new GUIContent("  Walking Sprint Return: ", "How quickly does sprint return when walking?"), Instance.sprintAddWalk);
			Instance.sprintMin = EditorGUILayout.FloatField(new GUIContent("  Minimum Sprint: ", "Minimum sprint value to sprint"), Instance.sprintMin);
			Instance.recoverDelay = EditorGUILayout.FloatField(new GUIContent("  Recover Delay: ", "Time in seconds after sprinting until sprint begins returning"), Instance.recoverDelay);
			Instance.exhaustedDelay = EditorGUILayout.FloatField(new GUIContent("  Recover Delay: ", "Time in seconds after sprinting to exhaustion until sprint begins returning"), Instance.exhaustedDelay);
			EditorGUILayout.EndVertical();
			EditorGUILayout.Separator();
		}
		
		Instance.jumpFoldout = EditorGUILayout.Foldout(Instance.jumpFoldout, "Jumping Movement");
		if(Instance.jumpFoldout){
			EditorGUILayout.BeginVertical("textField");
			Instance.CM.movement.gravity = EditorGUILayout.FloatField(new GUIContent("  Gravity: ", "Gravity Factor"), Instance.CM.movement.gravity);
			Instance.CM.movement.maxFallSpeed = EditorGUILayout.FloatField(new GUIContent("  Max Fall Speed: ", "Maximum fall speed of player"), Instance.CM.movement.maxFallSpeed);
			Instance.CM.movement.fallDamageStart = EditorGUILayout.FloatField(new GUIContent("  Fall Damage Start: ", "Fall speed at which damage begins to be applied"), Instance.CM.movement.fallDamageStart);
			Instance.CM.movement.fallDamageEnd = EditorGUILayout.FloatField(new GUIContent("  Fall Damage End: ", "Fall speed at which maximum damage is applied"), Instance.CM.movement.fallDamageEnd);
			Instance.CM.movement.fallDamageMax = EditorGUILayout.FloatField(new GUIContent("  Max Fall Damage: ", "Fall Damage applied at end speed. Fall damage scales linearly toward this from start"), Instance.CM.movement.fallDamageMax);
			EditorGUILayout.EndVertical();
			EditorGUILayout.Separator();
		}
		if (GUI.changed)
			EditorUtility.SetDirty(Instance);
	}
}
}