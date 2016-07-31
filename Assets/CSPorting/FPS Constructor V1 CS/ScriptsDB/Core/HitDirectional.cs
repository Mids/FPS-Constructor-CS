using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
	public class HitDirectional : MonoBehaviour
	{
		private Transform hitPos;
		private Vector3 lastHitPos;
		private float dirEffectTime;
		public Transform obj;
		public Vector3 thePos;
		public Vector3 theRot;

		public void Init(Transform pos, float time)
		{
			StartCoroutine(InitRoutine(pos, time));
		}

		IEnumerator InitRoutine(Transform pos, float time)
		{
			hitPos = pos;
			dirEffectTime = time;
			transform.localPosition = thePos;
			transform.localEulerAngles = theRot;
			yield return new WaitForSeconds(time);
			Destroy(gameObject);
		}
		void LateUpdate()
		{
			Renderer renderer = obj.GetComponent<Renderer>();
			Color color = renderer.material.color;
			color.a = dirEffectTime;
			renderer.material.color = color;

			dirEffectTime -= Time.deltaTime;
			if (hitPos != null)
				lastHitPos = hitPos.position;
			if (dirEffectTime > 0 && hitPos && obj != null)
			{
				Vector3 hitDir = new Vector3(lastHitPos.x, 0, lastHitPos.z) - new Vector3(transform.position.x, 0, transform.position.z);
				Vector3 relativePoint = transform.InverseTransformPoint(lastHitPos);
				float temp;
				if (relativePoint.x < 0.0f)
				{
					temp = -(Vector3.Angle(PlayerWeapons.mainCam.transform.forward, hitDir));
				}
				else if (relativePoint.x > 0.0f)
				{
					temp = (Vector3.Angle(PlayerWeapons.mainCam.transform.forward, hitDir));
				}
				else
				{
					temp = 0;
				}
				obj.transform.localEulerAngles = new Vector3(obj.transform.localEulerAngles.x, temp + 180, obj.transform.localEulerAngles.z);
			}
		}
	}
}