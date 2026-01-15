using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionInputData : ScriptableObject
{
    public InputAction PrimaryAction;
    public InputAction SecondaryAction;
    public InputAction TertiaryAction;
    public void EnableInput()
    {
        PrimaryAction.Enable();
        SecondaryAction.Enable();
        TertiaryAction.Enable();
    }
    public void DisableInput()
    {
        PrimaryAction.Disable();
        SecondaryAction.Disable();
        TertiaryAction.Disable();
    }
}

public class InteractionInputAction
{
    public InputAction Action;
    public GameObject GlyphPrefab;
    public void Enable()
    {
        Action?.Enable();
    }
    public void Disable()
    {
        Action?.Disable();
    }
}