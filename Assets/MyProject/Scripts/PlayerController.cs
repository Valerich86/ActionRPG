using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public interface IMortal
{
    public void Dying();
}

public enum DefenseType { CanRoll, CanBlock}

public class PlayerController : MonoBehaviour, IMortal
{
    public event Action OnPlayerDying;

    [SerializeField] private Transform[] _vulnerableParts;

    private CharacterController _controller;
    private HPController _hpController;
    private Animator _animator;
    private AttackController _attackController;
    private float _jumpForce = 0;
    private bool _isDead = false;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _attackController = gameObject.GetComponent<AttackController>();
        _attackController.SetWeapon(StaticData.PlayerRole.Weapon);
        _hpController = gameObject.GetComponent<HPController>();
        _hpController.SetStartHealth(StaticData.PlayerRole.MaxHP);
    }


    void Update()
    {
        Movement();
        Attack();
        Defense();
        Jump();
    }

    private void Defense()
    {
        if (Input.GetKeyDown(KeyCode.E) && StaticData.PlayerRole.Defense == DefenseType.CanBlock)
        {
            _hpController.SetBlock();
        }
        else if (Input.GetKeyUp(KeyCode.E) && StaticData.PlayerRole.Defense == DefenseType.CanBlock)
        {
            _hpController.ResetBlock();
        }
        if (Input.GetMouseButton(1) && StaticData.PlayerRole.Defense == DefenseType.CanRoll)
        {
            _animator.SetTrigger("RollBack");
        }
    }

    void Movement()
    {
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = transform.TransformDirection(0, _jumpForce, vertical);
        if (!_isDead)
        {
            if (Input.GetKey(KeyCode.A)) transform.Rotate(Vector3.up, -StaticData.PlayerRole.RotationSpeed * Time.deltaTime);
            if (Input.GetKey(KeyCode.D)) transform.Rotate(Vector3.up, StaticData.PlayerRole.RotationSpeed * Time.deltaTime);

            _controller.Move(direction * StaticData.PlayerRole.Speed * Time.deltaTime);
            _animator.SetFloat("Speed", vertical);
        }
    }

    void Attack()
    {
        if (Input.GetMouseButtonDown(0) && !_isDead) _attackController.Attack(StaticData.PlayerRole.Weapon.Type);
    }


    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jumpForce = StaticData.PlayerRole.MaxJumpForce;
            _animator.SetTrigger("Jump");
            Invoke("ReturnDirectionY", 1f);
        }
    }

    private void ReturnDirectionY() => _jumpForce = 0;
    

    public Transform SetCurrentVulnerablePart()
    {
        int choice = UnityEngine.Random.Range(0, _vulnerableParts.Length);
        return _vulnerableParts[choice];
    }
    public void Dying()
    {
        _isDead = true;
        OnPlayerDying?.Invoke();
    }
}
