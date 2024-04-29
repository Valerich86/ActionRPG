
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, IMortal
{
    [field: SerializeField] public EnemySO Enemy { get; private set; }

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
        _attackController.SetEnemyWeapon(Enemy.Weapon);
        _hpController.SetStartHealth(Enemy.MaxHP);
        _maxSpeed = Enemy.Speed;
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
            else if (distance <= Enemy.AgentEnabledDistance)
            {
                _isSearching = false;  
                _agent.SetDestination(_player.position);
                transform.LookAt(_player.position);
                if (Enemy.Defense == DefenseType.CanRoll && distance <= Enemy.MeleeAttackDistance + 2f && distance >= Enemy.MeleeAttackDistance + 1.8f)
                {
                    _animator.SetTrigger("RollForward");
                    _agent.speed = _maxSpeed;
                }
                else if (distance <= Enemy.ShootingDistance && distance > Enemy.MeleeAttackDistance * 2 && Enemy.Weapon.WeaponSO.Type == AttackType.long_range)
                {
                    _animator.SetFloat("Speed", 1);
                    _agent.speed = _maxSpeed;
                    _attackController.Attack();
                }
                else if (distance <= Enemy.MeleeAttackDistance)
                {
                    _animator.SetFloat("Speed", 0.1f);
                    _agent.speed = _maxSpeed/3;
                    _attackController.Attack();
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
        StaticData.OnEnemyDying?.Invoke(this);
        _agent.enabled = false;
        GetComponent<Collider>().enabled = false;
        Destroy(gameObject, 30f);
    }

}
