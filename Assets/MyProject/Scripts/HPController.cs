using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPController : MonoBehaviour
{
    [SerializeField] private float _hp;

    private Animator _animator;

    private void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
    }
    public void TakeDamage(float damage)
    {
        _hp -= damage;
        if (_hp <= 0)
        {
            _animator.SetTrigger("Die");
            EventManager.OnDead?.Invoke();
        }
        else
        {
            _animator.SetTrigger("GetHit");
        }
    }

}
