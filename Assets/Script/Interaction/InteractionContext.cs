using UnityEngine;

public class InteractionContext : MonoBehaviour
{
    public GameObject Interactor { get; }

    public InteractableBase Interactable { get; }

    public InteractionContext(GameObject interactor, InteractableBase interactable)
    {
        Interactor = interactor;
        Interactable = interactable;
    }

}
