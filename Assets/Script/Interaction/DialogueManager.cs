using UnityEngine;
using TMPro; // Sử dụng TextMeshPro cho UI sắc nét
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private GameObject _dialoguePanel; // Kéo bảng UI vào đây
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _contentText;
    [SerializeField] private Transform _choiceContainer;
    [SerializeField] private GameObject _choiceButtonPrefab; // Kéo Prefab nút đã tạo ở Bước 1 vào đây

    private void Awake()
    {
        Instance = this;
        _dialoguePanel.SetActive(false); // Đảm bảo lúc đầu game luôn đóng
    }

    public void OpenDialogue(string speakerName, string message, QuestHolder questHolder)
    {
        _dialoguePanel.SetActive(true); // Chỉ hiện khi tương tác
        _nameText.text = speakerName;
        _contentText.text = message;

        // Xóa nút cũ
        foreach (Transform child in _choiceContainer) Destroy(child.gameObject);

        // Tạo nút cho mỗi Quest
        foreach (var quest in questHolder.GetAvailableQuests())
        {
            CreateChoiceButton($"Nhận: {quest.Id}", () => {
                questHolder.GiveSpecificQuest(quest);
                CloseDialogue();
            });
        }

        // Nút đóng hội thoại
        CreateChoiceButton("Tạm biệt", CloseDialogue);
        
        // Hiện chuột để người chơi chọn
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void CreateChoiceButton(string text, UnityEngine.Events.UnityAction action)
    {
        GameObject btnObj = Instantiate(_choiceButtonPrefab, _choiceContainer);
        btnObj.GetComponentInChildren<TextMeshProUGUI>().text = text;
        btnObj.GetComponent<Button>().onClick.AddListener(action);
    }

    public void CloseDialogue()
    {
        _dialoguePanel.SetActive(false);
        // Ẩn chuột lại sau khi nói chuyện (tùy vào logic game của bạn)
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
