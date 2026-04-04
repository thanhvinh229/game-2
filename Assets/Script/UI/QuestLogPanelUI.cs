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
 
    void OnDestroy()
    {
        _questEventChannel.OnReceivedQuest -= OnReceivedQuest;
        _questEventChannel.OnCompleteQuest -= OnCompleteQuest;
        _questEventChannel.OnObjectiveProgress -= OnObjectiveProgress;
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
