using UnityEngine;

public class InteractionContext 
{
    public GameObject Interactor { get; }

    public InteractableBase Interactable { get; }

    public InteractionContext(GameObject interactor, InteractableBase interactable)
    {
        Interactor = interactor;
        Interactable = interactable;
    }

}
