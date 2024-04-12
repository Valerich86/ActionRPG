using System.Collections;
using UnityEngine;


public interface IMortal
{
    public void Dying();
}

public class PlayerController : MonoBehaviour, IMortal
{
    [SerializeField] private float _speed = 5;
    [SerializeField] private float _gravity = 5;
    [SerializeField] private float _rotationSpeed = 3;
    [SerializeField] private float _maxJump = 100;
    [SerializeField] private Transform _rHand;
    [SerializeField] private GameObject _swordClone;

    private GameObject _currentWeapon;
    private CharacterController _controller;
    private Animator _animator;
    private AttackController _attackController;
    //private Camera _camera;
    //private Vector3 _input;
    private float _jumpForce;


    private void OnEnable()
    {
        EventManager.OnDead += Dying;
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        //_camera = Camera.main;
        _currentWeapon = Instantiate(_swordClone, _rHand);
        _attackController = gameObject.GetComponent<AttackController>();
        _attackController.SetWeapon(_currentWeapon.GetComponent<WeaponController>());
    }

    private void OnDisable()
    {
        EventManager.OnDead -= Dying;
    }
    
    void Update()
    {
        //float horizontal = Input.GetAxis("Horizontal");
        //float vertical = Input.GetAxis("Vertical");

        //_input = new Vector3(horizontal, 0, vertical);

        Movement();
        Attack();
        Jump();
    }

    void Movement()
    {
        //Vector3 moveDirection = _camera.transform.TransformDirection(_input);
        //moveDirection.y = 0;
        //moveDirection.Normalize();
        ////moveDirection += Physics.gravity;
        //transform.forward = moveDirection;
        //if (!_attackController.IsAttacking) _controller.Move(_input * _speed * Time.deltaTime);
        //_animator.SetFloat("Speed", _controller.velocity.magnitude);
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = transform.TransformDirection(0, _jumpForce, vertical).normalized;
        direction.y -= _gravity * Time.deltaTime;

        if (Input.GetKey(KeyCode.A)) transform.Rotate(Vector3.up, -_rotationSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.D)) transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);

        if (!_attackController.IsAttacking) _controller.Move(direction * _speed * Time.deltaTime);
        _animator.SetFloat("Speed", vertical);
    }

    void Attack()
    {
        if (Input.GetMouseButtonDown(0)) _attackController.Attack();
    }


    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _controller.isGrounded)
        {
            _jumpForce = _maxJump;
            _animator.SetTrigger("Jump");
            Invoke("ReturnDirectionY", 1f);
        }
    }

    private void ReturnDirectionY() => _jumpForce = 0;
    

    public void Dying()
    {

    }
}
