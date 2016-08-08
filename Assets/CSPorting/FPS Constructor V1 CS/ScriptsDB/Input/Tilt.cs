using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;

namespace ooparts.fpsctorcs
{
	public class Tilt : MonoBehaviour
	{
		public enum axes
		{
			x,
			y,
			z
		} //which rotation axis should we use?
		public axes axis = axes.z;

		public enum directions
		{
			positive,
			negative
		} //should it be inverted?
		public directions direction = directions.positive;

		public InputItem input;
		public float sensitivity;
		public float offset;
		public float buffer;

		public void UpdateInput()
		{
			if (axis == axes.x)
			{
				input.axis = Input.acceleration.x;
			}
			else if (axis == axes.y)
			{
				input.axis = Input.acceleration.y;
			}
			else if (axis == axes.z)
			{
				input.axis = Input.acceleration.z;
			}

			input.axis += offset;
			if (input.axis > 0)
			{
				input.axis = Mathf.Clamp(input.axis - buffer, 0, input.axis);
			}
			else
			{
				input.axis = Mathf.Clamp(input.axis + buffer, input.axis, 0);
			}
			input.axis *= sensitivity;
			if (direction == directions.negative)
				input.axis *= -1;
		}
	}
}