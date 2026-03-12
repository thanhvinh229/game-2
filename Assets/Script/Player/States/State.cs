using UnityEngine;
using UnityEngine.InputSystem;

public class State : MonoBehaviour
{
protected PlayerController character;
    protected StateMachine stateMachine;

    protected Vector3 gravityVelocity;
    protected Vector3 velocity;
    protected Vector2 input;

    
    protected InputAction moveAction;
    protected InputAction lookAction;
    protected InputAction jumpAction;
    protected InputAction crouchAction;
    protected InputAction sprintAction;
    protected InputAction drawWeaponAction;
    protected InputAction attackAction;

    
    public State(PlayerController _character, StateMachine _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;

        
        var playerInput = character.GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
        crouchAction = playerInput.actions["Crouch"];
        sprintAction = playerInput.actions["Sprint"];
        drawWeaponAction = playerInput.actions["DrawWeapon"];
        attackAction = playerInput.actions["Attack"];
    }

    public virtual void Enter()
    {
        Debug.Log("Enter State: " + this.GetType().Name);
    }

    public virtual void HandleInput()
    {
        
        input = moveAction.ReadValue<Vector2>();
    }

    public virtual void LogicUpdate()
    {
    }

    public virtual void PhysicsUpdate()
    {
    }

    public virtual void Exit()
    {
    }
}

