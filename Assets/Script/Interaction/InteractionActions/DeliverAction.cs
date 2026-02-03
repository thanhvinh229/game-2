using UnityEngine;

[CreateAssetMenu(fileName = "DeliverAction", menuName = "Interaction System/Interaction Action/DeliverAction")]
public class DeliverAction : InteractionActionObject

{
    [SerializeField] private string _itemId;

    [SerializeField] private QuestEventChannel _questEventChannel;

   
    public override void OnInteract(InteractionContext context)
    {
        base.OnInteract(context);
        Debug.Log("Deliver");
        if (context.Interactor.TryGetComponent<PlayerController>(out var playerController))
        {
            if (playerController.CollectedItemGuid == _itemId)
            {
                _questEventChannel?.OnDeliverItem?.Invoke(_itemId);
            }
        }

    }
}