using UnityEngine;

public class PlayerJumpState : PlayerState
{
    
    public PlayerJumpState(PlayerController player) : base(player) { }

   public override void Enter()
    {
       
        player.velocity.y = Mathf.Sqrt(player.jumpForce * -2f * player.gravity);
        player.animator.SetTrigger("Jump");
    }

    public override void Update()
    {
       
        Vector3 move = player.GetMoveInput();
        
        
        player.ApplyGravity();

        
        Vector3 airMovement = move * player.walkSpeed; 
        airMovement.y = player.velocity.y;

        
        player.controller.Move(airMovement * Time.deltaTime);

        
        if (player.controller.isGrounded && player.velocity.y < 0)
        {
            if (player.HasMoveInput())
            {
                player.ChangeState(player.moveState);
            }
            else
            { 
                player.ChangeState(player.idleState);
            }
        }

        if (player.controller.isGrounded && player.velocity.y < 0)
        {
            if (player.HasMoveInput())
            {
            // Nếu đang cầm kiếm thì về CombatState, nếu không thì về MoveState
            if (player.isEquipped) 
                player.ChangeState(player.combatState);
            else
                player.ChangeState(player.moveState);
            }
            else
            {
                player.ChangeState(player.idleState);
            }
        }
   }
}

