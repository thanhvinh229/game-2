using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InteractionInputData", menuName = "Scriptable Objects/InteractionInputData")]
public class InteractionInputData : ScriptableObject
{

    public InputAction InteractionInput;

    private bool _interactionPressed;

    private bool _interactionReleased;

    public bool InteractionPressed
    {
       get => _interactionPressed;
        set => _interactionPressed = value;
    }

public bool InteractionReleased
    {
        get => _interactionReleased;
        set => _interactionReleased = value;
    }

    public void Enable()
    {
        InteractionInput.Enable();
    }
    public void Disable()
    {
        InteractionInput.Disable();
    }
}
 
    
