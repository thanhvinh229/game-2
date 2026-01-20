using UnityEngine;

public class PlayerIdleState : PlayerMoveState
{
    public PlayerIdleState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.animator?.SetFloat("Speed", 0);
    }


    public override void Update()
    {
        player.ApplyGravity();

        if (player.HasMoveInput())
            player.ChangeState(player.moveState);

        if (Input.GetButtonDown("Jump") && player.controller.isGrounded)
            player.ChangeState(player.jumpState);
    }


}
