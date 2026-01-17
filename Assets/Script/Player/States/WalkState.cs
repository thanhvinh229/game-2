using UnityEngine;

public class WalkState : PlayerMoveState
{
     public WalkState (PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.animator.SetBool("IsWalking", true);
    }

    public override void Exit()
    {
        player.animator.SetBool("IsWalking", false);
    }

    public override void Update()
    {
        base.Update();
    }
}
