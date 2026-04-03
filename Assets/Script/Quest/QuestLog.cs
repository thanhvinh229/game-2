using System.Collections.Generic;
using UnityEngine;

public class QuestLog 
{
    private Dictionary<string, Quest> _activeQuests = new();
    private Dictionary<string, Quest> _completedQuests = new();
 
    public IReadOnlyCollection<Quest> ActiveQuests => _activeQuests.Values;
    public IReadOnlyCollection<Quest> CompletedQuests => _completedQuests.Values;
 
    public void AddNewQuest(Quest quest)
    {
        if (!_activeQuests.ContainsKey(quest.Data.Id))
            _activeQuests.Add(quest.Data.Id, quest);
    }
 
    public Quest GetQuestById(string questId)
    {
        _activeQuests.TryGetValue(questId, out Quest quest);
        return quest;
    }
 
    public bool CompleteQuest(string questId)
    {
        if (_activeQuests.TryGetValue(questId, out Quest quest))
        {
            _activeQuests.Remove(questId); // fix: thiếu dòng này
            _completedQuests.Add(questId, quest);
            return true;
        }
        return false;
    }
 
    public bool IsQuestCompleted(string questId) => _completedQuests.ContainsKey(questId);
 
    public bool IsQuestActive(string questId) => _activeQuests.ContainsKey(questId);
}