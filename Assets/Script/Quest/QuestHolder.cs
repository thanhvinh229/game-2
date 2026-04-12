using UnityEngine;
using System.Collections.Generic;
public class QuestHolder : MonoBehaviour
{
    [SerializeField] private List<QuestData> _quests = new();
 
    [Header("Auto Start Settings")]
    [Tooltip("Tự động give tất cả quests khi game start?")]
    [SerializeField] private bool _autoGiveQuestsOnStart = false;
    
    [Tooltip("Delay giữa mỗi quest (giây) khi auto-give")]
    [SerializeField] private float _delayBetweenQuests = 0.5f;
 
    private int _currentQuestIndex = 0;
 
    void Start()
    {
        if (_autoGiveQuestsOnStart)
        {
            StartCoroutine(AutoGiveQuestsCoroutine());
        }
    }
 
    private System.Collections.IEnumerator AutoGiveQuestsCoroutine()
    {
        // Wait 1 frame để đảm bảo QuestManager đã init
        yield return null;
 
        foreach (var questData in _quests)
        {
            bool alreadyActive = QuestManager.Instance.QuestLog.IsQuestActive(questData.Id);
            bool alreadyDone = QuestManager.Instance.QuestLog.IsQuestCompleted(questData.Id);
 
            if (!alreadyActive && !alreadyDone)
            {
                QuestManager.Instance.ReceivedQuest(questData);
                Debug.Log($"[Auto-Start] Đã nhận quest: {questData.Description}");
                
                // Delay trước khi give quest tiếp theo
                if (_delayBetweenQuests > 0)
                {
                    yield return new WaitForSeconds(_delayBetweenQuests);
                }
            }
        }
 
        Debug.Log("[Auto-Start] Đã give tất cả quests!");
    }
 
    /// <summary>
    /// Give quest tiếp theo (dùng cho NPC interaction)
    /// </summary>
    public void GiveQuest()
    {
        // Tìm quest tiếp theo chưa được nhận hoặc hoàn thành
        while (_currentQuestIndex < _quests.Count)
        {
            var questData = _quests[_currentQuestIndex];
            bool alreadyActive = QuestManager.Instance.QuestLog.IsQuestActive(questData.Id);
            bool alreadyDone = QuestManager.Instance.QuestLog.IsQuestCompleted(questData.Id);
 
            if (!alreadyActive && !alreadyDone)
            {
                QuestManager.Instance.ReceivedQuest(questData);
                return;
            }
 
            _currentQuestIndex++;
        }
 
        Debug.Log($"{gameObject.name} has no more quests to give.");
    }
 
    /// <summary>
    /// Get danh sách quests có thể nhận
    /// </summary>
    public List<QuestData> GetAvailableQuests()
    {
        List<QuestData> availableQuests = new List<QuestData>();
        
        foreach (var questData in _quests)
        {
            bool alreadyActive = QuestManager.Instance.QuestLog.IsQuestActive(questData.Id);
            bool alreadyDone = QuestManager.Instance.QuestLog.IsQuestCompleted(questData.Id);
 
            if (!alreadyActive && !alreadyDone)
            {
                availableQuests.Add(questData);
            }
        }
        
        return availableQuests;
    }
 
    /// <summary>
    /// Give quest cụ thể (dùng cho UI button)
    /// </summary>
    public void GiveSpecificQuest(QuestData quest)
    {
        QuestManager.Instance.ReceivedQuest(quest);
        Debug.Log($"Đã nhận nhiệm vụ: {quest.Id}");
    }
 
    /// <summary>
    /// Give tất cả quests còn lại (debug/testing)
    /// </summary>
    public void GiveAllQuests()
    {
        foreach (var questData in _quests)
        {
            bool alreadyActive = QuestManager.Instance.QuestLog.IsQuestActive(questData.Id);
            bool alreadyDone = QuestManager.Instance.QuestLog.IsQuestCompleted(questData.Id);
 
            if (!alreadyActive && !alreadyDone)
            {
                QuestManager.Instance.ReceivedQuest(questData);
            }
        }
    }
}

