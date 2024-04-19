
using UnityEngine;

public class AttackController : MonoBehaviour
{
    
    [HideInInspector] public bool IsAttacking;

    [SerializeField] private LayerMask _attackingMask;
    [SerializeField] private GameObject _arrowClone;
    [SerializeField] private GameObject _arrowFXClone;
    [SerializeField] private Transform _arrowPoint;
    [SerializeField] private float _arrowSpawnTime = 1;
    [SerializeField] private float _bowShotForce;
    [SerializeField][Range(0, 1)] private float _hitReactionTime;
    
    private Collider[] _hits = new Collider[5];
    private Animator _animator;
    private WeaponController _weapon;

    private void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
        ResetAttack();
    }

    private void ResetAttack() => IsAttacking = false;

    public void SetWeapon(WeaponController weapon) => _weapon = weapon;

    public void Attack(AttackType type)
    {
        if (IsAttacking == false)
        {
            float cooldown = 0;
            IsAttacking = true;
            
            if (type == AttackType.melee)
            {
                int index = Random.Range(0, 4);
                _animator.SetInteger("AttackIndex", index);
                _animator.SetTrigger("MeleeAttack");
                cooldown = (index == 1) ? _weapon.MeleeCooldown  : _weapon.MeleeCooldown - 1;
                if (index != 1) Invoke("MeleeAttackCheck", _hitReactionTime);
                else Invoke("MeleeAttackCheck", _hitReactionTime + 0.5f);

            }
            if (type == AttackType.long_range)
            {
                _animator.SetTrigger("BowShot");
                Invoke("ArrowSpawn", _arrowSpawnTime);
                cooldown = _weapon.Cooldown;
            }
            
            Invoke("ResetAttack", cooldown);
        }
    }
    
    void MeleeAttackCheck()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position + _weapon.WeaponRange, _weapon.Range, _hits, _attackingMask);

        for (int i = 0; i < count; i++)
        {
            if (_hits[i].TryGetComponent<HPController>(out HPController hpController))
            {
                hpController.TakeDamage(_weapon.Damage);
            }
        }
    }

    void ArrowSpawn()
    {
        GameObject arrow = Instantiate(_arrowClone, _arrowPoint.position, _arrowPoint.rotation);
        GameObject arrowFX = Instantiate(_arrowFXClone, _arrowPoint.position, _arrowPoint.rotation);
        arrow.GetComponent<Rigidbody>().AddForce(transform.forward * _bowShotForce, ForceMode.Impulse);
        Destroy(arrow, 20f);
    }

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position + transform.forward + _weapon.WeaponRange, _weapon.Range);

    //    Gizmos.color = Color.green;
    //    Gizmos.DrawSphere(transform.position + transform.forward + _weapon.WeaponRange, 0.2f);
    //}
}
