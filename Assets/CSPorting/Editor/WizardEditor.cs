using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	[CustomEditor(typeof(EffectsAndPenetrationWizard))]
	public class WizardEditor : Editor
	{
		public EffectsAndPenetrationWizard Instance { get { return (EffectsAndPenetrationWizard)target; } }

		Collider[] cArray;
		List<Collider>[] NameArrays;
		List<string> Names = new List<string>();
		bool[] boolArray;
		int[] setArray;
		int[] setArrayDummy;
		string[] setNameArrayDummy;
		int[] penetrationArray;
		System.Type script;
		EffectsAndPenetrationWizard.wizardScripts sScript = EffectsAndPenetrationWizard.wizardScripts.UseEffects;
		System.Type prevScript; //Used to see if the user changed which script they were viewing
		GUIStyle penStyle;
		string effectTypeLabel = "";
		string effectTypeName = "";

		void OnEnable()
		{
			Instance.effectsManager = GameObject.FindWithTag("Manager").GetComponent<EffectsManager>();
			if (Instance.effectsManager == null)
			{
				Debug.Log("Effects Manager Script must be attached to the Manager Object");
			}
			setArrayDummy = new int[1];
			setNameArrayDummy = new string[1];
			setNameArrayDummy[0] = "None";
			penStyle = new GUIStyle();
			penStyle.alignment = (TextAnchor)1;
			//	penStyle.normal.textColor = Color(.7, .7, .7, 1);
		}


		void SortAndScanColliders()
		{
			cArray = null;
			Names.Clear();
			string n;
			cArray = (Collider[])FindObjectsOfType(typeof(Collider));
			for (int i = 0; i < cArray.Length; i++)
			{
				n = cArray[i].name;
				if (!Names.Contains(n))
				{
					Names.Add(n);
				}
			}
			Names.Sort();
			NameArrays = new List<Collider>[Names.Count];
			boolArray = new bool[Names.Count];
			setArray = new int[Names.Count];
			penetrationArray = new int[Names.Count];
			for (int i = 0; i < cArray.Length; i++)
			{
				n = cArray[i].name;
				for (int j = 0; j < Names.Count; j++)
				{
					if (Names[j] == n)
					{
						if (NameArrays[j] == null)
						{
							NameArrays[j] = new List<Collider>();
						}
						NameArrays[j].Add(cArray[i]);
						if (cArray[i].GetComponent(script.Name) != null)
						{
							boolArray[j] = true;
						}
					}
				}
			}
		}
		public override void OnInspectorGUI()
		{
			//EditorGUIUtility.LookLikeInspector();
			Instance.selectedScript = (EffectsAndPenetrationWizard.wizardScripts)EditorGUILayout.EnumPopup("   Effect: ", Instance.selectedScript);
			sScript = Instance.selectedScript;

			if (sScript == EffectsAndPenetrationWizard.wizardScripts.UseEffects)
			{
				if (Instance.effectsManager == null)
				{
					EditorGUILayout.LabelField("   Effects Manager Script must be attached to Manager Object", "");
					if (GUILayout.Button("Add Script Now"))
					{
						Instance.effectsManager = GameObject.FindWithTag("Manager").AddComponent<EffectsManager>();
					}
					return;
				}
				effectTypeLabel = "Enable?";
				effectTypeName = "Effect Set";
				script = typeof(UseEffects);
			}
			else if (sScript == EffectsAndPenetrationWizard.wizardScripts.BulletPenetration)
			{
				script = typeof(BulletPenetration);
				effectTypeLabel = "Enable?";
				effectTypeName = "Resistance";
			}

			if (prevScript != script)
			{
				SortAndScanColliders();


			}

			if (Names.Count > 0)
			{
				EditorGUILayout.Separator();
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("   Collider", "");
				EditorGUILayout.LabelField("", effectTypeLabel);
				EditorGUILayout.LabelField("", effectTypeName);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Separator();
			}
			bool changed = false;

			EditorGUILayout.BeginVertical("textField");

			for (int k = 0; k < Names.Count; k++)
			{
				if (NameArrays[k][0] == null)
				{
					Debug.Log("null, k = " + k);
					SortAndScanColliders();
					break;
				}
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("     " + Names[k], "");
				bool prevBool = boolArray[k];
				boolArray[k] = EditorGUILayout.Toggle("", boolArray[k]);
				if (boolArray[k] != prevBool) changed = true;

				//Use Effects options
				if (script is UseEffects)
				{
					if (boolArray[k])
					{
						if (!NameArrays[k][0].GetComponent<UseEffects>())
						{
							// Selected but script not found on object
							changed = true;
							//						ApplyChanges();
						}
						else
						{
							if (setArray[k] == 0)
							{
								if (NameArrays[k][0].GetComponent<UseEffects>())
								{
									setArray[k] = NameArrays[k][0].GetComponent<UseEffects>().setIndex;
								}
								else
								{
									setArray[k] = 0;
								}
							}
							int prev = setArray[k];
							setArray[k] = EditorGUILayout.Popup("", setArray[k], Instance.effectsManager.setNameArray);
							if (setArray[k] != prev) changed = true;
						}
					}
					else
					{
						// Effect not selected
						if (NameArrays[k][0].GetComponent<UseEffects>())
						{
							changed = true;
						}
						EditorGUILayout.Popup("", setArrayDummy[0], setNameArrayDummy);
					}
				}
				else if (script is BulletPenetration)
				{
					if (penetrationArray[k] == 0)
					{
						if (NameArrays[k][0].GetComponent<BulletPenetration>())
						{
							penetrationArray[k] = NameArrays[k][0].GetComponent<BulletPenetration>().penetrateValue;
						}
						else
						{
							penetrationArray[k] = 0;
						}
					}
					int prev = penetrationArray[k];
					if (boolArray[k])
					{
						penetrationArray[k] = EditorGUILayout.IntField("", penetrationArray[k], penStyle);
						if (penetrationArray[k] != prev) changed = true;
					}
					else
					{
						int tmp = EditorGUILayout.IntField("", 0, penStyle);
					}
				}
				EditorGUILayout.EndHorizontal();
				if (GUI.changed)
					EditorUtility.SetDirty(Instance);
			}

			if (changed)
			{
				ApplyChanges();
				changed = false;
			}

			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical();

			//		if (GUILayout.Button(new GUIContent("Apply Changes", "Adds the selected script script to all Coliders selected, removes it from those that are unselected. Changes decal sets if applicable"), "miniButton")){
			//			ApplyChanges();
			//		}
			EditorGUILayout.EndVertical();
			prevScript = script;
		}

		void ApplyChanges()
		{
			for (int i = 0; i < Names.Count; i++)
			{
				if (boolArray[i])
				{
					for (int j = 0; j < NameArrays[i].Count; j++)
					{
						if (NameArrays[i][j].GetComponent(script.Name) == null)
						{
							NameArrays[i][j].gameObject.AddComponent(script);
						}
						//Script specific
						if (script is UseEffects)
						{
							NameArrays[i][j].GetComponent<UseEffects>().setIndex = setArray[i];
						}
						else if (script is BulletPenetration)
						{ //Don't just use 'else' in case additional scripts are added to the wizard
							NameArrays[i][j].GetComponent<BulletPenetration>().penetrateValue = penetrationArray[i];
						}
					}
				}
				else
				{
					for (int j = 0; j < NameArrays[i].Count; j++)
					{
						if (NameArrays[i][j].GetComponent(script.Name) != null)
							DestroyImmediate(NameArrays[i][j].gameObject.GetComponent(script.Name));
					}
				}
			}
		}
	}
}