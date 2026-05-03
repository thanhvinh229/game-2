using UnityEngine;
using TMPro;    
using UnityEngine.UI;
using System.Collections.Generic;

public class NPCDialogueConfig
{
    public string DefaultQuestButtonPrefix  = "Hỏi về: ";
    public string DefaultDetailText         = "Tôi đang gặp rắc rối, bạn có thể giúp tôi xử lý việc này không?";
    public string DefaultAcceptButtonLabel  = "Tôi sẽ giúp!";
    public string DefaultAcceptedText       = "Tuyệt vời! Cảm ơn bạn rất nhiều.";
    public string DefaultDeclineButtonLabel = "Để tôi suy nghĩ đã...";
    public string FarewellButtonLabel       = "Tạm biệt";
    public List<QuestDialogueConfig> QuestOverrides = new List<QuestDialogueConfig>();
 
    /// <summary>Trả về config riêng của quest nếu có, ngược lại trả về null.</summary>
    public QuestDialogueConfig GetOverrideFor(string questId)
    {
        return QuestOverrides?.Find(q => q.questId == questId);
    }
}
 
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
 
    [Range(0f, 1f)] [SerializeField] private float _clickVolume = 0.5f;
 
    private Animator _currentNpcAnimator;
    private NPCDialogueConfig _currentConfig;
 
    private void Awake()
    {
        Instance = this;
        _dialoguePanel.SetActive(false);
    }
 
    public void OpenDialogue(
        string speakerName,
        string greeting,
        QuestHolder questHolder,
        NPCDialogueConfig config,
        Animator npcAnimator = null,
        AudioClip greetingSound = null)
    {
        _currentConfig = config ?? new NPCDialogueConfig();
 
        _dialoguePanel.SetActive(true);
        _nameText.text = speakerName;
        _contentText.text = greeting;
 
        _currentNpcAnimator = npcAnimator;
        if (_currentNpcAnimator != null)
            _currentNpcAnimator.SetBool("IsTalking", true);
 
        if (_audioSource != null && greetingSound != null)
            _audioSource.PlayOneShot(greetingSound);
 
        RefreshQuestChoices(speakerName, questHolder);
 
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
 
    private void RefreshQuestChoices(string speakerName, QuestHolder questHolder)
    {
        ClearChoices();
 
        foreach (var quest in questHolder.GetAvailableQuests())
        {
            var over = _currentConfig.GetOverrideFor(quest.Id);
            string btnLabel = over != null
                ? over.choiceButtonLabel
                : $"{_currentConfig.DefaultQuestButtonPrefix}{quest.Id}";
 
            QuestData capturedQuest = quest; // tránh closure trap
            CreateChoiceButton(btnLabel, () => ShowQuestDetail(speakerName, capturedQuest, questHolder));
        }
 
        CreateChoiceButton(_currentConfig.FarewellButtonLabel, CloseDialogue);
    }
 
    private void ShowQuestDetail(string speakerName, QuestData quest, QuestHolder questHolder)
    {
        ClearChoices();
 
        var over = _currentConfig.GetOverrideFor(quest.Id);
 
        string detailText  = over?.detailText         ?? _currentConfig.DefaultDetailText;
        string acceptLabel = over?.acceptButtonLabel   ?? _currentConfig.DefaultAcceptButtonLabel;
        string acceptedMsg = over?.acceptedText        ?? _currentConfig.DefaultAcceptedText;
        string declineLabel= over?.declineButtonLabel  ?? _currentConfig.DefaultDeclineButtonLabel;
 
        _contentText.text = detailText;
 
        CreateChoiceButton(acceptLabel, () =>
        {
            questHolder.GiveSpecificQuest(quest);
            _contentText.text = acceptedMsg;
            RefreshQuestChoices(speakerName, questHolder);
        });
 
        CreateChoiceButton(declineLabel, CloseDialogue);
    }
 
    private void CreateChoiceButton(string text, UnityEngine.Events.UnityAction action)
    {
        GameObject btnObj = Instantiate(_choiceButtonPrefab, _choiceContainer);
        btnObj.GetComponentInChildren<TextMeshProUGUI>().text = text;
        btnObj.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (_audioSource != null && _clickSound != null)
                _audioSource.PlayOneShot(_clickSound, _clickVolume);
            action();
        });
    }
 
    private void ClearChoices()
    {
        foreach (Transform child in _choiceContainer)
            Destroy(child.gameObject);
    }
 
    public void CloseDialogue()
    {
        _dialoguePanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
 
        if (_currentNpcAnimator != null)
        {
            _currentNpcAnimator.SetBool("IsTalking", false);
            _currentNpcAnimator = null;
        }
    }
}