using UnityEngine;

public class PlayerMoveState : PlayerState
{
    private const float DAMP_TIME = 0.1f;
 
    public PlayerMoveState(PlayerController player) : base(player) { }
 
    public override void Update()
    {
        Vector3 move = player.GetMoveInput();
 
        if (move.magnitude < 0.1f)
        {
            player.ChangeState(player.idleState);
            return;
        }
 
        if (Input.GetButtonDown("Jump") && player.controller.isGrounded)
        {
            player.ChangeState(player.jumpState);
            return;
        }
 
        if (Input.GetKey(KeyCode.LeftShift))
        {
            player.ChangeState(player.runState);
            return;
        }
 
        if (Input.GetMouseButtonDown(0))
        {
            player.isEquipped = true;
            player.animator.SetTrigger("drawWeapon");
            player.combatState.EnterCombatState();
            return;
        }
 
        Vector3 movement  = move * player.walkSpeed;
        player.velocity.x = movement.x;
        player.velocity.z = movement.z;
 
        UpdateAnimator(move, 0.5f);
    }
 
    private void UpdateAnimator(Vector3 move, float speedMultiplier)
    {
        Vector3 local = player.transform.InverseTransformDirection(move);
        player.animator.SetFloat("MoveX",  local.x * speedMultiplier, DAMP_TIME, Time.deltaTime);
        player.animator.SetFloat("MoveY",  local.z * speedMultiplier, DAMP_TIME, Time.deltaTime);
        player.animator.SetFloat("Speed",  move.magnitude * speedMultiplier, DAMP_TIME, Time.deltaTime);
    }
}


