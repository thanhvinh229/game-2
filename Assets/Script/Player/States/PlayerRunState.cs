using UnityEngine;

public class PlayerRunState : PlayerMoveState
{
    private float dampTime = 0.1f;

    public PlayerRunState(PlayerController player) : base(player) { }

    public override void Update()
    {
        Vector3 move = player.GetMoveInput();

        // 1. Nếu không còn bấm nút di chuyển -> Về Idle
        if (move.magnitude < 0.1f)
        {
            player.ChangeState(player.idleState);
            return;
        }

        // 2. Nếu nhả phím Shift -> Quay lại trạng thái đi bộ (MoveState)
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            player.ChangeState(player.moveState);
            return;
        }

        // 3. Xử lý nhảy khi đang chạy
        if (Input.GetButtonDown("Jump") && player.controller.isGrounded)
        {
            player.ChangeState(player.jumpState);
            return;
        }

        // 4. Logic di chuyển với tốc độ Chạy (runSpeed)
       
        Vector3 movement = move * player.runSpeed;
        player.velocity.x = movement.x;
        player.velocity.z = movement.z;
        
       

        // 5. Cập nhật Animator với multiplier là 1.0f (Full speed)
        UpdateAnimator(move);
    }

    private void UpdateAnimator(Vector3 move)
    {
        Vector3 local = player.transform.InverseTransformDirection(move);

        // Chạy thì multiplier là 1.0f
        player.animator.SetFloat("MoveX", local.x * 1.0f, dampTime, Time.deltaTime);
        player.animator.SetFloat("MoveY", local.z * 1.0f, dampTime, Time.deltaTime);
        player.animator.SetFloat("Speed", move.magnitude * 1.0f, dampTime, Time.deltaTime);

        player.animator.SetBool("IsCombat", player.isEquipped);
    }
}
