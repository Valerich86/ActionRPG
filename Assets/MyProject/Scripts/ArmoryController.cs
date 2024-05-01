using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;

public class ArmoryController : MonoBehaviour
{
    [SerializeField] private GameObject _blacksmith;
    [SerializeField] private Transform[] _movePoints;

    private NavMeshAgent _agent;
    private Animator _animator;
    public void Start()
    {
        _animator = _blacksmith.GetComponent<Animator>();
        _agent = _blacksmith.GetComponent<NavMeshAgent>();
        Move();
    }

    private void Move()
    {
        _animator.SetFloat("Speed", 0.05f);
        foreach (Transform point in _movePoints)
        {
            while (Vector3.Distance(_blacksmith.transform.position, point.position) >= 0.1)
            {
                _agent.SetDestination(point.position);
            }
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            player.ResetMovement(false);
            player.transform.LookAt(transform.position);
            StaticData.OnCameraChanged?.Invoke(1);
            _animator.SetFloat("Speed", 0.05f);
            while (Vector3.Distance(_blacksmith.transform.position, transform.position) >= 0.1)
                _agent.SetDestination(transform.position);
            _animator.SetFloat("Speed", 0f);
            //transform.LookAt(player.transform.position);
        }
    }
}
