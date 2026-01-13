using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(PlayerController player) : base(player) { }

    public override void Update()
    {
        Vector3 move = player.GetMoveInput();

        if (move.magnitude < 0.1f)
        {
            player.ChangeState(player.idleState);
            return;
        }

        player.controller.Move(move * player.moveSpeed * Time.deltaTime);
        player.RotateToMove(move);
        player.ApplyGravity();

        player.animator?.SetFloat("Speed", move.magnitude);

        if (Input.GetButtonDown("Jump") && player.controller.isGrounded)
            player.ChangeState(player.jumpState);
    }
}
