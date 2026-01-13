using UnityEngine;

public class Interactable : MonoBehaviour
{
     public bool CanInteract { get; set; } = true;

    public void Oninteract()
    {
        Debug.Log($" take  { gameObject.name} ");
    }
}
