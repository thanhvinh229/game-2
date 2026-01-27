using System.Collections.Generic;
using UnityEngine;

public class QuestLog : MonoBehaviour
{
    private Dictionary<string, Quest> _activeQuests = new();
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
    public void CompleteQuest(string questId)
    {
        if (_activeQuests.TryGetValue(questId, out Quest quest))
        {
            quest.Complete();
        }
    }
}
