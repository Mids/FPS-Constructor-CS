using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class MaterialChange : MonoBehaviour
	{
		public class rendererChanged
		{
			public Renderer r;
			public int index;

			public rendererChanged(Renderer render1, int num)
			{
				r = render1;
				index = num;
			}
		}

		private Renderer gscript;
		private rendererChanged[] materialsChanged;
		public Material startMat;
		public Material targetMat;
		private string name1;
		private float cache;
		private bool applied = false;

		public void Start()
		{
			findMaterials();
		}


		public void Apply()
		{
			findMaterials();
			applied = true;
			for (int i = 0; i < materialsChanged.Length; i++)
			{
				Renderer renderer1 = materialsChanged[i].r;
				Material[] tempArray = new Material[renderer1.materials.Length];
				for (int q = 0; q < renderer1.materials.Length; q++)
				{
					tempArray[q] = renderer1.materials[q];
				}
				tempArray[materialsChanged[i].index] = targetMat;
				materialsChanged[i].r.materials = tempArray;
			}
		}


		public void Remove()
		{
			applied = false;
			for (int i = 0; i < materialsChanged.Length; i++)
			{
				Renderer renderer1 = materialsChanged[i].r;
				Material[] tempArray = new Material[renderer1.materials.Length];
				for (int q = 0; q < renderer1.materials.Length; q++)
				{
					tempArray[q] = renderer1.materials[q];
				}
				tempArray[materialsChanged[i].index] = startMat;
				materialsChanged[i].r.materials = tempArray;
			}
		}

		public void findMaterials()
		{
			Renderer[] gscripts = this.transform.parent.GetComponentsInChildren<Renderer>() as Renderer[];
			List<rendererChanged> temp = new List<rendererChanged>();
			name1 = startMat.name + " (Instance)";

			for (int q = 0; q < gscripts.Length; q++)
			{
				for (int w = 0; w < gscripts[q].materials.Length; w++)
				{
					if (gscripts[q].materials[w].name.Equals(name1))
					{
						rendererChanged rc = new rendererChanged(gscripts[q], w);
						temp.Add(rc);
					}
				}
			}
			materialsChanged = temp.ToArray();
		}

		public void reapply()
		{
			if (applied)
			{
				Remove();
				Apply();
			}
		}
	}
}