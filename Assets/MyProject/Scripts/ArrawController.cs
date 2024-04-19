using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrawController : MonoBehaviour
{
    private Transform _parent;
    private Rigidbody _rigidbody;
    private Collider _collider;
    private bool _isStuck = false;
    private float _damage = 10;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (_isStuck)
        {
            transform.position = _parent.position;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        _rigidbody.isKinematic = true;
        _collider.enabled = false;
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
            {
                _parent = player.SetCurrentVulnerablePart();
            }
            if (collision.gameObject.TryGetComponent<HPController>(out HPController hpController))
            {
                hpController.TakeDamage(_damage);
            }
            transform.parent = _parent;
            _isStuck = true;
        }
    }
}
