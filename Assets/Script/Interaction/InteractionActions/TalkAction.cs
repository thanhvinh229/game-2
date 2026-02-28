using UnityEngine;

[CreateAssetMenu(fileName = "TalkAction", menuName = "Scriptable Objects/TalkAction")]
public class TalkAction : InteractionActionObject
{
    public override void OnInteract(InteractionContext context)
    {
        base.OnInteract(context);
        Debug.Log("talk");
        var interactable = context.Interactable;
        var questGiver = interactable.GetComponent<QuestHolder>();
        questGiver.GiveQuest();

        
    }
}
