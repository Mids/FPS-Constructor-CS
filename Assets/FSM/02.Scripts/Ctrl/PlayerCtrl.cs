using System;
using UnityEngine;
using System.Collections;
using System.Net;

public class PlayerCtrl : MonoBehaviour, Attackable
{
    private Transform _transform;
    private Animator _animator;

    public float _moveSpeed = 3f;
    public float _rotateSpeed = 100f;
    private bool _isDead = false;
    private bool _unControllable = false;

    public GameObject _weapon;
    private Collider _weaponCollider;

    // Use this for initialization
    void Start()
    {
        _transform = GetComponent<Transform>();
        _animator = GetComponent<Animator>();
        _weaponCollider = _weapon.GetComponent<Collider>();

        StartCoroutine(Action());
    }

    public IEnumerator Action()
    {
        while (!_isDead)
        {
            yield return new WaitForFixedUpdate();
            if (!IsMovable()) continue;

            // Attack
            if (Input.GetButton("Attack"))
            {
                SetTrigger("Attack");
                continue;
            }

            // Move
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector3 movement = Vector3.right*moveHorizontal + Vector3.forward*moveVertical;

            if (movement.Equals(Vector3.zero))
                _animator.SetBool("IsMoving", false);
            else
            {
                _animator.SetBool("IsMoving", true);
                _transform.LookAt(movement + _transform.position);
                _transform.Translate(Vector3.forward*Time.deltaTime*_moveSpeed, Space.Self);
            }
        }
    }

    // Return is movable or not
    private bool IsMovable()
    {
        return !(_unControllable || _animator.GetCurrentAnimatorStateInfo(0).IsName("UNAttack") || _animator.GetCurrentAnimatorStateInfo(0).IsName("UNPain"));
    }


    public void SetWeaponCollider(int oneIsTrue)
    {

        Debug.Log("SetWeaponCollider : " + (oneIsTrue == 1));
        _weaponCollider.enabled = oneIsTrue == 1;
    }

    public void GotHit(int damage)
    {
        SetTrigger("GotHit");
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "MONSTERWEAPON")
        {
            GotHit(0);
        }
    }

    /// <summary>
    /// Make object unControllabe for 0.1secs.
    /// </summary>
    /// <returns></returns>
    IEnumerator Freeze()
    {
        _unControllable = true;
        yield return new WaitForSeconds(0.1f);
        _unControllable = false;
    }
    
    /// <summary>
    /// Set Animation Trigger
    /// </summary>
    /// <param name="trigger">Trigger name</param>
    private void SetTrigger(String trigger)
    {
        StartCoroutine(Freeze());
        SetWeaponCollider(0);
        _animator.SetTrigger(trigger);
    }
}
