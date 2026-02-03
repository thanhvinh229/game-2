using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public class InteractableBase : MonoBehaviour, IInteractable
{
    [SerializeField] private InteractionData _data;
    [SerializeField] private bool _holdInteract;
    [SerializeField] private float _holdDuration;
    [SerializeField] private bool _canInteract;
    [SerializeField] private bool _canInspect;



    public InteractableData Data;
    public bool HoldInteract => _canInteract && _holdInteract;  
    public float HoldDuration => _holdDuration;
    public bool CanInteract => _canInteract;
    public bool CanInspect => _canInspect;
    public InteractionActionObject[] Actions;

    public void Interact(GameObject interactor, InteractionActionSlot slot)
    {
        var cotext = new InteractionContext(interactor, this);
        var action = Actions.FirstOrDefault(a => a.Slot == slot);
        if (action != null)
        {
            action.OnInteract(cotext);  
            
        }
    }
}
