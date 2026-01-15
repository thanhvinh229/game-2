using UnityEngine;

[CreateAssetMenu(menuName = "Interaction System/Interaction Data", fileName = "InteractionData")]
public class InteractionData : ScriptableObject
{
    private InteractableBase _interactable;
    public InteractableBase Interactable
    {
        get { return _interactable; }
        set { _interactable = value; }
    }
    public bool IsTheSame(InteractableBase newInteractable) => _interactable == newInteractable;

}