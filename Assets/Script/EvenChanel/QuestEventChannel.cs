using System;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestEventChannel", menuName = "Scriptable Objects/QuestEventChannel")]
public class QuestEventChannel : ScriptableObject
{
    public Action<string> OnReceivedQuest;
    public Action<string> OnStartQuest;
    public Action<string> OnCompleteQuest;
    public Action<string> OnCollectItem;
    public Action<string> OnDeliverItem;
 
    // questId, objectiveId, current, required
    public Action<string, string, int, int> OnObjectiveProgress;
}
