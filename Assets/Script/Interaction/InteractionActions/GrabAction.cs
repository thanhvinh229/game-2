using UnityEngine;

[CreateAssetMenu(fileName = "GrabAction", menuName = "Interaction System/Interaction Action/Grab Action")]
public class GrabAction : InteractionActionObject
{
    public override void OnInteract(InteractionContext context)
    {
        base.OnInteract(context);
        Debug.Log("Grabbing");
    }
}