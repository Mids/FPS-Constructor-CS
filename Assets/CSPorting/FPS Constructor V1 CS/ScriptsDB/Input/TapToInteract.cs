using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class TapToInteract : MonoBehaviour
	{
		public LayerMask mask;
		public float interactRange = 9;


		void Update()
		{
			CheckForTouch();
		}


		public void CheckForTouch()
		{
			//Iterate through all touches
			for (int i = 0; i < Input.touches.Length; i++)
			{
				//If a touch just began
				if (Input.touches[i].phase == TouchPhase.Began)
				{
					Ray ray;
					GameObject target;

					ray = Camera.main.ScreenPointToRay(Input.touches[i].position);

					target = ReturnRaycastHit(ray);

					if (target != null)
					{
						target.SendMessage("Interact", SendMessageOptions.DontRequireReceiver);
					}
				}
			}
		}

		public GameObject ReturnRaycastHit(Ray ray)
		{
			RaycastHit hit;
			//NOTE: May want to change to raycast all + use layer info
			if (Physics.Raycast(ray, out hit, interactRange, mask))
			{
				return hit.transform.gameObject;
			}

			else
			{
				return null;
			}
		}
	}
}