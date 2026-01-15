using UnityEngine;

public class InteractionActionObject : ScriptableObject
{
    public string ActionPrompt;
    public InteractionActionSlot Slot;
    public virtual void OnInteract() { }
}

public enum InteractionActionSlot
{
    Primary,
    Secondary,
    Tertiary
}