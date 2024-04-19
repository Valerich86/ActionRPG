using System;
using UnityEngine;

public class HPController : MonoBehaviour
{
    public event Action<HPController> OnHealthChanged;
    public float MaxHP => _maxhp;
    public float CurrentHP { get; private set; }

    [SerializeField] private float _maxhp;
    [SerializeField] private GameObject _bloodClone;
    [SerializeField] private GameObject _bloodClone2;
    [SerializeField] private Transform _bloodPoint;

    private Animator _animator;
    private void Start()
    {
        CurrentHP = _maxhp;
        _animator = gameObject.GetComponent<Animator>();
    }
    public void TakeDamage(float damage)
    {
        if (TryGetComponent<EnemyController>(out EnemyController enemy))
        {
            int variant = UnityEngine.Random.Range(0, 3);
            if (variant == 0 && enemy.SetRollAbility())
            {
                _animator.SetTrigger("RollBack");
            }
            else if (variant == 0 && !enemy.SetRollAbility())
            {
                _animator.SetBool("Blocking", true);
                Invoke("ResetBlock", 2f);
            }
            else SetDamage(damage);
        }
        else SetDamage(damage);
    }

    private void SetDamage(float damage)
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

    private void ResetBlock() => _animator.SetBool("Blocking", false);

}
