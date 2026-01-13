using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private InteractionInputData _interactionInputData;

    void Update()
    {
        _interactionInputData.InteractionInput.performed += ctx =>
        {
            _interactionInputData.InteractionPressed = true;
            _interactionInputData.InteractionReleased = false;
        };


        _interactionInputData.InteractionInput.canceled += ctx =>
        {
            _interactionInputData.InteractionPressed = false;
            _interactionInputData.InteractionReleased = true;
        };
    }

}