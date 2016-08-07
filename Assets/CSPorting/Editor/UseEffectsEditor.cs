using UnityEngine;
using UnityEditor;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	[CustomEditor(typeof(UseEffects))]
	public class UseEffectsEditor : Editor
	{
		public UseEffects Instance { get { return (UseEffects)target; } }
		EffectsManager manager;
		int prevIndex;

		void OnInspectorGUI()
		{
			manager = (EffectsManager)FindObjectOfType(typeof(EffectsManager));
			if (manager != null)
			{
				if (manager.setNameArray != null)
				{
					if (manager.setArray[0] != null)
					{
						prevIndex = Instance.setIndex;
						Instance.setIndex = EditorGUILayout.Popup(Instance.setIndex, manager.setNameArray);
						if (prevIndex != Instance.setIndex)
						{
							// set all colliders of the same name to use the same set.
							Collider[] colliders = (Collider[])FindObjectsOfType(typeof(Collider));
							foreach (Collider c in colliders)
							{
								if (c.name == Instance.transform.name)
								{
									if (c.GetComponent<UseEffects>())
									{
										c.GetComponent<UseEffects>().setIndex = Instance.setIndex;
									}
								}
							}
							prevIndex = Instance.setIndex;
						}
					}
					else
					{
						EditorGUILayout.LabelField("No sets exist", "");
					}
				}
			}
			if (GUI.changed)
				EditorUtility.SetDirty(Instance);
		}
	}
}