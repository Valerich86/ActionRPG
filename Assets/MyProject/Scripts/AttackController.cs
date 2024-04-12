
using UnityEngine;

public class AttackController : MonoBehaviour
{
    
    [HideInInspector] public bool IsAttacking;

    [SerializeField] private LayerMask _attackingMask;
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

    public void Attack()
    {
        if (IsAttacking == false)
        {
            float cooldown = 0;
            IsAttacking = true;
            int index = Random.Range(0, 3);
            if (_weapon.Type == AttackType.melee)
            {
                _animator.SetInteger("AttackIndex", index);
                _animator.SetTrigger("MeleeAttack");
                Invoke("AttackCheck", _hitReactionTime);
                cooldown = (index == 0) ? _weapon.Cooldown - 1 : _weapon.Cooldown;
            }
            Invoke("ResetAttack", cooldown);
        }
    }
    
    void AttackCheck()
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


    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position + transform.forward + _weapon.WeaponRange, _weapon.Range);

    //    Gizmos.color = Color.green;
    //    Gizmos.DrawSphere(transform.position + transform.forward + _weapon.WeaponRange, 0.2f);
    //}
}
