using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float gravity = -20f;
    public float jumpForce = 1.5f;

    [Header("References")]
    public Transform cam;
    public Animator animator;

    [HideInInspector] public CharacterController controller;
    [HideInInspector] public Vector3 velocity;

    // States
    [HideInInspector] public PlayerIdleState idleState;
    [HideInInspector] public PlayerMoveState moveState;
    [HideInInspector] public PlayerJumpState jumpState;
    [HideInInspector] public PlayerFallState fallState;

    PlayerState currentState;


    void Awake()
    {
        controller = GetComponent<CharacterController>();

        idleState = new PlayerIdleState(this);
        moveState = new PlayerMoveState(this);
        jumpState = new PlayerJumpState(this);
        fallState = new PlayerFallState(this);
    }

    void Start()
    {
        ChangeState(idleState);
    }

    void Update()
    {
        currentState.Update();
    }

    public void ChangeState(PlayerState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    // ===== HÀM DÙNG CHUNG =====
    public Vector3 GetMoveInput()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 forward = cam.forward;
        Vector3 right = cam.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        return forward * z + right * x;
    }

    public bool HasMoveInput()
    {
        return GetMoveInput().magnitude > 0.1f;
    }

    public void ApplyGravity()
    {
        if (controller.isGrounded)
        {
            if (velocity.y < 0)
                velocity.y = -2f; // giữ chạm đất
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(Vector3.up * velocity.y * Time.deltaTime);
    }

    public void RotateToMove(Vector3 move)
    {
       
        Vector3 forward = transform.forward;

        
        if (Vector3.Dot(forward, move.normalized) < 0)
            return;

        Quaternion targetRot = Quaternion.LookRotation(move);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
    }
}
