using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(PlayerController player) : base(player) { }


    [SerializeField] Animator animator;
    [SerializeField] float dampTime = 0.1f;


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


    void UpdateAnimator(Vector3 input)
    {
        animator.SetFloat("MoveX", input.x, dampTime, Time.deltaTime);
        animator.SetFloat("MoveY", input.z, dampTime, Time.deltaTime);

        float speed = input.magnitude;
        animator.SetFloat("Speed", speed);
    }

    Vector3 GetMoveInput()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camForward * v + camRight * h;
        return move;
    }
}
