using NUnit.Framework.Interfaces;
using Unity.Cinemachine;
using Unity.IO.LowLevel.Unsafe;
using Unity.Mathematics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // ── Movement ──────────────────────────────────────────────────────────────
    [Header("Movement")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private bool _shouldFaceMoveDirection = false;
 
    public float walkSpeed = 2.5f;
    public float runSpeed  = 5f;
    public float gravity   = -20f;
    public float jumpForce = 1.5f;
 
    // ── References ────────────────────────────────────────────────────────────
    [Header("References")]
    public Animator animator;
 
    [HideInInspector] public CharacterController controller;
    [HideInInspector] public Vector3 velocity;
 
    // ── Melee Combat ──────────────────────────────────────────────────────────
    [Header("Melee Combat")]
    public Transform attackPoint;
    public float     attackRange = 1.2f;
    public LayerMask enemyLayer;
    public float     meleeDamage = 15f; // Tổng damage = meleeDamage + PlayerStats.Attack
 
    [SerializeField] public GameObject Sword;
    [SerializeField] public GameObject SwordOnHand;
    public bool isEquipping;
    public bool isEquipped;
 
    // ── Audio ─────────────────────────────────────────────────────────────────
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    public AudioClip[] attackSounds;
    public AudioClip   footstepSound;
    public AudioClip   jumpSound;
    public AudioClip   drawSwordSound;
    public AudioClip   sheathSwordSound;
 
    // ── States ────────────────────────────────────────────────────────────────
    [HideInInspector] public PlayerIdleState   idleState;
    [HideInInspector] public PlayerMoveState   moveState;
    [HideInInspector] public PlayerJumpState   jumpState;
    [HideInInspector] public PlayerFallState   fallState;
    [HideInInspector] public PlayerRunState    runState;
    [HideInInspector] public PlayerCombatState combatState;
 
    public float rotateSpeed    = 10f;
    public float aimRotateSpeed = 15f;
    public bool  isAiming;

    public string CollectedItemGuid  ;
 
    private PlayerState  _currentState;
    private WeaponHolder _weaponHolder;
 
    // ── Lifecycle ─────────────────────────────────────────────────────────────
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator   = GetComponentInChildren<Animator>();
 
        idleState   = new PlayerIdleState(this);
        moveState   = new PlayerMoveState(this);
        runState    = new PlayerRunState(this);
        jumpState   = new PlayerJumpState(this);
        fallState   = new PlayerFallState(this);
        combatState = new PlayerCombatState(this);
    }
 
    void Start()
    {
        _weaponHolder = GetComponentInChildren<WeaponHolder>();
 
        audioSource.playOnAwake  = false;
        audioSource.spatialBlend = 0f;
 
        ChangeState(idleState);
    }
 
    void Update()
    {
        // Khi UI mở: dừng animation và bỏ qua mọi input
        if (GameStateManager.IsUIOpen)
        {
            animator.SetFloat("MoveX",    0f);
            animator.SetFloat("MoveY",    0f);
            animator.SetFloat("Speed",    0f);
            animator.SetBool("IsRunning", false);
            return;
        }
 
        _currentState.Update();
 
       
        ApplyGravity();
        controller.Move(velocity * Time.deltaTime);
 
        // Xoay theo hướng di chuyển 
        if (_shouldFaceMoveDirection && !IsAttacking())
        {
            Vector3 moveDir = GetMoveInput();
            if (moveDir.sqrMagnitude > 0.001f)
            {
                Quaternion toRotation = Quaternion.LookRotation(moveDir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotateSpeed * Time.deltaTime);
            }
        }
    }
 
    // ── State Machine ─────────────────────────────────────────────────────────
    public void ChangeState(PlayerState newState)
    {
        if (newState == null) { Debug.LogError("[PlayerController] ChangeState: newState is NULL"); return; }
 
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }
 
    // ── Input ─────────────────────────────────────────────────────────────────
    public Vector3 GetMoveInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
 
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight   = Camera.main.transform.right;
        camForward.y = 0f;
        camRight.y   = 0f;
 
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
 
    // ── Combat ────────────────────────────────────────────────────────────────
    public void ToggleWeaponVisibility(int isCombat)
    {
        bool inCombat = isCombat == 0;
        SwordOnHand.SetActive(inCombat);
        Sword.SetActive(!inCombat);
    }
 
    public void DealMeleeDamage()
    {
        if (attackPoint == null) return;
 
        float equipBonus  = PlayerStats.Instance != null ? PlayerStats.Instance.Attack : 0f;
        float totalDamage = meleeDamage + equipBonus;
 
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);
        foreach (Collider enemy in hitEnemies)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(totalDamage);
                Debug.Log($"[Attack] Gây {totalDamage} damage (base: {meleeDamage} + equip: {equipBonus})");
            }
        }
    }
 
    public bool IsAttacking()                      => animator.GetCurrentAnimatorStateInfo(1).IsTag("Attack");
    public void Equipped()                         => isEquipping = false;
    public void SetCombatLayerWeight(float weight) => animator.SetLayerWeight(1, weight);
 
    // ── Audio ─────────────────────────────────────────────────────────────────
    public void PlayAttackSound(int index)
    {
        if (index < attackSounds.Length && attackSounds[index] != null)
            audioSource.PlayOneShot(attackSounds[index]);
    }
 
    public void PlayFootstep(float volume)
    {
        if (footstepSound == null) return;
        audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(footstepSound, volume);
    }
 
    public void PlayJumpSound()
    {
        if (jumpSound == null) return;
        audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(jumpSound);
    }
 
    public void PlayDrawSound()
    {
        if (drawSwordSound == null) return;
        audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(drawSwordSound);
    }
 
    public void PlaySheathSound()
    {
        if (sheathSwordSound == null) return;
        audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(sheathSwordSound);
    }
 
    // ── Gizmos ────────────────────────────────────────────────────────────────
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}


 


