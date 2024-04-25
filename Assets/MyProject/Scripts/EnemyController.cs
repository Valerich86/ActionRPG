using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class EnemyController : MonoBehaviour, IMortal
{
    [SerializeField] private EnemySO _enemy;

    private AttackController _attackController;
    private HPController _hpController;
    private Transform _player;
    private NavMeshAgent _agent;
    private Animator _animator;
    private bool _isSearching = true;
    private float _maxSpeed;

    private void Start()
    {
        _player = FindObjectOfType<PlayerController>().transform;
        _player.gameObject.GetComponent<PlayerController>().OnPlayerDying += OnPlayerDying;
        _agent = GetComponent<NavMeshAgent>();
        _attackController = GetComponent<AttackController>();
        _hpController = GetComponent<HPController>();   
        _animator = GetComponent<Animator>();
        _attackController.SetWeapon(_enemy.Weapon);
        _hpController.SetStartHealth(_enemy.MaxHP);
        _maxSpeed = _enemy.Speed;
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
            else if (distance <= _enemy.AgentEnabledDistance)
            {
                _isSearching = false;  
                _agent.SetDestination(_player.position);
                transform.LookAt(_player.position);
                if (_enemy.Defense == DefenseType.CanRoll && distance <= _enemy.MeleeAttackDistance + 2f && distance >= _enemy.MeleeAttackDistance + 1.8f)
                {
                    _animator.SetTrigger("RollForward");
                    _agent.speed = _maxSpeed;
                }
                else if (distance <= _enemy.ShootingDistance && distance > _enemy.MeleeAttackDistance * 2 && _enemy.Weapon.Type == AttackType.long_range)
                {
                    _animator.SetFloat("Speed", 1);
                    _agent.speed = _maxSpeed;
                    _attackController.Attack(AttackType.long_range);
                }
                else if (distance <= _enemy.MeleeAttackDistance)
                {
                    _animator.SetFloat("Speed", 0.1f);
                    _agent.speed = _maxSpeed/3;
                    _attackController.Attack(AttackType.melee);
                }
                else
                {
                    _animator.SetFloat("Speed", 1);
                    _agent.speed = _maxSpeed;
                }
            }
        }
    }

    private void FindNewPoint()
    {
        _animator.SetFloat("Speed", 0.05f);
        _agent.speed = _maxSpeed / 3;

        NavMeshTriangulation data = NavMesh.CalculateTriangulation();
        int index = Random.Range(0, data.vertices.Length);
        Vector3 randomPoint = data.vertices[index];
        _agent.SetDestination(randomPoint);
    }


    public void Dying()
    {
        _agent.enabled = false;
        GetComponent<Collider>().enabled = false;
        Destroy(gameObject, 10f);
    }

}
