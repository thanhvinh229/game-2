using UnityEngine;

public class PlayerRunState : PlayerMoveState
{
    private const float DAMP_TIME = 0.1f;
 
    public PlayerRunState(PlayerController player) : base(player) { }
 
    public override void Update()
    {
        Vector3 move = player.GetMoveInput();
 
        if (move.magnitude < 0.1f)
        {
            player.ChangeState(player.idleState);
            return;
        }
 
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            player.ChangeState(player.moveState);
            return;
        }
 
        if (Input.GetButtonDown("Jump") && player.controller.isGrounded)
        {
            player.ChangeState(player.jumpState);
            return;
        }
 
        Vector3 movement  = move * player.runSpeed;
        player.velocity.x = movement.x;
        player.velocity.z = movement.z;
 
        UpdateAnimator(move);
    }
 
    private void UpdateAnimator(Vector3 move)
    {
        Vector3 local = player.transform.InverseTransformDirection(move);
        player.animator.SetFloat("MoveX", local.x,        DAMP_TIME, Time.deltaTime);
        player.animator.SetFloat("MoveY", local.z,        DAMP_TIME, Time.deltaTime);
        player.animator.SetFloat("Speed", move.magnitude, DAMP_TIME, Time.deltaTime);
        player.animator.SetBool("IsCombat", player.isEquipped);
    }
}
 
