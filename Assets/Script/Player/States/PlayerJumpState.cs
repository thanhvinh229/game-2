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
        
        Vector3 move        = player.GetMoveInput();
        Vector3 airMovement = move * player.walkSpeed;
        player.velocity.x   = airMovement.x;
        player.velocity.z   = airMovement.z;
 
       
        if (!player.controller.isGrounded || player.velocity.y >= 0) return;
 
        if (player.HasMoveInput())
        {
    if (player.isEquipped)
        player.ChangeState(player.combatState); // ← trực tiếp, không drawWeapon
    else
        player.ChangeState(player.moveState);
}
else
{
    player.ChangeState(player.idleState);
}
    }
}
 

