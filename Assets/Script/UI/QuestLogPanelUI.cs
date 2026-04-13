using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestLogPanelUI : MonoBehaviour
{
    [Header("Left panel")]
    [SerializeField] private QuestEventChannel _questEventChannel;
    [SerializeField] private Transform _questListContent;
    [SerializeField] private GameObject _questEntryPrefab;  // prefab có QuestEntryUI script
 
    [Header("Right panel")]
    [SerializeField] private QuestDetailUI _questDetail;
 
    private Dictionary<string, QuestEntryUI> _entries = new();
    private QuestEntryUI _selectedEntry;
 
    void Awake()
    {
        _questEventChannel.OnReceivedQuest += OnReceivedQuest;
        _questEventChannel.OnCompleteQuest += OnCompleteQuest;
        _questEventChannel.OnObjectiveProgress += OnObjectiveProgress;
    }
    void Start()
    {
        // Khi UI lần đầu tiên được tạo/bật lên, hãy đồng bộ những quest ĐÃ NHẬN 
        // từ trước khi Awake() kịp lắng nghe event.
        SyncExistingQuests();
    }
 
    void OnDestroy()
    {
        _questEventChannel.OnReceivedQuest -= OnReceivedQuest;
        _questEventChannel.OnCompleteQuest -= OnCompleteQuest;
        _questEventChannel.OnObjectiveProgress -= OnObjectiveProgress;
    }
    private void SyncExistingQuests()
    {
        // Gọi trực tiếp đến property ActiveQuests từ QuestLog của bạn
        var activeQuests = QuestManager.Instance.QuestLog.ActiveQuests; 
        
        if (activeQuests == null || activeQuests.Count == 0) return;

        foreach (var quest in activeQuests)
        {
            // quest.Data.Id dựa theo cấu trúc class Quest của bạn
            if (!_entries.ContainsKey(quest.Data.Id)) 
            {
                CreateQuestEntryUI(quest);
            }
        }
    }
 
    private void OnReceivedQuest(string questId)
    {
        if (_entries.ContainsKey(questId)) return;
 
        var quest = QuestManager.Instance.QuestLog.GetQuestById(questId);
        if (quest == null) return;
 
        var go = Instantiate(_questEntryPrefab, _questListContent);
        var entry = go.GetComponent<QuestEntryUI>();
        entry.Initialize(quest.Data, OnQuestSelected);
        _entries.Add(questId, entry);
 
        // Auto-select quest đầu tiên
        if (_selectedEntry == null)
            OnQuestSelected(quest.Data);
    }
    private void CreateQuestEntryUI(Quest logicQuest)
    {
        var go = Instantiate(_questEntryPrefab, _questListContent);
        var entry = go.GetComponent<QuestEntryUI>();
        
        entry.Initialize(logicQuest.Data, OnQuestSelected);
        _entries.Add(logicQuest.Data.Id, entry);
 
        // Auto-select quest đầu tiên
        if (_selectedEntry == null)
            OnQuestSelected(logicQuest.Data);
    }
 
    private void OnCompleteQuest(string questId)
    {
        if (_entries.TryGetValue(questId, out var entry))
            entry.RefreshStatus();
    }
 
    private void OnObjectiveProgress(string questId, string objectiveId, int current, int required)
    {
        if (_selectedEntry != null &&
            _entries.TryGetValue(questId, out var entry) &&
            entry == _selectedEntry)
        {
            _questDetail.UpdateObjectiveProgress(objectiveId, current, required);
        }
    }
 
    private void OnQuestSelected(QuestData data)
    {
        if (_selectedEntry != null)
            _selectedEntry.SetSelected(false);
 
        if (_entries.TryGetValue(data.Id, out var newEntry))
        {
            newEntry.SetSelected(true);
            _selectedEntry = newEntry;
        }
 
        _questDetail.Show(data);
    }
}
