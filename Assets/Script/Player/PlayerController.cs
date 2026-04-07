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
    [HideInInspector] public GameInput playerInput;
 
 
    [Header("Melee Combat")]
    public Transform attackPoint;    // Điểm chính giữa vùng chém
    public float attackRange = 1.2f; // Độ rộng của cú chém
    public LayerMask enemyLayer;     // Chỉ chọn Layer "Enemy" để đánh
    public float meleeDamage = 15f;  // Sát thương mỗi đòn
 
    //Equip-Unequip parameters
    [SerializeField] public GameObject Sword;  
    [SerializeField] public GameObject SwordOnHand; 
     public bool isEquipping;
    public bool isEquipped;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    public AudioClip[] attackSounds; 
    public AudioClip footstepSound; 
    public AudioClip jumpSound;  
    public AudioClip drawSwordSound;   
    public AudioClip sheathSwordSound;   
 
    // States
    [HideInInspector] public PlayerIdleState idleState;
    [HideInInspector] public PlayerMoveState moveState;
    [HideInInspector] public PlayerJumpState jumpState;
    [HideInInspector] public PlayerFallState fallState;
    [HideInInspector] public PlayerRunState runState;
    [HideInInspector] public PlayerAttackState attackState;
    [HideInInspector] public PlayerCombatState combatState;
 
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
        runState = new PlayerRunState(this);
        jumpState = new PlayerJumpState(this);
        attackState = new PlayerAttackState(this);
        combatState = new PlayerCombatState(this);
    }
 
    void Start()
    {
        controller.Move(Vector3.up * 0.1f);
 
        ChangeState(idleState);

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
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
 
    
    
    if(_shouldFaceMoveDirection && moveDirection.sqrMagnitude > 0.001f && !IsAttacking())
    {
        Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
    }
 
   
     if (Input.GetMouseButtonDown(0) && controller.isGrounded)
    {
    
    
    }
    if (GameStateManager.IsUIOpen)
    {
        // Dừng animation di chuyển
        animator.SetFloat("MoveX", 0f);
        animator.SetFloat("MoveY", 0f); 
        animator.SetBool("IsRunning", false);
        return; 
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
 
     // Hàm này sẽ được gọi bởi Animation Event
    public void ToggleWeaponVisibility(int isCombat)
    {
        if (isCombat == 0) // Đang rút kiếm
        {
            SwordOnHand.SetActive(true);
            Sword.SetActive(false);
        }
        else // Đang cất kiếm
        {
            SwordOnHand.SetActive(false);
            Sword.SetActive(true);
        }
    }
    public void Equipped()
    {
        isEquipping = false;
    }
 
 
    public void SetCombatLayerWeight(float weight)
    {
    // Giả sử Combat Layer là Layer thứ 1 (Base Layer là 0)
    animator.SetLayerWeight(1, weight);
    }
 
    public bool IsAttacking()
    {
    
    return animator.GetCurrentAnimatorStateInfo(1).IsTag("Attack");
    }
 
  
  public void DealMeleeDamage()
{
    if (attackPoint == null) return;

    // Quét tất cả các Collider trong vùng hình cầu
    Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);

    foreach (Collider enemy in hitEnemies)
    {
        // Lấy thành phần nhận sát thương trên quái
        IDamageable damageable = enemy.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(meleeDamage);
        }
    }
}

// Vẽ vùng chém trong cửa sổ Scene để dễ căn chỉnh
private void OnDrawGizmosSelected()
{
    if (attackPoint == null) return;
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(attackPoint.position, attackRange);
}


public void PlayAttackSound(int index)
    {
        if (attackSounds.Length > index && attackSounds[index] != null)
        {
            // Dùng PlayOneShot để âm thanh không bị ngắt nếu có tiếng khác đè lên
            audioSource.PlayOneShot(attackSounds[index]);
        }
    }

    public void PlayFootstep(float volume)
    {
        if (footstepSound != null)
        {
            // Thay đổi pitch nhẹ để tiếng bước chân nghe tự nhiên hơn
            audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(footstepSound, volume);
        }
    }

    public void PlayJumpSound()
    {
        if (jumpSound != null)
        {
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(jumpSound);
        }
    }
    public void PlayDrawSound()
    {
    if (audioSource != null && drawSwordSound != null)
    {
        audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f); // Tránh bị lặp âm đơn điệu
        audioSource.PlayOneShot(drawSwordSound);
    }
    }

// Hàm này để gọi khi cất kiếm
    public void PlaySheathSound()
    {
    if (audioSource != null && sheathSwordSound != null)
    {
        audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(sheathSwordSound);
    }
    }
}


 


