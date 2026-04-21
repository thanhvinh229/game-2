using UnityEngine;

public class PlayerIdleState : PlayerMoveState
{
    private float dampTime = 0.1f;
    public PlayerIdleState(PlayerController player) : base(player) { }
 
    public override void Enter()
    {
        player.animator?.SetFloat("Speed", 0);
    }
 
 
    public override void Update()
    {
         player.velocity.x = 0;
         player.velocity.z = 0;
        
 
        if (player.GetMoveInput().magnitude > 0.1f)
        {
             player.ChangeState(player.moveState);
             return;
        }
            
 
        if (Input.GetButtonDown("Jump") && player.controller.isGrounded)
            player.ChangeState(player.jumpState);
 
        // Vào combat khi click chuột và đã trang bị vũ khí
        if (Input.GetMouseButtonDown(0) && player.isEquipped)
        {
            player.ChangeState(player.combatState);
            return;
        }
 
 
        player.animator.SetFloat("MoveX", 0, dampTime, Time.deltaTime);
        player.animator.SetFloat("MoveY", 0, dampTime, Time.deltaTime);
        player.animator.SetFloat("Speed", 0, dampTime, Time.deltaTime);
    }
 
 
}
