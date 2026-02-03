using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestLogPanelUI : MonoBehaviour
{
    [SerializeField] private QuestEventChannel _questEventChanel;
    [SerializeField] private Transform _content;
    [SerializeField] private GameObject _questItemPrefab;

    private Dictionary<string, QuestItemUI> _questItemCollection = new();

     void Awake()
    {
        _questEventChanel.OnReceivedQuest += OnReceivedQuest;
        _questEventChanel.OnCompleteQuest += OnCompleteQuest;
    }


    private void OnReceivedQuest(string questId)
    {

        if (!_questItemCollection.TryGetValue(questId, out QuestItemUI questItemUI))
        {
            var elementGO = Instantiate(_questItemPrefab, _content);
            questItemUI = elementGO.GetComponent<QuestItemUI>();
            questItemUI.Initialize(name: questId, isCompleted: false);
            _questItemCollection.Add(questId, questItemUI);
        }



    }
    private void OnCompleteQuest(string questId)
    {
        if(_questItemCollection.TryGetValue(questId,out QuestItemUI questItemUI))
        {
            questItemUI.UpdateStatus(true);
        }
    }
        
   
}
