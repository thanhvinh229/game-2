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

        if (Input.GetButtonDown("Jump") && player.controller.isGrounded)
        {
            player.ChangeState(player.jumpState);
        }

        
         if(Input.GetKey(KeyCode.LeftShift))
        {
            player.ChangeState(player.runState);
            return; 
        }
        if (Input.GetKey(KeyCode.R))
        {
            player.ChangeState(player.attackState);
            player.animator.SetTrigger("drawWeapon");
            return;
        
        }
        
        player.ApplyGravity();
        
        
        Vector3 movement = move * player.walkSpeed;
        movement.y = player.velocity.y; 
        player.controller.Move(movement * Time.deltaTime);

       UpdateAnimator(move, 0.5f);

        
        


        
    } 

    void UpdateAnimator(Vector3 move ,float speedMultiplier)
    {
      
    Vector3 local = player.transform.InverseTransformDirection(move);


    player.animator.SetFloat("MoveX", local.x * speedMultiplier,dampTime,  Time.deltaTime);
    player.animator.SetFloat("MoveY", local.z * speedMultiplier,dampTime, Time.deltaTime);
    player.animator.SetFloat("Speed", move.magnitude * speedMultiplier, dampTime, Time.deltaTime);

}
}


