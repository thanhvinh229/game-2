using UnityEngine;

[CreateAssetMenu(fileName = "TalkAction", menuName = "Scriptable Objects/TalkAction")]
public class TalkAction : InteractionActionObject
{
    public override void OnInteract(InteractionContext context)
    {
        base.OnInteract(context);
        if(context.Interactable.TryGetComponent<QuestHolder>(out var questholder))
        {
            questholder.GiveQuest();
        }
        Debug.Log("talk");
    }
}
