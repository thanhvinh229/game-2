using UnityEngine;

public class InteractionContext : MonoBehaviour
{
    public GameObject Interactor;

    public InteractableBase Interactable;

    public InteractionContext(GameObject interactor, InteractableBase interactable)
    {
        Interactor = interactor;
        interactable = Interactable;
    }

}
