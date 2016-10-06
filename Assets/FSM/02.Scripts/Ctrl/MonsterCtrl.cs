using System;
using System.Collections;
using UnityEngine;

public class MonsterCtrl : MonoBehaviour, Attackable
{
    private IState _myState;
    private bool _isDead = false;
    private bool _unControllable = false;

	[HideInInspector]
    public GameObject _player;
    private Transform _playerTransform;
    private Transform _monsterTransform;
	public Vector3 _startPosition;
    public Vector3 _checkPoint;

    private Animator _animator;
	private NavMeshAgent _nav;

    public double TraceRange = 5.0;
    public double AttackRange = 2.0;
    public float Speed = 1.0f;

    public GameObject _weapon;
    private Collider _weaponCollider;

	[HideInInspector]
	public bool FoundUser = false;

	public float Angle = 120f;
	public float SightDistance = 10f;

	// Use this for initialization
	private void Awake()
    {
        // Find player transform
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        _monsterTransform = GetComponent<Transform>();
		
        // Get components
        _animator = GetComponent<Animator>();
        _weaponCollider = _weapon.GetComponent<Collider>();
//	    _nav = GetComponent<NavMeshAgent>();

        ChangeState(StateManager.GetState(StateManager.State.Patrol));
        StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        while (!_isDead)
        {
            _myState.Run(this);
            yield return null;
        }
    }

    // Return true when the player is found.
    public bool Detect()
	{
		Vector3 playerDirection = _playerTransform.position - transform.position;

		// Can hear Player
		double distance = Vector3.Distance(_playerTransform.position, _monsterTransform.position);
	    if (distance < TraceRange)
		{
			Debug.DrawLine(transform.position, _playerTransform.position, Color.red);
			_checkPoint = _playerTransform.position;
			return true;
		}

		// Can see Player
		RaycastHit hit;
		if (playerDirection.magnitude < SightDistance &&                    // In sight distance
			Vector3.Angle(playerDirection, transform.forward) < Angle / 2 &&    // In sight angle
			Physics.Raycast(transform.position + transform.up, playerDirection, out hit, SightDistance) &&
			hit.collider.gameObject.tag.Equals("Player"))                   // No obstacle 
		{
			Debug.DrawLine(transform.position, _playerTransform.position, Color.red);
			_checkPoint = _playerTransform.position;
			return true;
		}

//		Debug.DrawLine(transform.position, _playerTransform.position, Color.green);
		return false;
    }

    // Return true when the player is closer than AttackRange.
    public bool IsAttackable()
    {
        double distance = Vector3.Distance(_playerTransform.position, _monsterTransform.position);
        return distance < AttackRange;
    }

    // Move to checkpoint
    public void Move()
    {
        // Don't move if is attacking
        if (!IsMovable()) return;

        Vector3 distanceVector = _checkPoint - _monsterTransform.position;
        if (distanceVector.magnitude > 0.5f)
        {
            _animator.SetBool("IsMoving", true);
//			_nav.destination = _checkPoint;
			transform.LookAt(_playerTransform);
	        transform.position += Time.deltaTime * Speed * transform.forward;
        }
        else
        {
            _animator.SetBool("IsMoving", false);
			transform.LookAt(_playerTransform);
			_checkPoint = _startPosition;
        }
    }

    // Attack player
    public void Attack()
	{
		if (IsMovable())
        {
            SetTrigger("Attack");
            CallFreeze();
		}
	}

    public void SetWeaponCollider(int oneIsTrue)
    {
        _weaponCollider.enabled = oneIsTrue == 1;
    }

    public void ChangeState(IState state)
    {
        _myState = state;
    }

	public void GotHit()
	{
		GotHit(0);
	}

    public void GotHit(int damage)
    {
        SetTrigger("GotHit");
        CallFreeze();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "WEAPON")
        {
            GotHit(0);
        }
    }

    // Return is movable or not
    private bool IsMovable()
    {
        return !(_unControllable || _animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Damage"));
    }

    private void CallFreeze()
    {
        _unControllable = true;
        StartCoroutine(Freeze());
    }

    public IEnumerator Freeze()
    {
        yield return new WaitForSeconds(0.1f);
        _unControllable = false;
    }

    private void SetTrigger(String trigger)
    {
        SetWeaponCollider(0);
        _animator.SetTrigger(trigger);
    }

	public void SetStartingPoint()
	{
		_startPosition = transform.position;
		_checkPoint = _startPosition;
	}

	public void Die()
	{
		_isDead = true;
		StopAllCoroutines();
		StartCoroutine(CoDie());
	}

	private IEnumerator CoDie()
	{
		SetTrigger("Die");
		yield return new WaitForSeconds(3f);
		Destroy(gameObject);
	}
}