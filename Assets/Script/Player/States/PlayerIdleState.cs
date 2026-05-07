using UnityEngine;

public class PlayerIdleState : PlayerMoveState
{
    private const float DAMP_TIME = 0.1f;
 
    public PlayerIdleState(PlayerController player) : base(player) { }
 
    public override void Enter()
    {
        Debug.Log("[IdleState] Enter() called");
        player.velocity.x = 0f;
        player.velocity.z = 0f;
        player.animator?.SetFloat("Speed", 0f);
    }
 
    public override void Update()
    {
        player.velocity.x = 0f;
        player.velocity.z = 0f;
 
        if (player.GetMoveInput().magnitude > 0.1f)
        {
            player.ChangeState(player.moveState);
            return;
        }
 
        if (Input.GetButtonDown("Jump") && player.controller.isGrounded)
        {
            player.ChangeState(player.jumpState);
            return;
        }
 
        if (Input.GetMouseButtonDown(0) && player.isEquipped)
        {
           player.combatState.EnterCombatState();
           return;
        }
 
        player.animator.SetFloat("MoveX", 0f, DAMP_TIME, Time.deltaTime);
        player.animator.SetFloat("MoveY", 0f, DAMP_TIME, Time.deltaTime);
        player.animator.SetFloat("Speed", 0f, DAMP_TIME, Time.deltaTime);
    }
}
