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
    public float runSpeed  = 5f;
 
    [Header("References")]
    public Animator animator;
 
    [HideInInspector] public CharacterController controller;
    private Vector2 _moveInput;
    private Vector3 _velocity;
    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public GameInput playerInput;
 
    [Header("Melee Combat")]
    public Transform attackPoint;
    public float     attackRange  = 1.2f;
    public LayerMask enemyLayer;
    // meleeDamage là damage gốc của đòn đánh
    // Tổng damage = meleeDamage + PlayerStats.Attack (từ equipment)
    public float meleeDamage = 15f;
 
    [SerializeField] public GameObject Sword;
    [SerializeField] public GameObject SwordOnHand;
    public bool isEquipping;
    public bool isEquipped;
 
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    public AudioClip[] attackSounds;
    public AudioClip   footstepSound;
    public AudioClip   jumpSound;
    public AudioClip   drawSwordSound;
    public AudioClip   sheathSwordSound;
 
    [HideInInspector] public PlayerIdleState   idleState;
    [HideInInspector] public PlayerMoveState   moveState;
    [HideInInspector] public PlayerJumpState   jumpState;
    [HideInInspector] public PlayerFallState   fallState;
    [HideInInspector] public PlayerRunState    runState;
    [HideInInspector] public PlayerAttackState attackState;
    [HideInInspector] public PlayerCombatState combatState;
 
    public float _horizontalInput;
    public float _verticalInput;
    public Vector3 _moveDirection;
    public float _moveX;
    public float _moveY;
 
    public float rotateSpeed    = 10f;
    public float aimRotateSpeed = 15f;
    public string CollectedItemGuid;
    public bool isAiming;
 
    PlayerState currentState;
 
    public float HorizontalInput => _horizontalInput;
    public float VerticalInput   => _verticalInput;
    public Vector3 MoveDirection => _moveDirection;
 
    private WeaponHolder weaponHolder;
 
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator   = GetComponentInChildren<Animator>();
 
        idleState   = new PlayerIdleState(this);
        moveState   = new PlayerMoveState(this);
        runState    = new PlayerRunState(this);
        jumpState   = new PlayerJumpState(this);
        attackState = new PlayerAttackState(this);
        combatState = new PlayerCombatState(this);
    }
 
    void Start()
    {
        weaponHolder = GetComponentInChildren<WeaponHolder>();
        controller.Move(Vector3.up * 0.1f);
        ChangeState(idleState);
        audioSource.playOnAwake  = false;
        audioSource.spatialBlend = 0f;
    }
 
    void Update()
    {
        
        
        _moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        currentState.Update();
        ApplyGravity();
        
        controller.Move(velocity * Time.deltaTime);
        
 
        Vector3 forward = _cameraTransform.forward;
        Vector3 right   = _cameraTransform.right;
        forward.y = 0;
        right.y   = 0;
 
        Vector3 moveDirection = (forward * _moveInput.y + right * _moveInput.x).normalized;
 
        if (_shouldFaceMoveDirection && moveDirection.sqrMagnitude > 0.001f && !IsAttacking())
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
        }
 
        if (GameStateManager.IsUIOpen)
        {
            animator.SetFloat("MoveX",    0f);
            animator.SetFloat("MoveY",    0f);
            animator.SetBool("IsRunning", false);
            return;
        }
    }
 
    public void ChangeState(PlayerState newState)
    {
        if (newState == null) { Debug.LogError("State is NULL"); return; }
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }
 
    public Vector3 GetMoveInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
 
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight   = Camera.main.transform.right;
        camForward.y = 0;
        camRight.y   = 0;
 
        return (camForward.normalized * v + camRight.normalized * h).normalized;
    }
 
    public bool HasMoveInput() => GetMoveInput().magnitude > 0.1f;
 
    public void ApplyGravity()
    {
        if (controller.isGrounded)
        {
            if (velocity.y < -2f) velocity.y = -2f;
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
 
    public void ToggleWeaponVisibility(int isCombat)
    {
        if (isCombat == 0) { SwordOnHand.SetActive(true);  Sword.SetActive(false); }
        else               { SwordOnHand.SetActive(false); Sword.SetActive(true);  }
    }
 
    public void Equipped()      => isEquipping = false;
    public void SetCombatLayerWeight(float weight) => animator.SetLayerWeight(1, weight);
    public bool IsAttacking()   => animator.GetCurrentAnimatorStateInfo(1).IsTag("Attack");
 
    // ── DAMAGE ──────────────────────────────────────────────────────────────
    public void DealMeleeDamage()
    {
        if (attackPoint == null) return;
 
        // Tổng sát thương = base melee + Attack stat từ equipment/level
        float equipBonus  = PlayerStats.Instance != null ? PlayerStats.Instance.Attack : 0f;
        float totalDamage = meleeDamage + equipBonus;
 
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);
        foreach (Collider enemy in hitEnemies)
        {
            var damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(totalDamage);
                Debug.Log($"[Attack] Gây {totalDamage} damage (base:{meleeDamage} + equip:{equipBonus})");
            }
        }
    }
    // ────────────────────────────────────────────────────────────────────────
 
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
 
    public void PlayAttackSound(int index)
    {
        if (attackSounds.Length > index && attackSounds[index] != null)
            audioSource.PlayOneShot(attackSounds[index]);
    }
 
    public void PlayFootstep(float volume)
    {
        if (footstepSound != null)
        {
            audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(footstepSound, volume);
        }
    }
 
    public void PlayJumpSound()
    {
        if (jumpSound != null) { audioSource.pitch = 1f; audioSource.PlayOneShot(jumpSound); }
    }
 
    public void PlayDrawSound()
    {
        if (audioSource != null && drawSwordSound != null)
        {
            audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(drawSwordSound);
        }
    }
 
    public void PlaySheathSound()
    {
        if (audioSource != null && sheathSwordSound != null)
        {
            audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(sheathSwordSound);
        }
    }
}


 


