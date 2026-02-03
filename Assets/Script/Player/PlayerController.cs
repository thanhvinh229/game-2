using NUnit.Framework.Interfaces;
using Unity.Cinemachine;
using Unity.IO.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private bool _shouldFaceMoveDirection = false;
    public float moveSpeed = 5f;
    public float gravity = -20f;
    public float jumpForce = 1.5f;


    public float walkSpeed = 2.5f;
    public float runSpeed = 5f;

    [Header("References")]
    
    public Animator animator;



    [HideInInspector] public CharacterController controller;
    private Vector2 _moveInput;
    private Vector3 _velocity;
    [HideInInspector] public Vector3 velocity;

    // States
    [HideInInspector] public PlayerIdleState idleState;
    [HideInInspector] public PlayerMoveState moveState;
    [HideInInspector] public PlayerJumpState jumpState;
    [HideInInspector] public PlayerFallState fallState;
    [HideInInspector] public RunState runState;

    public float _horizontalInput;
    public float _verticalInput;
    public Vector3 _moveDirection;
    public float _moveX;
    public float _moveY;
    
    public float rotateSpeed = 10f;

    
    public float aimRotateSpeed = 15f;

    public string CollectedItemGuid;

    public bool isAiming;

    PlayerState currentState;




    public float HorizontalInput => _horizontalInput;
    public float VerticalInput => _verticalInput;
    public Vector3 MoveDirection => _moveDirection;


    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        idleState = new PlayerIdleState(this);
        moveState = new PlayerMoveState(this);
        runState = new RunState(this);
        jumpState = new PlayerJumpState(this);
    }

    void Start()
    {
        controller.Move(Vector3.up * 0.1f);

        ChangeState(idleState);
    }

    void Update()
    {
        currentState.Update();

        Vector3 foward = _cameraTransform.forward;
        Vector3 right = _cameraTransform.right;

        foward.y = 0;
        right.y = 0;

        foward.Normalize();
        right.Normalize();

        Vector3 moveDirection = foward * _moveInput.y + right * _moveInput.x;
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        if(_shouldFaceMoveDirection && moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
        }

        Vector3 move =new Vector3(_moveInput.x,0,  _moveInput.y);
        controller.Move(move * moveSpeed * Time.deltaTime);

        



    }

    public void ChangeState(PlayerState newState)
    {
        if (newState == null)
        {
            Debug.LogError("State is NULL");
            return;
        }

        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    // ===== HÀM DÙNG CHUNG =====
    public Vector3 GetMoveInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0;
        camRight.y = 0;

        return (camForward.normalized * v + camRight.normalized * h).normalized;
    }

    public bool HasMoveInput()
    {
        return GetMoveInput().magnitude > 0.1f;
    }

    public void ApplyGravity()
    {
        if (controller.isGrounded)
        {
            // giữ nhân vật dính đất nhưng KHÔNG ép mạnh
            if (velocity.y < -2f)
                velocity.y = -2f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }

    
    }
    

