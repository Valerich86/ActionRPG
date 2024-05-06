
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public enum AttackType { melee, long_range, melee_long }
public class AttackController : MonoBehaviour
{
    [HideInInspector] public bool IsAttacking { get; private set; }
    [HideInInspector] public int ArrowAmount { get; private set; }

    [SerializeField] private LayerMask _attackingMask;
    [SerializeField] private Transform _arrowPoint;
    [SerializeField] private Transform _axePoint;
    
    private Collider[] _hits = new Collider[5];
    private Animator _animator;
    private Weapon _weapon;
    private GameObject _currentWeapon;
    private GameObject _axe;
    private Transform _rightHand;
    private Transform _leftHand;
    private Transform _head;
    private Transform _spine;
    private Vector3 _rangeOffset = new Vector3(0, 1, 0);
    private int _arrowAmount;

    private void Start()
    {
        _rightHand = FindObjectOfType<WeaponHand>().transform;
        _leftHand = FindObjectOfType<ShieldHand>().transform;
        _head = FindObjectOfType<Head>().transform;
        _spine = FindObjectOfType<Spine>().transform;
        _animator = gameObject.GetComponent<Animator>();
        ResetAttack();
    }
    

    public void ResetAttack() => IsAttacking = false;


    public void SetPlayerWeapon(ItemSO weapon)
    {
        if (_currentWeapon != null) Destroy(_currentWeapon);
        if (weapon.WeaponSO.Type == AttackType.long_range) _currentWeapon = Instantiate(weapon.Clone, _leftHand);
        else if (weapon.WeaponSO.Type == AttackType.melee || weapon.WeaponSO.Type == AttackType.melee_long) _currentWeapon = Instantiate(weapon.Clone, _rightHand);
        if (_currentWeapon.TryGetComponent<Rigidbody>(out var rb)) rb.isKinematic = true;
        if (_currentWeapon.TryGetComponent<ItemController>(out var ic)) ic.Deactivate();
        _weapon = weapon.WeaponSO;
        ResetAttack();
    }

    public void SetPlayerShield(ItemSO shield)
    {
        GameObject s = null;
        if (_currentWeapon == null || _weapon.Type != AttackType.long_range) s = Instantiate(shield.Clone, _leftHand);
        else s = Instantiate(shield.Clone, _rightHand);
        if (s.TryGetComponent<Rigidbody>(out var rb)) rb.isKinematic = true;
        if (s.TryGetComponent<ItemController>(out var ic)) ic.Deactivate();
        if (TryGetComponent<PlayerController>(out PlayerController player)) player.ChangeDefenceType(DefenseType.Shield);
    }

    public void SetPlayerQuiver(ItemSO quiver)
    {
        GameObject q = null;
        q = Instantiate(quiver.Clone, _spine);
        if (q.TryGetComponent<Rigidbody>(out var rb)) rb.isKinematic = true;
        if (q.TryGetComponent<ItemController>(out var ic)) ic.Deactivate();
    }

    public void SetPlayerHelmet(ItemSO helmet)
    {
        GameObject h = null;
        h = Instantiate(helmet.Clone, _head);
        if (h.TryGetComponent<Rigidbody>(out var rb)) rb.isKinematic = true;
        if (h.TryGetComponent<ItemController>(out var ic)) ic.Deactivate();
        if (TryGetComponent<HPController>(out HPController hp)) hp.SetHelmet();
    }
    public void Attack()
    {
        if (IsAttacking == false)
        {
            if (_weapon == null) return;
            else
            {
                if (_weapon.Type == AttackType.melee || _weapon.Type == AttackType.melee_long)
                {
                    IsAttacking = true;
                    _animator.SetTrigger("Combo");
                }
                if (_weapon.Type == AttackType.long_range && _arrowAmount > 0)
                {
                    IsAttacking = true;
                    _animator.SetTrigger("BowShot");
                    Invoke("ArrowSpawn", .5f);
                }
                Invoke("ResetAttack", _weapon.Cooldown);
            }
        }
    }


    public void AlternativeAttack()
    {
        if (IsAttacking == false)
        {
            if (_weapon == null) return;
            else
            {
                if (_weapon.Type == AttackType.melee_long)
                {
                    IsAttacking = true;
                    _animator.SetTrigger("AxeThrow");
                    Invoke("CheckWeaponMesh", 5f);
                }
            }
        }
    }

    private void CheckWeaponMesh()
    {
        if (_currentWeapon.GetComponent<MeshRenderer>().enabled == false)
            OnAxeCatch();
    }


    public void OnAxeThrow()
    {
        _currentWeapon.GetComponent<MeshRenderer>().enabled = false;
        _axe = Instantiate(_weapon.AxeClone, _axePoint.position, _axePoint.rotation);
        Rigidbody rb = _axe.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * _weapon.ForceZ + transform.up * _weapon.ForceY, ForceMode.Impulse);
    }

    public void OnAxeCatch()
    {
        Destroy(_axe);
        _currentWeapon.GetComponent<MeshRenderer>().enabled = true;
        ResetAttack();
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
