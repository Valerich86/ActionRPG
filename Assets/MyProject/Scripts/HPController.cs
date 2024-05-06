using System;
using UnityEngine;
using static UnityEditor.Progress;

public class HPController : MonoBehaviour
{
    public event Action<HPController> OnHealthChanged;
    public float MaxHP => _maxhp;
    public float CurrentHP { get; private set; }

    public Transform[] DamagePoints;
    [SerializeField] private GameObject _bloodClone;
    [SerializeField] private GameObject _bloodClone2;
    [SerializeField] private Transform _bloodPoint;

    private float _maxhp;
    private Animator _animator;
    private DefenseType _defenseType;
    private bool _inBlock;
    private bool _hasHelmet = false;
    private void Start() => _animator = gameObject.GetComponent<Animator>();

    public void SetStartHealth(float startHealth)
    {
        _maxhp = startHealth;
        CurrentHP = _maxhp;
    }
    public void SetHelmet() => _hasHelmet = true;

    public void SetDefenseType(DefenseType defType) => _defenseType = defType;
    
    public void TakeDamage(float damage)
    {
        if (TryGetComponent<EnemyController>(out EnemyController enemy))
        {
            int variant = UnityEngine.Random.Range(0, 3);
            if (variant == 0 && _defenseType == DefenseType.Default)
            {
                _animator.SetTrigger("RollBack");
                return;
            }
            else if (variant == 0 && _defenseType == DefenseType.Shield)
            {
                SetBlock();
                Invoke("ResetBlock", 1.8f);
                return;
            }
        }
        else if (TryGetComponent<PlayerController>(out PlayerController player))
        {
            if (_inBlock) return;
            if (_hasHelmet) damage -= damage * 0.3f;
        }
        SetDamage(damage);
    }

    public void SetDamage(float damage)
    {
        CurrentHP -= damage;
        OnHealthChanged?.Invoke(this);
        GameObject blood = Instantiate(_bloodClone, _bloodPoint);
        Destroy(blood, 1f);
        if (CurrentHP <= 0)
        {
            _animator.SetTrigger("Die");
            blood = Instantiate(_bloodClone2, _bloodPoint);
            Destroy(blood, 10f);
            if (TryGetComponent<EnemyController>(out EnemyController enemy))
            {
                enemy.Dying();
            }
            else if (TryGetComponent<PlayerController>(out PlayerController player))
            {
                player.Dying();
            }
        }
        else
        {
            _animator.SetTrigger("GetHit");
        }
    }

    public void SetBlock()
    {
        _animator.SetBool("Blocking", true);
        _inBlock = true;
    }
    public void ResetBlock()
    {
        _animator.SetBool("Blocking", false);
        _inBlock = false;
    }


    public void Healing(int healPercent)
    {
        CurrentHP += MaxHP * healPercent / 100;
        if(CurrentHP > MaxHP) CurrentHP = MaxHP;
        OnHealthChanged?.Invoke(this);
    }

}
