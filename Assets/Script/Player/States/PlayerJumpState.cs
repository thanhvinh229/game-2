using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.velocity.y = Mathf.Sqrt(player.jumpForce * -2f * player.gravity);
        player.animator?.SetTrigger("Jump");
    }

    public override void Update()
    {
        Vector3 move = player.GetMoveInput();
        player.controller.Move(move * player.moveSpeed * Time.deltaTime);
        player.RotateToMove(move);
        player.ApplyGravity();

        if (player.velocity.y < 0)
            player.ChangeState(player.fallState);
    }
}
