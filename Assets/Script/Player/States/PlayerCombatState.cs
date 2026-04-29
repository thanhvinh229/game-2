using System;
using UnityEngine;

public class PlayerCombatState : PlayerMoveState
{
    // ── Config ────────────────────────────────────────────────────────────────
    private const float DAMP_TIME       = 0.1f;
    private const float ROTATION_SPEED  = 10f;
    private const int   MAX_COMBO       = 3;
    private const float COMBO_RESET_TIME  = 0.5f;
    private const float COMBO_COOLDOWN    = 0.5f;
    private const float BUFFER_WINDOW    = 0.25f;
 
    public float autoSheatheTime = 5f; // Giây không hành động thì tự về idle
 
    // ── Combo ─────────────────────────────────────────────────────────────────
    private int   _comboStep       = 0;
    private float _comboTimer      = 0f;
    private bool  _comboCoolingDown = false;
 
    // ── Input Buffer ──────────────────────────────────────────────────────────
    private bool  _attackBuffered = false;
    private float _bufferTimer    = 0f;
 
    // ── Rotation ──────────────────────────────────────────────────────────────
    private bool _isFacingCamera = false;
 
    // ── Auto-sheathe timer ────────────────────────────────────────────────────
    private float _lastCombatActionTime;
 
    public PlayerCombatState(PlayerController player) : base(player) { }
 
    // ── State Lifecycle ───────────────────────────────────────────────────────
    public override void Enter()
    {
        _lastCombatActionTime = Time.time;
    }
 
    public override void Update()
    {
        Vector3 move      = player.GetMoveInput();
        bool isMoving     = move.magnitude > 0.1f;
        bool isAttacking  = Input.GetMouseButtonDown(0);
        bool isJumping    = Input.GetButtonDown("Jump");
 
        if (Input.GetKey(KeyCode.LeftShift) && player.HasMoveInput())
        {
            player.ChangeState(player.runState);
            return;
        }
 
        if (isJumping && player.controller.isGrounded)
        {
            player.ChangeState(player.jumpState);
            return;
        }
 
        // Nhận attack input vào buffer
        if (isAttacking)
        {
            _attackBuffered       = true;
            _bufferTimer          = BUFFER_WINDOW;
            _lastCombatActionTime = Time.time;
        }
 
        UpdateComboTimer();
        TryConsumeBuffer();
 
        if (isMoving || isJumping)
            _lastCombatActionTime = Time.time;
 
        // Tự động về idle sau autoSheatheTime giây không hành động
        if (Time.time - _lastCombatActionTime > autoSheatheTime)
        {
            ExitCombatState();
            return;
        }
 
        HandleRotation(move, isAttacking);
 
        Vector3 movement  = move * player.walkSpeed;
        player.velocity.x = movement.x;
        player.velocity.z = movement.z;
 
        UpdateCombatAnimator(move, 0.5f);
    }
 
    // ── Buffer ────────────────────────────────────────────────────────────────
    private void TryConsumeBuffer()
    {
        if (!_attackBuffered) return;
 
        _bufferTimer -= Time.deltaTime;
        if (_bufferTimer <= 0f)
        {
            _attackBuffered = false;
            return;
        }
 
        if (_comboCoolingDown) return;
 
        // Chờ Animator kết thúc transition
        if (player.animator.IsInTransition(1)) return;
 
        // Chờ animation attack qua 30% trước khi nhận đòn tiếp
        AnimatorStateInfo stateInfo = player.animator.GetCurrentAnimatorStateInfo(1);
        if (stateInfo.IsTag("Attack") && stateInfo.normalizedTime < 0.3f) return;
 
        _attackBuffered = false;
        _bufferTimer    = 0f;
        HandleComboAttack();
    }
 
    // ── Combo ─────────────────────────────────────────────────────────────────
    private void HandleComboAttack()
    {
        if (_comboCoolingDown) return;
 
        _lastCombatActionTime = Time.time;
        _comboStep++;
        _comboTimer = COMBO_RESET_TIME;
 
        ResetAllAttackTriggers();
        player.animator.applyRootMotion = true;
        player.animator.SetTrigger("attack" + _comboStep);
 
        if (_comboStep >= MAX_COMBO)
        {
            _comboCoolingDown = true;
            _comboTimer       = COMBO_COOLDOWN;
            _comboStep        = 0;
            player.StartCoroutine(DisableRootMotionAfterDelay(1.5f));
        }
    }
 
    private void UpdateComboTimer()
    {
        if (_comboTimer <= 0f) return;
 
        _comboTimer -= Time.deltaTime;
        if (_comboTimer <= 0f)
        {
            _comboTimer       = 0f;
            _comboCoolingDown = false;
            _comboStep        = 0;
            ResetAllAttackTriggers();
        }
    }
 
    private void ResetAllAttackTriggers()
    {
        for (int i = 1; i <= MAX_COMBO; i++)
            player.animator.ResetTrigger("attack" + i);
    }
 
    // ── Rotation ──────────────────────────────────────────────────────────────
    private void HandleRotation(Vector3 move, bool isAttacking)
    {
        if (isAttacking) _isFacingCamera = true;
 
        if (_isFacingCamera)
        {
            Vector3 cameraForward = Camera.main.transform.forward;
            cameraForward.y = 0f;
 
            if (cameraForward.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
                player.transform.rotation = Quaternion.Slerp(
                    player.transform.rotation,
                    targetRotation,
                    Time.deltaTime * ROTATION_SPEED
                );
 
                if (Quaternion.Angle(player.transform.rotation, targetRotation) < 2f)
                    _isFacingCamera = false;
            }
        }
        else if (move.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            player.transform.rotation = Quaternion.Slerp(
                player.transform.rotation,
                targetRotation,
                Time.deltaTime * ROTATION_SPEED
            );
        }
    }
 
    // ── Animator ──────────────────────────────────────────────────────────────
    private void UpdateCombatAnimator(Vector3 move, float speedMultiplier)
    {
        Vector3 local = player.transform.InverseTransformDirection(move);
        player.animator.SetFloat("MoveX", local.x * speedMultiplier, DAMP_TIME, Time.deltaTime);
        player.animator.SetFloat("MoveY", local.z * speedMultiplier, DAMP_TIME, Time.deltaTime);
        player.animator.SetFloat("Speed", move.magnitude * speedMultiplier, DAMP_TIME, Time.deltaTime);
    }
 
    // ── Public API ────────────────────────────────────────────────────────────
    /// <summary>Gọi từ bên ngoài (ví dụ: PlayerSkills) để vào combat.</summary>
    public void EnterCombatState()
    {
        _lastCombatActionTime = Time.time;
        player.animator.SetBool("IsCombat", true);
        player.isEquipped = true;
        player.ChangeState(player.combatState);
    }
 
    /// <summary>Thoát combat, cất kiếm và về Idle.</summary>
    public void ExitCombatState()
    {
        _comboCoolingDown = false;
        _comboStep        = 0;
        _attackBuffered   = false;
 
        player.isEquipped = false;
        player.animator.SetBool("IsCombat", false);
        player.animator.SetTrigger("sheathWeapon");
        ResetAllAttackTriggers();
        player.ToggleWeaponVisibility(1);
 
        player.ChangeState(player.idleState);
    }
 
    // ── Coroutine ─────────────────────────────────────────────────────────────
    private System.Collections.IEnumerator DisableRootMotionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        player.animator.applyRootMotion = false;
    }
}
    
    
 
   
 
    
 
 

