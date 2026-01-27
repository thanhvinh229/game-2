using System;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private static QuestManager _instance;

    public static QuestManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject().AddComponent<QuestManager>();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    public QuestEventChannel _questEventChannel;

    private QuestLog _questLog;

    

    void Awake()
    {
        _questEventChannel.OnReceivedQuest += OnReceivedQuest;
    }

   
    private void OnReceivedQuest(QuestData questData)
    {
        var newQuest = new Quest(questData);
        _questLog.AddNewQuest(newQuest);
        StartQuest(newQuest.Data.Id);
    }

    public void StartQuest(String questId)
    {
        var quest = _questLog.GetQuestById(questId);
        quest?.Start();
    }

}

