using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class QuestDialogueConfig
{
    [Tooltip("ID của quest cần ghi đè lời thoại (phải khớp với QuestData.Id)")]
    public string questId;
 
    [Tooltip("Nhãn nút hiển thị trong danh sách chọn quest")]
    public string choiceButtonLabel = "Hỏi về nhiệm vụ...";
 
    [Tooltip("Lời thoại NPC khi người chơi hỏi về quest này")]
    [TextArea(2, 5)]
    public string detailText = "Tôi đang gặp rắc rối, bạn có thể giúp tôi xử lý việc này không?";
 
    [Tooltip("Nhãn nút đồng ý nhận quest")]
    public string acceptButtonLabel = "Tôi sẽ giúp!";
 
    [Tooltip("Lời thoại NPC khi người chơi đồng ý nhận quest")]
    [TextArea(2, 5)]
    public string acceptedText = "Tuyệt vời! Cảm ơn bạn rất nhiều.";
 
    [Tooltip("Nhãn nút từ chối / suy nghĩ lại")]
    public string declineButtonLabel = "Để tôi suy nghĩ đã...";
}
 
[CreateAssetMenu(fileName = "TalkAction", menuName = "Scriptable Objects/TalkAction")]
public class TalkAction : InteractionActionObject
{
    [Header("NPC Info")]
    [SerializeField] private string _npcName = "NPC";
 
    [Header("Dialogue - Chào hỏi")]
    [SerializeField, TextArea(2, 5)] private string _greetingText = "Tôi có vài việc cần bạn giúp...";
 
    [Header("Dialogue - Nút & Văn bản chung")]
    [Tooltip("Tiền tố nút chọn quest khi KHÔNG có cấu hình riêng")]
    [SerializeField] private string _defaultQuestButtonPrefix = "Hỏi về: ";
 
    [Tooltip("Lời thoại chi tiết quest mặc định khi KHÔNG có cấu hình riêng")]
    [SerializeField, TextArea(2, 5)] private string _defaultDetailText = "Tôi đang gặp rắc rối, bạn có thể giúp tôi xử lý việc này không?";
 
    [Tooltip("Nhãn nút chấp nhận quest mặc định")]
    [SerializeField] private string _defaultAcceptButtonLabel = "Tôi sẽ giúp!";
 
    [Tooltip("Lời NPC sau khi quest được nhận, mặc định")]
    [SerializeField, TextArea(2, 5)] private string _defaultAcceptedText = "Tuyệt vời! Cảm ơn bạn rất nhiều.";
 
    [Tooltip("Nhãn nút từ chối mặc định")]
    [SerializeField] private string _defaultDeclineButtonLabel = "Để tôi suy nghĩ đã...";
 
    [Tooltip("Nhãn nút tạm biệt")]
    [SerializeField] private string _farewellButtonLabel = "Tạm biệt";
 
    [Header("Dialogue - Ghi đè theo từng Quest")]
    [Tooltip("Thêm vào đây để tuỳ chỉnh lời thoại cho từng quest riêng biệt")]
    [SerializeField] private List<QuestDialogueConfig> _questDialogues = new List<QuestDialogueConfig>();
 
    [Header("Audio")]
    [SerializeField] private AudioClip _npcGreetingVoice;
 
    public override void OnInteract(InteractionContext context)
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogError("LỖI: DialogueManager chưa được khởi tạo!");
            return;
        }
 
        var interactable = context.Interactable;
        var questGiver = interactable.GetComponent<QuestHolder>();
        var animator = interactable.GetComponent<Animator>();
 
        if (questGiver == null) return;
 
        var config = BuildDialogueConfig();
        DialogueManager.Instance.OpenDialogue(_npcName, _greetingText, questGiver, config, animator, _npcGreetingVoice);
    }
 
    private NPCDialogueConfig BuildDialogueConfig()
    {
        return new NPCDialogueConfig
        {
            DefaultQuestButtonPrefix = _defaultQuestButtonPrefix,
            DefaultDetailText        = _defaultDetailText,
            DefaultAcceptButtonLabel = _defaultAcceptButtonLabel,
            DefaultAcceptedText      = _defaultAcceptedText,
            DefaultDeclineButtonLabel= _defaultDeclineButtonLabel,
            FarewellButtonLabel      = _farewellButtonLabel,
            QuestOverrides           = _questDialogues
        };
    }
}