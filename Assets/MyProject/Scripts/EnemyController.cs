using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class EnemyController : MonoBehaviour, IMortal
{
    [SerializeField] private float _agentEnabledDistance = 50;
    [SerializeField] private float _shootingDistance = 20;
    [SerializeField] private float _meleeAttackDistance = 2;
    [SerializeField] private bool _canRoll;
    [SerializeField] private WeaponController _weapon;

    private AttackController _attackController;
    private Transform _player;
    private NavMeshAgent _agent;
    private Animator _animator;
    private bool _isSearching = true;

    private void Start()
    {
        _player = FindObjectOfType<PlayerController>().transform;
        _player.gameObject.GetComponent<PlayerController>().OnPlayerDying += OnPlayerDying;
        _agent = GetComponent<NavMeshAgent>();
        _attackController = GetComponent<AttackController>();
        _attackController.SetWeapon(_weapon);
        _animator = GetComponent<Animator>();
        FindNewPoint();
    }

    private void OnPlayerDying()
    {
        _agent.enabled = false;
        _animator.SetFloat("Speed", 0);
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, _player.position);

        if (_agent.enabled)
        {
            if (_isSearching && !_agent.pathPending && _agent.remainingDistance <= 3)
            {
                FindNewPoint();
            }
            else if (distance <= _agentEnabledDistance)
            {
                _isSearching = false;  
                _agent.SetDestination(_player.position);
                transform.LookAt(_player.position);
                if (_canRoll && distance <= _meleeAttackDistance + 2f && distance >= _meleeAttackDistance + 1.8f)
                {
                    _animator.SetTrigger("RollForward");
                    _agent.speed = 3;
                }
                else if (distance <= _shootingDistance && distance > _meleeAttackDistance * 2 && _weapon.Type == AttackType.long_range)
                {
                    _animator.SetFloat("Speed", 1);
                    _agent.speed = 3;
                    _attackController.Attack(AttackType.long_range);
                }
                else if (distance <= _meleeAttackDistance)
                {
                    _animator.SetFloat("Speed", 0.1f);
                    _agent.speed = 1;
                    _attackController.Attack(AttackType.melee);
                }
                else
                {
                    _animator.SetFloat("Speed", 1);
                    _agent.speed = 3;
                }
            }
        }
    }

    private void FindNewPoint()
    {
        _animator.SetFloat("Speed", 0.05f);
        _agent.speed = 0.5f;

        NavMeshTriangulation data = NavMesh.CalculateTriangulation();
        int index = Random.Range(0, data.vertices.Length);
        Vector3 randomPoint = data.vertices[index];
        float distanceToPoint = Vector3.Distance(transform.position, randomPoint);
        _agent.SetDestination(randomPoint);
    }


    public void Dying()
    {
        _agent.enabled = false;
        GetComponent<Collider>().enabled = false;
        Destroy(gameObject, 20f);
    }


    public bool SetRollAbility()
    {
        return _canRoll;
    }
}
