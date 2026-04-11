using UnityEngine;
using TMPro;    
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _contentText;
    [SerializeField] private Transform _choiceContainer;
    [SerializeField] private GameObject _choiceButtonPrefab;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource _audioSource; 
    [SerializeField] private AudioClip _clickSound;

    // Biến lưu trữ Animator của NPC đang nói chuyện
    private Animator _currentNpcAnimator; 

    private void Awake() { Instance = this; _dialoguePanel.SetActive(false); }

    // Thêm tham số Animator vào hàm Open
    public void OpenDialogue(string speakerName, string greeting, QuestHolder questHolder, Animator npcAnimator = null, AudioClip greetingSound = null)
    {
        _dialoguePanel.SetActive(true);
        _nameText.text = speakerName;
        _contentText.text = greeting;

        // Lưu và bật Animation
        _currentNpcAnimator = npcAnimator;
        if (_currentNpcAnimator != null)
        {
            _currentNpcAnimator.SetBool("IsTalking", true); // Kích hoạt trạng thái nói chuyện
        }
        // PHÁT ÂM THANH NPC CHÀO HỎI
        if (_audioSource != null && greetingSound != null)
        {
            _audioSource.PlayOneShot(greetingSound);
        }

        RefreshQuestChoices(speakerName, questHolder);
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void RefreshQuestChoices(string speakerName, QuestHolder questHolder)
    {
        ClearChoices();

        foreach (var quest in questHolder.GetAvailableQuests())
        {
            CreateChoiceButton($"Hỏi về: {quest.Id}", () => {
                ShowQuestDetail(speakerName, quest, questHolder);
            });
        }

        CreateChoiceButton("Tạm biệt", CloseDialogue);
    }

    private void ShowQuestDetail(string speakerName, QuestData quest, QuestHolder questHolder)
    {
        ClearChoices();
        
        _contentText.text = $"[Về {quest.Id}]: Tôi đang gặp rắc rối, bạn có thể giúp tôi xử lý việc này không?";

        CreateChoiceButton("Tôi sẽ giúp!", () => {
            questHolder.GiveSpecificQuest(quest);
            _contentText.text = "Tuyệt vời! Cảm ơn bạn rất nhiều.";
            RefreshQuestChoices(speakerName, questHolder); 
        });

        // ĐÃ SỬA: Thay vì quay lại danh sách, giờ nó sẽ gọi CloseDialogue để thoát luôn
        CreateChoiceButton("Để tôi suy nghĩ đã...", CloseDialogue); 
    }

    private void CreateChoiceButton(string text, UnityEngine.Events.UnityAction action)
    {
        GameObject btnObj = Instantiate(_choiceButtonPrefab, _choiceContainer);
        btnObj.GetComponentInChildren<TextMeshProUGUI>().text = text;
        btnObj.GetComponent<Button>().onClick.AddListener(action);
        if (_audioSource != null && _clickSound != null)
            {
                _audioSource.PlayOneShot(_clickSound); 
            }
           
    }

    private void ClearChoices() { foreach (Transform child in _choiceContainer) Destroy(child.gameObject); }

    public void CloseDialogue() 
    { 
        _dialoguePanel.SetActive(false); 
        Cursor.visible = false; 
        Cursor.lockState = CursorLockMode.Locked; 

        // Tắt Animation khi kết thúc trò chuyện
        if (_currentNpcAnimator != null)
        {
            _currentNpcAnimator.SetBool("IsTalking", false);
            _currentNpcAnimator = null; // Xóa cache
        }
    }
}