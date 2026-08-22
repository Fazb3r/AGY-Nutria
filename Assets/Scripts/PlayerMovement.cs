using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int InputX = Animator.StringToHash("InputX");
    private static readonly int InputY = Animator.StringToHash("InputY");
    private static readonly int LastInputX = Animator.StringToHash("LastInputX");
    private static readonly int LastInputY = Animator.StringToHash("LastInputY");


    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D _rb;
    private Vector2 _moveInput;
    private Animator _animator;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        _rb.linearVelocity = _moveInput * moveSpeed;
        
    }

    public void Move(InputAction.CallbackContext context)
    {
        _animator.SetBool(IsWalking, true);

        if (context.canceled)
        {
            _animator.SetBool(IsWalking, false);
            _animator.SetFloat(LastInputX, _moveInput.x);
            _animator.SetFloat(LastInputY, _moveInput.y);
        } 
        _moveInput = context.ReadValue<Vector2>(); 
        _animator.SetFloat(InputX, _moveInput.x);
        _animator.SetFloat(InputY, _moveInput.y);
    }   

}