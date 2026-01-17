using UnityEngine;

public class RunState : PlayerMoveState
{
    public RunState (PlayerController player) : base(player) { }
    public override void Enter()
    {
        player.animator.SetBool("IsRunning", true);
    }

    public override void Exit()
    {
        player.animator.SetBool("IsRunning", false);
    }

    public override void Update()
    {
        base.Update();
    }
}
