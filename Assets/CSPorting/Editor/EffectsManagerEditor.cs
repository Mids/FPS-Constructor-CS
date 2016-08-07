using UnityEngine;
using UnityEditor;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	[CustomEditor(typeof(EffectsManager))]
	public class EffectsManagerEditor : Editor
	{

		public EffectsManager Instance { get { return (EffectsManager)target; } }
		public string[] nameArray = new string[1];

		void OnEnable()
		{
			if (Instance.highestSet == 0)
			{
				Instance.CreateSet();
			}
			updateNameArray();
		}

		public void updateNameArray()
		{
			nameArray = new string[Instance.setNameArray.Length + 1];
			for (int i = 0; i < nameArray.Length - 1; i++)
			{
				nameArray[i] = Instance.setNameArray[i];
			}
			nameArray[nameArray.Length - 1] = "New Set";
		}
		void OnInspectorGUI()
		{
			bool markForDelete = false;

			EditorGUIUtility.LookLikeInspector();
			EditorGUILayout.BeginVertical();
			EffectsManager.maxDecals = EditorGUILayout.IntField("  Max. Decals in World: ", EffectsManager.maxDecals);



			if (Instance.highestSet > 0)
			{
				Instance.selectedSet = EditorGUILayout.Popup("  Select Set to Edit: ", Instance.selectedSet, nameArray);
				if (Instance.selectedSet == nameArray.Length - 1)
				{
					if (Instance.highestSet < EffectsManager.maxSets)
					{
						Instance.CreateSet();
						updateNameArray();
					}
					else
					{
						Debug.LogWarning("Effects Set not created - too many decal sets exist already!");
					}
				}
				string setName = Instance.setNameArray[Instance.selectedSet];

				//EditorGUILayout.Separator();
				setName = EditorGUILayout.TextField("  Effects Set Name:  ", setName);
				if (Instance.setNameArray[Instance.selectedSet] != setName)
				{
					Instance.Rename(setName);
					updateNameArray();
				}

				EditorGUILayout.Separator();
				if (GUILayout.Button("Delete Set"))
				{
					markForDelete = true;
				}

				EditorGUILayout.Separator();


				if (Instance.setArray.Length > 0)
				{
					//Bullet Decals
					Instance.setArray[Instance.selectedSet].bulletDecalsFolded = EditorGUILayout.Foldout(Instance.setArray[Instance.selectedSet].bulletDecalsFolded, "  Bullet Decals: ");
					if (Instance.setArray[Instance.selectedSet].bulletDecalsFolded)
					{
						for (int i = 0; i < Instance.setArray[Instance.selectedSet].bulletDecals.Length; i++)
						{
							Instance.setArray[Instance.selectedSet].bulletDecals[i] = (GameObject)EditorGUILayout.ObjectField("  Bullet Decal: ", Instance.setArray[Instance.selectedSet].bulletDecals[i], typeof(GameObject), false);
							if (Instance.setArray[Instance.selectedSet].bulletDecals[i] == null)
							{
								if (i < Instance.setArray[Instance.selectedSet].bulletDecals.Length)
								{
									if (Instance.setArray[Instance.selectedSet].bulletDecals[i + 1] == null)
									{
										Instance.setArray[Instance.selectedSet].lastBulletDecal = i;
										break;
									}
									else
									{
										for (int m = i + 1; m < Instance.setArray[Instance.selectedSet].bulletDecals.Length; m++)
										{
											if (Instance.setArray[Instance.selectedSet].bulletDecals[m] == null)
											{
												Instance.setArray[Instance.selectedSet].bulletDecals[m - 1] = Instance.setArray[Instance.selectedSet].bulletDecals[m];
												break;
											}
											else
											{
												Instance.setArray[Instance.selectedSet].bulletDecals[m - 1] = Instance.setArray[Instance.selectedSet].bulletDecals[m];
											}
										}
									}
								}
							}
						}
					}

					//Dent Decals
					Instance.setArray[Instance.selectedSet].dentDecalsFolded = EditorGUILayout.Foldout(Instance.setArray[Instance.selectedSet].dentDecalsFolded, "  Dent Decals: ");
					if (Instance.setArray[Instance.selectedSet].dentDecalsFolded)
					{
						for (int i = 0; i < Instance.setArray[Instance.selectedSet].dentDecals.Length; i++)
						{
							Instance.setArray[Instance.selectedSet].dentDecals[i] = (GameObject)EditorGUILayout.ObjectField("  Dent Decal: ", Instance.setArray[Instance.selectedSet].dentDecals[i], typeof(GameObject), false);
							if (Instance.setArray[Instance.selectedSet].dentDecals[i] == null)
							{
								if (i < Instance.setArray[Instance.selectedSet].dentDecals.Length)
								{
									if (Instance.setArray[Instance.selectedSet].dentDecals[i + 1] == null)
									{
										Instance.setArray[Instance.selectedSet].lastDentDecal = i;
										break;
									}
									else
									{
										for (int m = i + 1; m < Instance.setArray[Instance.selectedSet].dentDecals.Length; m++)
										{
											if (Instance.setArray[Instance.selectedSet].dentDecals[m] == null)
											{
												Instance.setArray[Instance.selectedSet].dentDecals[m - 1] = Instance.setArray[Instance.selectedSet].dentDecals[m];
												break;
											}
											else
											{
												Instance.setArray[Instance.selectedSet].dentDecals[m - 1] = Instance.setArray[Instance.selectedSet].dentDecals[m];
											}
										}
									}
								}
							}
						}
					}

					//Hit Particles
					Instance.setArray[Instance.selectedSet].hitParticlesFolded = EditorGUILayout.Foldout(Instance.setArray[Instance.selectedSet].hitParticlesFolded, "  Hit Particles: ");
					if (Instance.setArray[Instance.selectedSet].hitParticlesFolded)
					{
						for (int i = 0; i < Instance.setArray[Instance.selectedSet].hitParticles.Length; i++)
						{
							Instance.setArray[Instance.selectedSet].hitParticles[i] = (GameObject)EditorGUILayout.ObjectField("  Hit Particle: ", Instance.setArray[Instance.selectedSet].hitParticles[i], typeof(GameObject), false);
							if (Instance.setArray[Instance.selectedSet].hitParticles[i] == null)
							{
								if (i < Instance.setArray[Instance.selectedSet].hitParticles.Length)
								{
									if (Instance.setArray[Instance.selectedSet].hitParticles[i + 1] == null)
									{
										Instance.setArray[Instance.selectedSet].lastHitParticle = i;
										break;
									}
									else
									{
										for (int m = i + 1; m < Instance.setArray[Instance.selectedSet].hitParticles.Length; m++)
										{
											if (Instance.setArray[Instance.selectedSet].hitParticles[m] == null)
											{
												Instance.setArray[Instance.selectedSet].hitParticles[m - 1] = Instance.setArray[Instance.selectedSet].hitParticles[m];
												break;
											}
											else
											{
												Instance.setArray[Instance.selectedSet].hitParticles[m - 1] = Instance.setArray[Instance.selectedSet].hitParticles[m];
											}
										}
									}
								}
							}
						}
					}

					//Bullet Sounds
					Instance.setArray[Instance.selectedSet].bulletSoundsFolded = EditorGUILayout.Foldout(Instance.setArray[Instance.selectedSet].bulletSoundsFolded, "  Bullet Sounds: ");
					if (Instance.setArray[Instance.selectedSet].bulletSoundsFolded)
					{
						for (int i = 0; i < Instance.setArray[Instance.selectedSet].bulletSounds.Length; i++)
						{
							Instance.setArray[Instance.selectedSet].bulletSounds[i] = (AudioClip)EditorGUILayout.ObjectField("  Bullet Sound: ", Instance.setArray[Instance.selectedSet].bulletSounds[i], typeof(AudioClip), false);
							if (Instance.setArray[Instance.selectedSet].bulletSounds[i] == null)
							{
								if (i < Instance.setArray[Instance.selectedSet].bulletSounds.Length)
								{
									if (Instance.setArray[Instance.selectedSet].bulletSounds[i + 1] == null)
									{
										Instance.setArray[Instance.selectedSet].lastBulletSound = i;
										break;
									}
									else
									{
										for (int m = i + 1; m < Instance.setArray[Instance.selectedSet].bulletSounds.Length; m++)
										{
											if (Instance.setArray[Instance.selectedSet].bulletSounds[m] == null)
											{
												Instance.setArray[Instance.selectedSet].bulletSounds[m - 1] = Instance.setArray[Instance.selectedSet].bulletSounds[m];
												break;
											}
											else
											{
												Instance.setArray[Instance.selectedSet].bulletSounds[m - 1] = Instance.setArray[Instance.selectedSet].bulletSounds[m];
											}
										}
									}
								}
							}
						}
					}

					//Collision Sounds
					Instance.setArray[Instance.selectedSet].collisionSoundsFolded = EditorGUILayout.Foldout(Instance.setArray[Instance.selectedSet].collisionSoundsFolded, "  Collision Sounds: ");
					if (Instance.setArray[Instance.selectedSet].collisionSoundsFolded)
					{
						for (int i = 0; i < Instance.setArray[Instance.selectedSet].collisionSounds.Length; i++)
						{
							Instance.setArray[Instance.selectedSet].collisionSounds[i] = (AudioClip)EditorGUILayout.ObjectField("  Collision Sound: ", Instance.setArray[Instance.selectedSet].collisionSounds[i], typeof(AudioClip), false);
							if (Instance.setArray[Instance.selectedSet].collisionSounds[i] == null)
							{
								if (i < Instance.setArray[Instance.selectedSet].collisionSounds.Length)
								{
									if (Instance.setArray[Instance.selectedSet].collisionSounds[i + 1] == null)
									{
										Instance.setArray[Instance.selectedSet].lastCollisionSound = i;
										break;
									}
									else
									{
										for (int m = i + 1; m < Instance.setArray[Instance.selectedSet].collisionSounds.Length; m++)
										{
											if (Instance.setArray[Instance.selectedSet].collisionSounds[m] == null)
											{
												Instance.setArray[Instance.selectedSet].collisionSounds[m - 1] = Instance.setArray[Instance.selectedSet].collisionSounds[m];
												break;
											}
											else
											{
												Instance.setArray[Instance.selectedSet].collisionSounds[m - 1] = Instance.setArray[Instance.selectedSet].collisionSounds[m];
											}
										}
									}
								}
							}
						}
					}

					//Footstep Sounds
					Instance.setArray[Instance.selectedSet].footstepSoundsFolded = EditorGUILayout.Foldout(Instance.setArray[Instance.selectedSet].footstepSoundsFolded, "  Footstep Sounds: ");
					if (Instance.setArray[Instance.selectedSet].footstepSoundsFolded)
					{
						for (int i = 0; i < Instance.setArray[Instance.selectedSet].footstepSounds.Length; i++)
						{
							Instance.setArray[Instance.selectedSet].footstepSounds[i] = (AudioClip)EditorGUILayout.ObjectField("  Footstep Sound: ", Instance.setArray[Instance.selectedSet].footstepSounds[i], typeof(AudioClip), false);
							if (Instance.setArray[Instance.selectedSet].footstepSounds[i] == null)
							{
								if (i < Instance.setArray[Instance.selectedSet].footstepSounds.Length)
								{
									if (Instance.setArray[Instance.selectedSet].footstepSounds[i + 1] == null)
									{
										Instance.setArray[Instance.selectedSet].lastFootstepSound = i;
										break;
									}
									else
									{
										for (int m = i + 1; m < Instance.setArray[Instance.selectedSet].footstepSounds.Length; m++)
										{
											if (Instance.setArray[Instance.selectedSet].footstepSounds[m] == null)
											{
												Instance.setArray[Instance.selectedSet].footstepSounds[m - 1] = Instance.setArray[Instance.selectedSet].footstepSounds[m];
												break;
											}
											else
											{
												Instance.setArray[Instance.selectedSet].footstepSounds[m - 1] = Instance.setArray[Instance.selectedSet].footstepSounds[m];
											}
										}
									}
								}
							}
						}
					}

					//Crawl Sounds	
					Instance.setArray[Instance.selectedSet].crawlSoundsFolded = EditorGUILayout.Foldout(Instance.setArray[Instance.selectedSet].crawlSoundsFolded, "  Crawl (Prone) Sounds: ");
					if (Instance.setArray[Instance.selectedSet].crawlSoundsFolded)
					{
						for (int i = 0; i < Instance.setArray[Instance.selectedSet].crawlSounds.Length; i++)
						{
							Instance.setArray[Instance.selectedSet].crawlSounds[i] = (AudioClip)EditorGUILayout.ObjectField("  Crawl Sound: ", Instance.setArray[Instance.selectedSet].crawlSounds[i], typeof(AudioClip), false);
							if (Instance.setArray[Instance.selectedSet].crawlSounds[i] == null)
							{
								if (i < Instance.setArray[Instance.selectedSet].crawlSounds.Length)
								{
									if (Instance.setArray[Instance.selectedSet].crawlSounds[i + 1] == null)
									{
										Instance.setArray[Instance.selectedSet].lastCrawlSound = i;
										break;
									}
									else
									{
										for (int m = i + 1; m < Instance.setArray[Instance.selectedSet].crawlSounds.Length; m++)
										{
											if (Instance.setArray[Instance.selectedSet].crawlSounds[m] == null)
											{
												Instance.setArray[Instance.selectedSet].crawlSounds[m - 1] = Instance.setArray[Instance.selectedSet].crawlSounds[m];
												break;
											}
											else
											{
												Instance.setArray[Instance.selectedSet].crawlSounds[m - 1] = Instance.setArray[Instance.selectedSet].crawlSounds[m];
											}
										}
									}
								}
							}
						}
					}
					/*	AudioClip[] crawlSounds = new AudioClip[maxOfEach];
		bool  crawlSoundsFolded = false;
		int lastCrawlSound = 0;*/
				}
			}
			if (markForDelete)
			{
				if (Instance.highestSet == 1)
				{
					Debug.Log("Can't Delete Last Effects Set");
				}
				else
				{
					Instance.DeleteSet(Instance.selectedSet);
					updateNameArray();
					Instance.selectedSet = 0;
				}
			}
			if (GUI.changed)
			{
				EditorUtility.SetDirty(Instance);
			}
		}

	}
}