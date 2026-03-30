using System;
using UnityEngine;

public class PlayerCombatState : PlayerMoveState
{
    private float dampTime = 0.1f;
    private float idleTimer = 0f;
    private const float TIMEOUT_DURATION = 5f;
    private const float ROTATION_SPEED = 10f;
    private bool sheathWeapon;
    private bool isFacingCamera = false;
 
    // --- Combo System ---
    private int comboStep = 0;                    // Đòn hiện tại (0 = chưa đánh)
    private const int MAX_COMBO = 3;              // Tổng số đòn trong chuỗi
    private const float COMBO_RESET_TIME = 1.2f;  // Thời gian chờ tối đa giữa 2 đòn
    private const float COMBO_COOLDOWN = 1.0f;    // Cooldown sau khi đánh xong chuỗi
    private float comboTimer = 0f;                // Đếm ngược reset combo
    private bool comboCoolingDown = false;        // Đang trong cooldown sau chuỗi
    public PlayerCombatState(PlayerController player) : base(player) { }
 
 
 public override void Enter()
{
    
    player.animator.ResetTrigger("attack1");
    player.animator.ResetTrigger("attack2");
    player.animator.ResetTrigger("attack3");

    comboStep = 0;
    
    
    player.animator.SetLayerWeight(1, 1f);
}
    public override void Update()
{
    Vector3 move = player.GetMoveInput();
    bool isMoving = move.magnitude > 0.1f;
    bool isAttacking = Input.GetMouseButtonDown(0);
    bool isJumping = Input.GetButtonDown("Jump");
 
    
    if (Input.GetKeyDown(KeyCode.R))
    {
        player.animator.SetTrigger("sheathWeapon"); // Bạn cần có trigger này trong Animator
        player.ChangeState(player.moveState);
        return;
    }
 
    
    if (isAttacking)
    {
        HandleComboAttack();
    }
    UpdateComboTimer();
    //  Jump
    if (isJumping && player.controller.isGrounded)
    {
        player.ChangeState(player.jumpState);
        return;
    }
 
    //  Action -> idlestate
    if (isMoving || isAttacking || isJumping)
        {
            idleTimer = 0f;
        }
        else
        {
            
            idleTimer += Time.deltaTime;
        }
 
       
        if (idleTimer >= TIMEOUT_DURATION)
        {
            player.animator.SetTrigger("sheathWeapon"); 
            player.ChangeState(player.idleState);
            return;
        }
     
    HandleRotation(move, isAttacking);
       
 
            
 
    
    player.ApplyGravity();
    Vector3 movement = move * player.walkSpeed;
    movement.y = player.velocity.y; 
    
    player.controller.Move(movement * Time.deltaTime);
 
   
    UpdateCombatAnimator(move,0.5f);
}
 
 void HandleComboAttack()
{
    // Đang cooldown sau chuỗi combo -> bỏ qua input
    if (comboCoolingDown) return;
 
    comboStep++;
    comboTimer = COMBO_RESET_TIME; // Reset thời gian chờ đòn tiếp
 
    // Kích hoạt animation theo đòn hiện tại
    player.animator.SetTrigger("attack" + comboStep);
 
    // Nếu đã đánh đủ MAX_COMBO đòn -> vào cooldown và reset
    if (comboStep >= MAX_COMBO)
    {
        comboCoolingDown = true;
        comboTimer = COMBO_COOLDOWN;
        comboStep = 0;
    }
}
 
 void UpdateComboTimer()
{
    if (comboTimer > 0f)
    {
        comboTimer -= Time.deltaTime;
 
        if (comboTimer <= 0f)
        {
            // Hết thời gian chờ: reset combo hoặc kết thúc cooldown
            comboTimer = 0f;
            comboCoolingDown = false;
            comboStep = 0;
        }
    }
}
 
 void HandleRotation(Vector3 move, bool isAttacking)
{
    // Khi tấn công: quay mặt theo hướng camera đang nhìn
    if (isAttacking)
    {
        isFacingCamera = true;
    }
 
    if (isFacingCamera)
    {
        // Lấy hướng forward của camera, bỏ trục Y để tránh nhân vật nghiêng
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
 
            // Khi đã gần thẳng hướng camera thì dừng lock
            if (Quaternion.Angle(player.transform.rotation, targetRotation) < 2f)
                isFacingCamera = false;
        }
    }
    else if (move.magnitude > 0.1f)
    {
        // Không attack: quay theo hướng di chuyển như bình thường
        Quaternion targetRotation = Quaternion.LookRotation(move);
        player.transform.rotation = Quaternion.Slerp(
            player.transform.rotation,
            targetRotation,
            Time.deltaTime * ROTATION_SPEED
        );
    }
}
 
 void UpdateCombatAnimator(Vector3 move, float speedMultiplier)
{
    Vector3 local = player.transform.InverseTransformDirection(move);
 
 
    player.animator.SetFloat("MoveX", local.x * speedMultiplier,dampTime,  Time.deltaTime);
    player.animator.SetFloat("MoveY", local.z * speedMultiplier,dampTime, Time.deltaTime);
    player.animator.SetFloat("Speed", move.magnitude * speedMultiplier, dampTime, Time.deltaTime);
 
}
}

    
 
   
 
    
 
 

