
using UnityEngine;

public class AttackController : MonoBehaviour
{
    [HideInInspector] public WeaponController Weapon;
    [HideInInspector] public bool IsAttacking;

    [SerializeField] private LayerMask _attackingMask;
    
    private Collider[] _hits = new Collider[5];
    private Animator _animator;


    private void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
        ResetAttack();
    }

    private void ResetAttack() => IsAttacking = false;

    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0) && IsAttacking == false)
        {
            IsAttacking = true;
            int index = Random.Range(0, 3);
            if (Weapon.Type == AttackType.melee)
            {
                _animator.SetInteger("AttackIndex", index);
                _animator.SetTrigger("MeleeAttack");
            }
            float cooldown = (index == 0) ? Weapon.Cooldown - 1 : Weapon.Cooldown;
            Invoke("ResetAttack", cooldown);
        }
    }
    void Attacking()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position + Weapon.WeaponRange, Weapon.Range, _hits, _attackingMask);
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward + Weapon.WeaponRange, Weapon.Range);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position + transform.forward + Weapon.WeaponRange, 0.2f);
    }
}
