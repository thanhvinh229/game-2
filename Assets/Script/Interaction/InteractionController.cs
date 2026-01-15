using System.Linq;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private InteractionData _interactionData;
    [SerializeField] private InteractionInputData _interactionInputData;
    [SerializeField] private float _interactRadius;
    [SerializeField] private float _interactDistance;
    [SerializeField] private LayerMask _interactLayer;
    [SerializeField] private InteractionActionUILayer _primaryActionUI;
    [SerializeField] private InteractionActionUILayer _secondaryActionUI;
    [SerializeField] private InteractionActionUILayer _tertiaryActionUI;
    private float _holdTime;
    private bool _isInteracting;
    private Camera _interactCamera;
    void Start()
    {
        _interactCamera = Camera.main;
        InputHandler.OnInteraction += HandleAction;
    }
    void Update()
    {
        CheckInteractionRaycast();
    }
    void OnEnable()
    {
        _interactionInputData.EnableInput();
    }
    void OnDisable()
    {
        _interactionInputData.DisableInput();
    }

    private void CheckInteractionRaycast()
    {
        RaycastHit hitInfo;
        if (Physics.SphereCast(_interactCamera.transform.position, _interactRadius, _interactCamera.transform.forward, out hitInfo, _interactDistance, _interactLayer))
        {
            if (hitInfo.collider.gameObject.TryGetComponent<InteractableBase>(out var interactable))
            {
                if (!_interactionData.IsTheSame(interactable))
                {
                    var primaryAction = interactable.Actions.FirstOrDefault(x => x.Slot == InteractionActionSlot.Primary);
                    _primaryActionUI.gameObject.SetActive(primaryAction != null);
                    _primaryActionUI.SetInteractionPromptText(primaryAction != null ? primaryAction.ActionPrompt : "");

                    var secondaryAction = interactable.Actions.FirstOrDefault(x => x.Slot == InteractionActionSlot.Secondary);
                    _secondaryActionUI.gameObject.SetActive(secondaryAction != null);
                    _secondaryActionUI.SetInteractionPromptText(secondaryAction != null ? secondaryAction.ActionPrompt : "");

                    var tertiaryAction = interactable.Actions.FirstOrDefault(x => x.Slot == InteractionActionSlot.Tertiary);
                    _tertiaryActionUI.gameObject.SetActive(tertiaryAction != null);
                    _tertiaryActionUI.SetInteractionPromptText(tertiaryAction != null ? tertiaryAction.ActionPrompt : "");
                }
                _interactionData.Interactable = interactable;
            }
        }
        else
        {
            _interactionData.Interactable = null;
            _primaryActionUI.gameObject.SetActive(false);
            _secondaryActionUI.gameObject.SetActive(false);
            _tertiaryActionUI.gameObject.SetActive(false);
        }
    }
    private void HandleAction(InteractionActionSlot actionSlot)
    {
        var currentInteractable = _interactionData.Interactable;
        var slot = currentInteractable.Actions.FirstOrDefault(x => x.Slot == actionSlot);
        if (currentInteractable.CanInteract)
        {
            _isInteracting = true;
            slot.OnInteract();
        }
        // if (_interactionInputData.InteractReleased)
        // {
        //     _isInteracting = false;
        //     _holdTime = 0f;
        // }
        // if (_isInteracting)
        // {
        //     if (currentInteractable.HoldInteract)
        //     {
        //         _holdTime += Time.deltaTime;
        //         if (_holdTime >= currentInteractable.HoldDuration)
        //         {
        //             currentInteractable.OnInteract();
        //         }
        //         _interactionPromptUI.SetProgress(_holdTime / currentInteractable.HoldDuration);
        //     }
        // }
    }
    void OnDrawGizmos()
    {
        if (_interactCamera == null)
        {
            return;
        }
        Gizmos.color = Color.green;
        Gizmos.DrawRay(_interactCamera.transform.position, _interactCamera.transform.forward * _interactDistance);
    }
}