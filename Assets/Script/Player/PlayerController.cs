using NUnit.Framework.Interfaces;
using Unity.Cinemachine;
using Unity.IO.LowLevel.Unsafe;
using Unity.Mathematics;
using System.Collections;
using System.Collections.Generic;
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


    

    //Equip-Unequip parameters
    [SerializeField] public GameObject Sword;  
    [SerializeField] public GameObject SwordOnHand; 
     public bool isEquipping;
    public bool isEquipped;

    // States
    [HideInInspector] public PlayerIdleState idleState;
    [HideInInspector] public PlayerMoveState moveState;
    [HideInInspector] public PlayerJumpState jumpState;
    [HideInInspector] public PlayerFallState fallState;
    [HideInInspector] public RunState runState;
    [HideInInspector] public PlayerAttackState attackState;

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
        attackState = new PlayerAttackState(this);
    }

    void Start()
    {
        controller.Move(Vector3.up * 0.1f);

        ChangeState(idleState);
    }


    void Update()
    {
       
    _moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

    currentState.Update();

    
    Vector3 forward = _cameraTransform.forward;
    Vector3 right = _cameraTransform.right;
    forward.y = 0;
    right.y = 0;

    
    Vector3 moveDirection = (forward * _moveInput.y + right * _moveInput.x).normalized;

    
    
    if(_shouldFaceMoveDirection && moveDirection.sqrMagnitude > 0.001f)
    {
        Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
    }

   
     if (Input.GetMouseButtonDown(0) && controller.isGrounded)
{
    
   
}



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


public void Jump()
{
    
    velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
    animator.SetTrigger("Jump"); 
}

private void Equip()
    {
        if (Input.GetKeyDown(KeyCode.R) && animator.GetBool("Grounded"))
        {
            isEquipping = true;
            animator.SetTrigger("Equip");
        }
    }

     public void ActiveWeapon()
    {
        if (!isEquipped)
        {
            Sword.SetActive(true);
            SwordOnHand.SetActive(false);
            isEquipped = !isEquipped;
        }
        else
        {
            Sword.SetActive(false);
            SwordOnHand.SetActive(true);
            isEquipped = !isEquipped;
        }
    }
  public void Equipped()
    {
        isEquipping = false;
    }




  
}
