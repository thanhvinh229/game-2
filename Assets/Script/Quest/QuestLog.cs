using System.Collections.Generic;
using UnityEngine;

public class QuestLog 
{
    private Dictionary<string, Quest> _activeQuests = new();
    private Dictionary<string, Quest> _completedQuests = new();
    public IReadOnlyCollection<Quest>  ActiveQuests => _activeQuests.Values;

    public void AddNewQuest(Quest quest)
    {
        if (!_activeQuests.ContainsKey(quest.Data.Id))
        {
            _activeQuests.Add(quest.Data.Id, quest);
        }
    }
    public Quest GetQuestById(string questId)
    {
        if(_activeQuests.TryGetValue(questId,out Quest quest))
        {
            return quest;
        }
       return null;
    }
    public bool CompleteQuest(string questId)
    {
        if (_activeQuests.TryGetValue(questId, out Quest quest))
        {
            quest.Complete();
            _completedQuests.Add(questId, quest);
            return true;    
        }
        return false;
    }
}
