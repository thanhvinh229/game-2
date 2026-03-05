using UnityEngine;

public class PlayerMoveState : PlayerState
{
    private float dampTime = 0.1f;

    public PlayerMoveState(PlayerController player) : base(player) { }

    public override void Update()
    {
       
        Vector3 move = player.GetMoveInput();

        
        if (move.magnitude < 0.1f)
        {
            player.ChangeState(player.idleState);
            return; 
        }

        
        bool isRun = Input.GetKey(KeyCode.LeftShift);
        float targetSpeed = isRun ? player.runSpeed : player.walkSpeed;
        
        
        player.ApplyGravity();
        
        
        Vector3 movement = move * targetSpeed;
        movement.y = player.velocity.y; 
        
        player.controller.Move(movement * Time.deltaTime);

        // 5. Cập nhật Animator
        UpdateAnimator(move, isRun) ;
       

        // 6. Kiểm tra lệnh Nhảy
        if (Input.GetButtonDown("Jump") && player.controller.isGrounded)
        {
            player.ChangeState(player.jumpState);
        }
    } // Đóng ngoặc hàm Update đúng vị trí

    void UpdateAnimator(Vector3 move, bool isRun)
    {
      // Lấy hướng di chuyển cục bộ
    Vector3 local = player.transform.InverseTransformDirection(move);

    // Nếu đang chạy, giá trị MoveX/MoveY sẽ đạt tối đa là 1 hoặc -1
    // Nếu đi bộ, chúng ta nhân với 0.5f để nó chỉ ở vùng "Walk" trong Blend Tree
    float multiplier = isRun ? 1.0f : 0.5f;

    player.animator.SetFloat("MoveX", local.x * multiplier, 0.1f, Time.deltaTime);
    player.animator.SetFloat("MoveY", local.z * multiplier, 0.1f, Time.deltaTime);

    // Cập nhật thêm biến Speed để thoát trạng thái Idle (nếu cần)
    player.animator.SetFloat("Speed", move.magnitude * multiplier, 0.1f, Time.deltaTime);

}
}


