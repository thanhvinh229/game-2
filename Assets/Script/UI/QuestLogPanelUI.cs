using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestLogPanelUI : MonoBehaviour
{
    [Header("Left panel")]
    [SerializeField] private QuestEventChannel _questEventChannel;
    [SerializeField] private Transform         _questListContent;
    [SerializeField] private GameObject        _questEntryPrefab;
 
    [Header("Right panel")]
    [SerializeField] private QuestDetailUI _questDetail;
 
    private Dictionary<string, QuestEntryUI> _entries = new();
    private QuestEntryUI _selectedEntry;
 
    void Awake()
    {
        _questEventChannel.OnReceivedQuest     += OnReceivedQuest;
        _questEventChannel.OnCompleteQuest     += OnCompleteQuest;
        _questEventChannel.OnObjectiveProgress += OnObjectiveProgress;
    }
 
    void Start()
    {
        SyncExistingQuests();
    }
 
    void OnDestroy()
    {
        _questEventChannel.OnReceivedQuest     -= OnReceivedQuest;
        _questEventChannel.OnCompleteQuest     -= OnCompleteQuest;
        _questEventChannel.OnObjectiveProgress -= OnObjectiveProgress;
    }
 
    private void SyncExistingQuests()
    {
        var activeQuests = QuestManager.Instance.QuestLog.ActiveQuests;
        if (activeQuests == null || activeQuests.Count == 0) return;
 
        foreach (var quest in activeQuests)
            if (!_entries.ContainsKey(quest.Data.Id))
                CreateQuestEntryUI(quest);
    }
 
    private void OnReceivedQuest(string questId)
    {
        if (_entries.ContainsKey(questId)) return;
 
        var quest = QuestManager.Instance.QuestLog.GetQuestById(questId);
        if (quest == null) return;
 
        CreateQuestEntryUI(quest);
 
        if (_selectedEntry == null)
            OnQuestSelected(quest.Data);
    }
 
    private void CreateQuestEntryUI(Quest logicQuest)
    {
        var go    = Instantiate(_questEntryPrefab, _questListContent);
        var entry = go.GetComponent<QuestEntryUI>();
        entry.Initialize(logicQuest.Data, OnQuestSelected);
        _entries.Add(logicQuest.Data.Id, entry);
 
        if (_selectedEntry == null)
            OnQuestSelected(logicQuest.Data);
    }
 
    private void OnCompleteQuest(string questId)
    {
        if (_entries.TryGetValue(questId, out var entry))
            entry.RefreshStatus();
 
        // Nếu quest vừa hoàn thành đang được select → refresh detail
        if (_selectedEntry != null &&
            _entries.TryGetValue(questId, out var sel) &&
            sel == _selectedEntry)
        {
            var quest = QuestManager.Instance.QuestLog.GetQuestById(questId);
            _questDetail.Show(quest?.Data, quest);
        }
    }
 
    private void OnObjectiveProgress(string questId, string objectiveId, int current, int required)
    {
        // Cập nhật real-time khi đang xem đúng quest đó
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
 
        // Truyền Quest runtime để đọc CurrentKills thật
        var questRuntime = QuestManager.Instance.QuestLog.GetQuestById(data.Id);
        _questDetail.Show(data, questRuntime);
    }
}
