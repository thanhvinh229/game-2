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
        DontDestroyOnLoad(_instance.gameObject);
    }

   
    public void ReceivedQuest(QuestData questData)
    {
        var newQuest = new Quest(questData);
        QuestLog.AddNewQuest(newQuest);
        StartQuest(newQuest.Data.Id);
        _questEventChannel.OnReceivedQuest?.Invoke(questData.Id);
    }

    public void StartQuest(String questId)
    {
        var quest = QuestLog.GetQuestById(questId);
        quest?.Start();
        _questEventChannel.OnStartQuest?.Invoke(questId);
    }

}

