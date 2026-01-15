using System;
using UnityEngine;

[Serializable]
public class InteractableBase : MonoBehaviour, IInteractable
{
    [SerializeField] private bool _holdInteract;
    [SerializeField] private float _holdDuration;
    [SerializeField] private bool _canInteract;
    [SerializeField] private bool _canInspect;
    public bool HoldInteract => _holdInteract;
    public float HoldDuration => _holdDuration;
    public bool CanInteract => _canInteract;
    public bool CanInspect => _canInspect;
    public InteractionActionObject[] Actions;
}
