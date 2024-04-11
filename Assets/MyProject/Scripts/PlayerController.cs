
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _speed = 5;
    [SerializeField] private float _gravity = 5;
    [SerializeField] private Transform _rHand;
    [SerializeField] private GameObject _swordClone;

    private GameObject _currentWeapon;
    private CharacterController _controller;
    private Animator _animator;
    private AttackController _attackController;
    private Camera _camera;
    private Vector3 _input;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _camera = Camera.main;
        _currentWeapon = Instantiate(_swordClone, _rHand);
        _attackController = gameObject.GetComponent<AttackController>();
        _attackController.Weapon = _currentWeapon.GetComponent<WeaponController>();
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        _input = new Vector3(horizontal, 0, vertical);

        Movement();
    }

    void Movement()
    {
        Vector3 moveDirection = _camera.transform.TransformDirection(_input);
        moveDirection.y = 0;
        moveDirection.Normalize();
        //moveDirection += Physics.gravity;
        transform.forward = moveDirection;
        if (!_attackController.IsAttacking) _controller.Move(_input * _speed * Time.deltaTime);
        _animator.SetFloat("Speed", _controller.velocity.magnitude);
    }
    
}
