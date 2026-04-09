using UnityEngine;
using System.Collections.Generic;
public class QuestHolder : MonoBehaviour
{
    [SerializeField] private List<QuestData> _quests = new();
 
    private int _currentQuestIndex = 0;
 
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

    // Hàm này sẽ được gọi bởi Nút bấm (Button) trên UI Dialogue
    public void GiveSpecificQuest(QuestData quest)
    {
        QuestManager.Instance.ReceivedQuest(quest);
        Debug.Log($"Đã nhận nhiệm vụ: {quest.Id}");
    }
}
