using System;
using UnityEngine;

public class ArrawController : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private float _damage = 15;
    private Transform _parent;

    private void Start() => _rigidbody = GetComponent<Rigidbody>();

    private void Update()
    {
        if (_parent != null) transform.position = _parent.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        _rigidbody.isKinematic = true;
        if (collision.gameObject.TryGetComponent<HPController>(out HPController hpController))
        {
            int index = UnityEngine.Random.Range(0, hpController.DamagePoints.Length);
            _parent = hpController.DamagePoints[index];
            transform.parent = _parent;
            hpController.TakeDamage(_damage);
        }
        else transform.parent = collision.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            StaticData.OnArrowAmountChanged?.Invoke(1);
            StaticData.OnGlobalHintChanged?.Invoke("+ 1 стрела !", 10);
        }
            
    }
}
