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
        player.ApplyGravity();

        Vector3 verticalMove = Vector3.up * player.velocity.y;
        player.controller.Move(verticalMove * Time.deltaTime);

        if (player.controller.isGrounded && player.velocity.y < 0)
        {
            player.ChangeState(player.idleState);
        }
    }

    public override void Exit() { }
}

