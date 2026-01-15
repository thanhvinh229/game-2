using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private InteractionInputData _interactionInputData;
    public static event Action<InteractionActionSlot> OnInteraction;
    void Awake()
    {
        _interactionInputData.PrimaryAction.performed += OnPrimaryAction;
        _interactionInputData.SecondaryAction.performed += OnSecondaryAction;
        _interactionInputData.TertiaryAction.performed += OnTertiaryAction;
    }

    public void OnPrimaryAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnInteraction?.Invoke(InteractionActionSlot.Primary);
        }
    }
    public void OnSecondaryAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnInteraction?.Invoke(InteractionActionSlot.Secondary);
        }
    }
    public void OnTertiaryAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnInteraction?.Invoke(InteractionActionSlot.Tertiary);
        }
    }
}