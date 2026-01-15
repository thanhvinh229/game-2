using System.Collections.Generic;

public interface IInteractable
{
    bool HoldInteract { get; }
    bool CanInteract { get; }
}