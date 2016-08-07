using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class GraphicsObject : MonoBehaviour
	{
		public GameObject obj;
		public bool val = true;
		public bool instant = false;
		private bool cache;
		private bool applied = false;
		private bool tempInstant = false;

		public void Apply()
		{
			cache = obj.activeSelf;
			if (val)
			{
				obj.SetActive(true);
			}
			else
			{
				obj.SetActive(false);
			}

			Renderer[] gos = obj.GetComponentsInChildren<Renderer>();

			if (!instant && !tempInstant)
			{
				foreach (Renderer go in gos)
				{
					go.enabled = false;
				}
			}
			else
			{
				foreach (Renderer go in gos)
				{
					go.enabled = true;
				}
			}
			tempInstant = false;

			transform.parent.BroadcastMessage("reapply", SendMessageOptions.DontRequireReceiver);
		}

		public void Remove()
		{
			obj.SetActive(cache);
			if (instant || tempInstant)
			{
				Renderer[] gos = obj.GetComponentsInChildren<Renderer>();
				foreach (Renderer go in gos)
				{
					go.enabled = true;
				}
				tempInstant = false;
			}
		}

		public void TempInstant()
		{
			tempInstant = true;
		}
	}
}