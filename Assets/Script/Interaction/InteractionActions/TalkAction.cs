using UnityEngine;

[CreateAssetMenu(fileName = "TalkAction", menuName = "Scriptable Objects/TalkAction")]
public class TalkAction : InteractionActionObject
{
    [SerializeField] private string _npcName = "NPC";
    [SerializeField, TextArea] private string _dialogueText = "Tôi có vài việc cần bạn giúp...";

    [Header("Audio")]
    [SerializeField] private AudioClip _npcGreetingVoice;

    public override void OnInteract(InteractionContext context)
    {
        var interactable = context.Interactable;
        var questGiver = context.Interactable.GetComponent<QuestHolder>();
        var animator = interactable.GetComponent<Animator>();
        if (DialogueManager.Instance == null) 
    {
        Debug.LogError("LỖI: DialogueManager chưa được khởi tạo! Hãy kiểm tra xem nó có nằm trên object nào bị tắt không.");
        return;
    }

        if (questGiver != null && DialogueManager.Instance != null)
        {
            // Thay vì tự động nhận Quest, giờ nó mở bảng UI
            DialogueManager.Instance.OpenDialogue(_npcName, _dialogueText, questGiver, animator , _npcGreetingVoice);
        }
    }
}
