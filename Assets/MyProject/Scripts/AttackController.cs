
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public enum AttackType { melee, long_range }
public class AttackController : MonoBehaviour
{
    [HideInInspector] public bool IsAttacking { get; private set; }
    [HideInInspector] public int ArrowAmount { get; private set; }

    [SerializeField] private LayerMask _attackingMask;
    [SerializeField] private Transform _arrowPoint;
    
    private Collider[] _hits = new Collider[5];
    private Animator _animator;
    private Weapon _weapon;
    private Transform _rightHand;
    private Transform _leftHand;
    private Vector3 _rangeOffset = new Vector3(0, 1, 0);
    private int _arrowAmount;

    private void Start()
    {
        _rightHand = FindObjectOfType<WeaponHand>().transform;
        _leftHand = FindObjectOfType<ShieldHand>().transform;
        _animator = gameObject.GetComponent<Animator>();
        ResetAttack();
    }

    private void ResetAttack() => IsAttacking = false;


    public void SetPlayerWeapon(ItemSO weapon)
    {
        GameObject w = null;
        if (weapon.WeaponSO.Type == AttackType.long_range) w = Instantiate(weapon.Clone, _leftHand);
        else if (weapon.WeaponSO.Type == AttackType.melee) w = Instantiate(weapon.Clone, _rightHand);
        Debug.Log($"type: {weapon.WeaponSO.Type}");
        if (w.TryGetComponent<Rigidbody>(out var rb)) rb.isKinematic = true;
        if (w.TryGetComponent<ItemController>(out var ic)) ic.Deactivate();
        _weapon = weapon.WeaponSO;
    }

    public void Attack()
    {
        if (IsAttacking == false)
        {
            if (_weapon == null) return;
            else
            {
                IsAttacking = true;
                if (_weapon.Type == AttackType.melee) _animator.SetTrigger("Combo");
                if (_weapon.Type == AttackType.long_range && _arrowAmount > 0)
                {
                    _animator.SetTrigger("BowShot");
                    Invoke("ArrowSpawn", .5f);
                }
                Invoke("ResetAttack", _weapon.Cooldown);
            }
        }
    }

    
    void MeleeAttackCheck()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position + _rangeOffset, _weapon.MeleeRange, _hits, _attackingMask);

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
        GameObject arrow = Instantiate(_weapon.ArrowClone, _arrowPoint.position, _arrowPoint.rotation);
        arrow.GetComponent<Rigidbody>().AddForce(transform.forward * _weapon.BowShotForce, ForceMode.Impulse);
        StaticData.OnArrowAmountChanged?.Invoke(-1);
    }

    public void SetCurrentArrowsAmount(int amount) => _arrowAmount = amount;
    internal void SetEnemyWeapon(ItemSO weapon) => _weapon = weapon.WeaponSO;
    

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position + transform.forward + _weapon.WeaponRange, _weapon.Range);

    //    Gizmos.color = Color.green;
    //    Gizmos.DrawSphere(transform.position + transform.forward + _weapon.WeaponRange, 0.2f);
    //}
}
