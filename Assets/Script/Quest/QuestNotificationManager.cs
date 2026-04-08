using UnityEngine;

public class QuestNotificationManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private QuestEventChannel _questEventChannel;
    [SerializeField] private QuestNotificationUI _notificationUI;
 
    [Header("Settings")]
    [SerializeField] private bool _showOnQuestReceived = true;
    [SerializeField] private bool _showOnQuestCompleted = true;
    [SerializeField] private bool _showOnObjectiveCompleted = true;
 
    void Awake()
    {
        if (_questEventChannel == null)
        {
            Debug.LogError("QuestNotificationManager: QuestEventChannel not assigned!");
            return;
        }
 
        // Subscribe to quest events
        _questEventChannel.OnReceivedQuest += OnQuestReceived;
        _questEventChannel.OnCompleteQuest += OnQuestCompleted;
        _questEventChannel.OnObjectiveProgress += OnObjectiveProgress;
    }
 
    void OnDestroy()
    {
        if (_questEventChannel != null)
        {
            _questEventChannel.OnReceivedQuest -= OnQuestReceived;
            _questEventChannel.OnCompleteQuest -= OnQuestCompleted;
            _questEventChannel.OnObjectiveProgress -= OnObjectiveProgress;
        }
    }
 
    private void OnQuestReceived(string questId)
    {
        if (!_showOnQuestReceived || _notificationUI == null) return;
 
        var quest = QuestManager.Instance?.QuestLog?.GetQuestById(questId);
        if (quest != null)
        {
            _notificationUI.ShowQuestReceived(quest.Data.Id);
            Debug.Log($"📬 Quest Received Notification: {quest.Data.Id}");
        }
    }
 
    private void OnQuestCompleted(string questId)
    {
        if (!_showOnQuestCompleted || _notificationUI == null) return;
 
        var quest = QuestManager.Instance?.QuestLog?.GetQuestById(questId);
        if (quest != null)
        {
            _notificationUI.ShowQuestCompleted(quest.Data.Id);
            Debug.Log($"✅ Quest Completed Notification: {quest.Data.Id}");
        }
    }
 
    private void OnObjectiveProgress(string questId, string objectiveId, int current, int required)
    {
        // Only show notification when objective is completed
        if (!_showOnObjectiveCompleted || _notificationUI == null) return;
        if (current < required) return;
 
        var quest = QuestManager.Instance?.QuestLog?.GetQuestById(questId);
        if (quest != null)
        {
            var objective = quest.Data.ObjectiveData.Find(obj => obj.Id == objectiveId);
            if (objective != null)
            {
                _notificationUI.ShowObjectiveCompleted(objective.Description);
                Debug.Log($"🎯 Objective Completed Notification: {objective.Description}");
            }
        }
    }
 
    #if UNITY_EDITOR
    void OnValidate()
    {
        // Auto-find QuestEventChannel if not assigned
        if (_questEventChannel == null)
        {
            _questEventChannel = FindFirstObjectByType<QuestEventChannel>();
        }
 
        // Auto-find QuestNotificationUI if not assigned
        if (_notificationUI == null)
        {
            _notificationUI = FindFirstObjectByType<QuestNotificationUI>();
        }
    }
    #endif
}
