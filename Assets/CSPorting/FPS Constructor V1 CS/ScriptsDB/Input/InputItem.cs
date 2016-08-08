using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class InputItem : MonoBehaviour
	{
		[HideInInspector] public bool up; //GetButtonUp
		[HideInInspector] public bool down; //GetButtonDown
		[HideInInspector] public bool got; //GetButton
		[HideInInspector] public float axis; //GetAxis

		public string id; //identifier for this input
	}
}