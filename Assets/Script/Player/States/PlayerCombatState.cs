using System;
using UnityEngine;

public class PlayerCombatState : PlayerMoveState
{
    private float dampTime = 0.1f;
    private const float ROTATION_SPEED = 10f;
    private bool sheathWeapon;
    private bool isFacingCamera = false;
 
    // --- Combo System ---
    private int comboStep = 0;
    private const int MAX_COMBO = 3;
    private const float COMBO_RESET_TIME = 0.5f;
    private const float COMBO_COOLDOWN = 1.0f;
    private float comboTimer = 0f;
    private bool comboCoolingDown = false;
 
    public float autoSheatheTime = 5f;  // 5 giây không hành động thì về idle
    
    private float lastCombatActionTime;
    private bool isInCombat;
 
 
    // --- Input Buffer ---
    // Lưu input khi Animator chưa sẵn sàng, thay vì bỏ mất
    private bool attackBuffered = false;
    private const float BUFFER_WINDOW = 0.25f;
    private float bufferTimer = 0f;
 
    public PlayerCombatState(PlayerController player) : base(player) { }
 
    public override void Enter()
    {
        isInCombat = true;
        lastCombatActionTime = Time.time;
    }
 
    public override void Update()
    {
        Vector3 move = player.GetMoveInput();
        bool isMoving = move.magnitude > 0.1f;
        bool isAttacking = Input.GetMouseButtonDown(0);
        bool isJumping = Input.GetButtonDown("Jump");

        if (GameStateManager.IsUIOpen)
        { 
        
        player.animator.SetFloat("MoveX", 0f, 0.1f, Time.deltaTime);
        player.animator.SetFloat("MoveY", 0f, 0.1f, Time.deltaTime);
        player.animator.SetFloat("Speed", 0f, 0.1f, Time.deltaTime);
        player.ApplyGravity(); 
        return;
        }
 
        // 1. Nếu đang di chuyển và nhấn Shift -> Chuyển sang trạng thái Chạy
        if (Input.GetKey(KeyCode.LeftShift) && player.HasMoveInput())
        {
            player.ChangeState(player.runState);
            return;
        }
 
        // Jump
        if (isJumping && player.controller.isGrounded)
        {
            player.ChangeState(player.jumpState);
            return;
        }
 
        // Nhận input vào buffer
        if (isAttacking)
        {
            attackBuffered = true;
            bufferTimer = BUFFER_WINDOW;
 
            // Đánh dấu đang trong combat và reset bộ đếm tự thoát
            isInCombat = true;
            lastCombatActionTime = Time.time;
        }
 
        // Cập nhật combo timer trước
        UpdateComboTimer();
 
        // Thử xử lý buffer
        TryConsumeBuffer();
 
        // Reset bộ đếm khi có hành động
        if (isMoving || isAttacking || isJumping)
            lastCombatActionTime = Time.time;
 
        // Tự động về idle sau autoSheatheTime giây không hành động
        float timeSinceLastAction = Time.time - lastCombatActionTime;
        if (timeSinceLastAction > autoSheatheTime)
        {
            ExitCombatState();
            return;
        }
 
        HandleRotation(move, isAttacking);
        player.ApplyGravity();
 
        Vector3 movement = move * player.walkSpeed;
        movement.y = player.velocity.y;
        player.controller.Move(movement * Time.deltaTime);
 
        UpdateCombatAnimator(move, 0.5f);
    }
 
    void TryConsumeBuffer()
    {
        // Không có input đang chờ
        if (!attackBuffered) return;
 
        // Đếm ngược buffer — nếu hết hạn thì bỏ input
        bufferTimer -= Time.deltaTime;
        if (bufferTimer <= 0f)
        {
            attackBuffered = false;
            return;
        }
 
        // Đang cooldown sau chuỗi -> chưa nhận được
        if (comboCoolingDown) return;
 
        // Animator đang transition -> chưa nhận được, giữ buffer chờ tiếp
        if (player.animator.IsInTransition(1)) return;
 
        // Kiểm tra animation hiện tại đã qua cửa sổ combo chưa
        // (>= 30% để nhận sớm, tránh nhận quá sớm gây giật animation)
        AnimatorStateInfo stateInfo = player.animator.GetCurrentAnimatorStateInfo(1);
        bool isInAttackAnim = stateInfo.IsTag("Attack");
        if (isInAttackAnim && stateInfo.normalizedTime < 0.3f) return;
 
        // Sẵn sàng — consume buffer và thực hiện đòn
        attackBuffered = false;
        bufferTimer = 0f;
        HandleComboAttack();
    }
 
    void HandleComboAttack()
    {
        if (comboCoolingDown) return;
 
        // Reset bộ đếm mỗi lần thực sự thực hiện đòn
        lastCombatActionTime = Time.time;
 
        comboStep++;
        comboTimer = COMBO_RESET_TIME;
 
        ResetAllAttackTriggers();
        player.animator.SetTrigger("attack" + comboStep);
 
        if (comboStep >= MAX_COMBO)
        {
            comboCoolingDown = true;
            comboTimer = COMBO_COOLDOWN;
            comboStep = 0;
        }
    }
 
    void UpdateComboTimer()
    {
        if (comboTimer <= 0f) return;
 
        comboTimer -= Time.deltaTime;
        if (comboTimer <= 0f)
        {
            comboTimer = 0f;
            comboCoolingDown = false;
            comboStep = 0;
            ResetAllAttackTriggers();
        }
    }
 
    void ResetAllAttackTriggers()
    {
        for (int i = 1; i <= MAX_COMBO; i++)
            player.animator.ResetTrigger("attack" + i);
    }
 
    void HandleRotation(Vector3 move, bool isAttacking)
    {
        if (isAttacking) isFacingCamera = true;
 
        if (isFacingCamera)
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
                    isFacingCamera = false;
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
 
 
    // Hàm này được gọi bởi PlayerSkills khi dùng chiêu
    public void EnterCombatState()
    {
        isInCombat = true;
        lastCombatActionTime = Time.time;
        player.animator.SetBool("IsCombat", true);
        player.isEquipped = true;
 
        // ChangeState sẽ gọi Enter() → set lại lastCombatActionTime
        player.ChangeState(player.combatState);
    }
 
    public void ExitCombatState()
    {
        isInCombat = false;
        comboCoolingDown = false;
        comboStep = 0;
        attackBuffered = false;
 
        player.isEquipped = false;
        player.animator.SetBool("IsCombat", false);
        player.animator.SetTrigger("sheathWeapon");
        ResetAllAttackTriggers();
 
        // Ẩn vũ khí
        player.ToggleWeaponVisibility(1);
 
        // Về idle — isEquipped = false nên IdleState sẽ không trigger combat lại
        player.ChangeState(player.idleState);
    }
 
    void UpdateCombatAnimator(Vector3 move, float speedMultiplier)
    {
        Vector3 local = player.transform.InverseTransformDirection(move);
        player.animator.SetFloat("MoveX", local.x * speedMultiplier, dampTime, Time.deltaTime);
        player.animator.SetFloat("MoveY", local.z * speedMultiplier, dampTime, Time.deltaTime);
        player.animator.SetFloat("Speed", move.magnitude * speedMultiplier, dampTime, Time.deltaTime);
    }
}
    
    
 
   
 
    
 
 

