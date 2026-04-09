using UnityEngine;

[CreateAssetMenu(fileName = "TalkAction", menuName = "Scriptable Objects/TalkAction")]
public class TalkAction : InteractionActionObject
{
    [SerializeField] private string _npcName = "NPC";
    [SerializeField, TextArea] private string _dialogueText = "Tôi có vài việc cần bạn giúp...";

    public override void OnInteract(InteractionContext context)
    {
        var questGiver = context.Interactable.GetComponent<QuestHolder>();
        if (questGiver != null && DialogueManager.Instance != null)
        {
            // Thay vì tự động nhận Quest, giờ nó mở bảng UI
            DialogueManager.Instance.OpenDialogue(_npcName, _dialogueText, questGiver);
        }
    }
}
