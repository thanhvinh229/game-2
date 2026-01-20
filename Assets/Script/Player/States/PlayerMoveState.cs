using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(PlayerController player) : base(player) { }


    [SerializeField] Animator animator;
    [SerializeField] float dampTime = 0.1f;


   

    public override void Update()
    {

        Vector3 move = player.GetMoveInput();

        //Debug.Log(move);

        if (move.magnitude < 0.1f)
        {
            player.ChangeState(player.idleState);
            return;
        }

        bool isRun = Input.GetKey(KeyCode.LeftShift);
        float speed = isRun ? player.runSpeed : player.walkSpeed;

        player.controller.Move(move * player.moveSpeed * Time.deltaTime);
       
        player.ApplyGravity();

        UpdateAnimator(move, isRun);

        Vector3 finalMove = move * speed + Vector3.up * player.velocity.y;
        player.controller.Move(finalMove * Time.deltaTime);




        if (Input.GetButtonDown("Jump") && player.controller.isGrounded)
            player.ChangeState(player.jumpState);
             return;
    }


    void UpdateAnimator(Vector3 move, bool isRun)
    {

        

        Vector3 local = player.transform.InverseTransformDirection(move);

        player.animator.SetFloat("MoveX", local.x, 0.1f, Time.deltaTime);
        player.animator.SetFloat("MoveY", local.z, 0.1f, Time.deltaTime);

        player.animator.SetBool("IsRun", isRun);
        player.animator.SetFloat("Speed", isRun ? 1f : 0.5f, 0.1f, Time.deltaTime);
    }

    Vector3 GetMoveInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

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
