using System;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private static QuestManager _instance;
    public static QuestManager Instance => _instance;
 
    public QuestLog QuestLog = new();
    [SerializeField] private QuestEventChannel _questEventChannel;
 
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
 
    public void ReceivedQuest(QuestData questData)
    {
        // Không nhận quest đã active hoặc đã hoàn thành
        if (QuestLog.IsQuestActive(questData.Id) || QuestLog.IsQuestCompleted(questData.Id))
        {
            Debug.Log($"Quest {questData.Id} already received.");
            return;
        }
 
        var newQuest = new Quest(questData);
        newQuest.OnQuestCompleted += HandleQuestCompleted;
 
        QuestLog.AddNewQuest(newQuest);
        _questEventChannel.OnReceivedQuest?.Invoke(questData.Id);
 
        newQuest.Start();
        _questEventChannel.OnStartQuest?.Invoke(questData.Id);
    }
 
    private void HandleQuestCompleted(Quest quest)
    {
        QuestLog.CompleteQuest(quest.Data.Id);
        _questEventChannel.OnCompleteQuest?.Invoke(quest.Data.Id);
        Debug.Log($"Quest {quest.Data.Id} completed!");
    }
}

