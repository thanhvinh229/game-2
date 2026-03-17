using System;
using UnityEngine;

public class PlayerCombatState : PlayerMoveState
{
    private float dampTime = 0.1f;
    private float idleTimer = 0f;
    private const float TIMEOUT_DURATION = 5f;
    private bool sheathWeapon;
    public PlayerCombatState(PlayerController player) : base(player) { }


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
            player.animator.SetTrigger("attack");   
            
        }
    //  Jump
    if (Input.GetButtonDown("Jump") && player.controller.isGrounded)
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
     
     if (move.magnitude > 0.1f)
    {
        // Tạo hướng quay dựa trên vector di chuyển
        Quaternion targetRotation = Quaternion.LookRotation(move);
        
        // Xoay nhân vật mượt mà về hướng đó
        player.transform.rotation = Quaternion.Slerp(
            player.transform.rotation, 
            targetRotation, 
            Time.deltaTime * 10f // Tốc độ xoay
        );
    }
       

            

    
    player.ApplyGravity();
    Vector3 movement = move * player.walkSpeed;
    movement.y = player.velocity.y; 
    
    player.controller.Move(movement * Time.deltaTime);

   
    UpdateCombatAnimator(move,0.5f);
}

 void UpdateCombatAnimator(Vector3 move, float speedMultiplier)
{
    Vector3 local = player.transform.InverseTransformDirection(move);


    player.animator.SetFloat("MoveX", local.x * speedMultiplier,dampTime,  Time.deltaTime);
    player.animator.SetFloat("MoveY", local.z * speedMultiplier,dampTime, Time.deltaTime);
    player.animator.SetFloat("Speed", move.magnitude * speedMultiplier, dampTime, Time.deltaTime);

}
}
    
 
   
 
    
 
 

