using UnityEngine;
using System.Collections;
using ooparts.fpsctorcs;
namespace ooparts.fpsctorcs
{
public class JumpCrouchEffects : MonoBehaviour {

	public float jumpHeight = .15f;
	public float crouchHeight = -.1f;
	public float proneHeight = -.2f;

	public float crouchSpeed = 1;
	public float jumpAdjustSpeed = 1;
	public float landingHeight = -.06f;
	public float landAdjustSpeed = 1;
	private bool airborne = false;
	private bool landingAdjusted = true;
	private float targetHeight = 0;
	private bool aim = false;
	private float speed;
	private CharacterMotorDB CM;
	private float aimSpeed = 1;

	void Update()
	{
		if (!CM.grounded)
		{
			targetHeight = jumpHeight;
			airborne = true;
			speed = jumpAdjustSpeed;
		}
		else if (airborne)
		{
			airborne = false;
			targetHeight = landingHeight;
			landingAdjusted = false;
			speed = landAdjustSpeed;
		}
		else if (CharacterMotorDB.crouching && landingAdjusted)
		{
			targetHeight = crouchHeight;
			speed = crouchSpeed;
		}
		else if (CharacterMotorDB.prone && landingAdjusted)
		{
			targetHeight = proneHeight;
			speed = crouchSpeed;
		}
		else if (landingAdjusted)
		{
			targetHeight = 0;
			speed = crouchSpeed;
		}

		if (aim && landingAdjusted)
		{
			targetHeight = 0;
			speed = crouchSpeed * 2;
		}
		Vector3 localPos = transform.localPosition;
		localPos.y = Mathf.Lerp(transform.localPosition.y, targetHeight, Time.deltaTime * speed);
		if (Mathf.Abs(localPos.y - targetHeight) < .1)
		{
			landingAdjusted = true;
		}
		transform.localPosition = localPos;
	}

	void Start()
	{
		CM = GameObject.FindWithTag("Player").GetComponent<CharacterMotorDB>();
		targetHeight = 0;
	}

	void Aiming()
	{
		aim = true;
	}

	void StopAiming()
	{
		aim = false;
	}
}
}