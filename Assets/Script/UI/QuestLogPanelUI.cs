using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestLogPanelUI : MonoBehaviour
{
    [SerializeField] private QuestEventChannel _questEventChannel;
    [SerializeField] private Transform _content;
    [SerializeField] private GameObject _questItemPrefab;
 
    private Dictionary<string, QuestItemUI> _questItemCollection = new();
 
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
        if (_questItemCollection.ContainsKey(questId)) return;
 
        var quest = QuestManager.Instance.QuestLog.GetQuestById(questId);
        if (quest == null) return;
 
        var go = Instantiate(_questItemPrefab, _content);
        var questItemUI = go.GetComponent<QuestItemUI>();
        questItemUI.Initialize(quest.Data);
        _questItemCollection.Add(questId, questItemUI);
    }
 
    private void OnCompleteQuest(string questId)
    {
        if (_questItemCollection.TryGetValue(questId, out var questItemUI))
            questItemUI.UpdateStatus(isCompleted: true);
    }
 
    private void OnObjectiveProgress(string questId, string objectiveId, int current, int required)
    {
        if (_questItemCollection.TryGetValue(questId, out var questItemUI))
            questItemUI.UpdateObjectiveProgress(objectiveId, current, required);
    }
}
