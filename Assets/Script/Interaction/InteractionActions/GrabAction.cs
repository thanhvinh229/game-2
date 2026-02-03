using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GrabAction", menuName = "Interaction System/Interaction Action/Grab Action")]
public class GrabAction : InteractionActionObject
{
    [SerializeField] private QuestEventChannel _questEventChannel;
    public override void OnInteract(InteractionContext context)
    {

        base.OnInteract(context);
        

        if (context.Interactor.TryGetComponent<PlayerController>(out var playerController))
        {
            
            playerController.CollectedItemGuid = context.Interactable.Data.Guid;
            Debug.Log($"Grabbing {context.Interactable.Data.Guid} ");
           _questEventChannel?.OnCollectItem?.Invoke(context.Interactable.Data.Guid);
            
        }
    }
}