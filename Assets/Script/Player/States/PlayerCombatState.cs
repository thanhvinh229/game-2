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
    private float _sheathTime         = -999f;
    private const float SHEATHE_LOCK  = 1.2f;  
    private float _autoSheatheLockUntil = -999f;
    private const float AUTO_SHEATHE_DETECTION_LOCK = 4f;
    public float AutoSheatheLockUntil => _autoSheatheLockUntil;
    private bool _isFirstEnter = false;

    public void RefreshCombatTimer() => _lastCombatActionTime = Time.time;
 
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
        Debug.Log("[CombatState] Enter() called");
         player.isInCombatState = true;
        _lastCombatActionTime = Time.time;

        _attackBuffered   = false;
        _bufferTimer      = 0f;
        _comboStep        = 0;
        _comboCoolingDown = false;
        _comboTimer       = 0f;
        _isFacingCamera   = false;
        _isFirstEnter     = false;
        _lastCombatActionTime = Time.time;
    }
 
    public override void Update()
    {
        //  SHEATHE_LOCK    
        if (Time.time - _sheathTime < SHEATHE_LOCK)
        {
         player.ChangeState(player.idleState);
         return;
        }

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
    public void EnterCombatState(bool forceEnter = false)
{
   Debug.Log($"[EnterCombatState] forceEnter={forceEnter} | isInCombatState={player.isInCombatState} | wasEquipped={player.isEquipped} | sheathDiff={Time.time - _sheathTime} | autoLockDiff={Time.time - _autoSheatheLockUntil}");
    if (!forceEnter && Time.time - _sheathTime < SHEATHE_LOCK) return;
    if (forceEnter) _sheathTime = -999f;

    if (player.isInCombatState)
    {
        _lastCombatActionTime = Time.time;
        return;
    }

    bool wasEquipped = player.isEquipped; // ← lưu TRƯỚC
    _lastCombatActionTime  = Time.time;
    player.animator.SetBool("IsCombat", true);
    player.isEquipped      = true;
    player.isInCombatState = true;

    if (!wasEquipped) // ← chỉ draw nếu chưa cầm
        player.animator.SetTrigger("drawWeapon");

    player.ChangeState(player.combatState);
}
 
    /// <summary>Thoát combat, cất kiếm và về Idle.</summary>
    public void ExitCombatState()
    {

        _sheathTime = Time.time;
        _autoSheatheLockUntil = Time.time + AUTO_SHEATHE_DETECTION_LOCK;

        _comboCoolingDown = false;
        _comboStep        = 0;
        _attackBuffered   = false;  
         _isFirstEnter     = false;
 
        player.isEquipped = false;
        player.animator.SetBool("IsCombat", false);
        player.animator.ResetTrigger("drawWeapon");
        player.animator.SetTrigger("sheathWeapon");
        ResetAllAttackTriggers();
        
 
        player.ChangeState(player.idleState);
    }

    public override void Exit()
    {
     player.isInCombatState = false;
    }

    public void Reset()
    {
      _sheathTime           = -999f;
      _lastCombatActionTime = -999f;
      _attackBuffered       = false;
      _bufferTimer          = 0f;
      _comboStep            = 0;
      _comboCoolingDown     = false;
      _comboTimer           = 0f;
      _isFirstEnter         = false;
      _isFacingCamera       = false;
    }
    
 
    // ── Coroutine ─────────────────────────────────────────────────────────────
    private System.Collections.IEnumerator DisableRootMotionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        player.animator.applyRootMotion = false;
    }

    
}
    
    
 
   
 
    
 
 

