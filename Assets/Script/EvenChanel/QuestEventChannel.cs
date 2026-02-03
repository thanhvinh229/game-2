using System;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestEventChannel", menuName = "Scriptable Objects/QuestEventChannel")]
public class QuestEventChannel : ScriptableObject
{
    public Action OnTarget;
    public Action<string> OnReceivedQuest;
    public Action<string> OnCompleteQuest;
    public Action<string> OnStartQuest;
    public Action<string> OnCollectItem;
    public Action<string> OnDeliverItem;
}
