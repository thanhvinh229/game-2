using UnityEngine;

public class PlayerFallState : PlayerState
{
    public PlayerFallState(PlayerController player) : base(player) { }

    public override void Update()
    {
        player.ApplyGravity();

        if (player.controller.isGrounded && player.velocity.y <= 0)
        {
            player.velocity.y = -2f;
            player.ChangeState(player.idleState);
        }
    }
}
