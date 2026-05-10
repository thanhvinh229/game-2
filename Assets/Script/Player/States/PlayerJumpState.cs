using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(PlayerController player) : base(player) { }
 
    public override void Enter()
    {
        player.velocity.y = Mathf.Sqrt(player.jumpForce * -2f * player.gravity);
        player.animator.SetTrigger("Jump");
        player.SetCombatLayerWeight(0f);
    }
 
    public override void Update()
    {
        
        Vector3 move        = player.GetMoveInput();
        Vector3 airMovement = move * player.walkSpeed;
        player.velocity.x   = airMovement.x;
        player.velocity.z   = airMovement.z;
 
       
        if (!player.controller.isGrounded || player.velocity.y >= 0) return;
 
        if (player.isEquipped)
        {
            player.SetCombatLayerWeight(1f);
        }
        if (player.HasMoveInput())
        {
            player.ChangeState(player.isEquipped ? (PlayerState)player.combatState : player.moveState);
        }
        else
        {
            player.ChangeState(player.idleState);
        }
       

    }
}
 

