using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Quest 
{
    public QuestData Data;
    public List<Objective> Objectives = new();
    public bool IsCompleted => Objectives.All(x => x.IsCompleted);
 
    public event Action<Quest> OnQuestCompleted;
 
    public Quest(QuestData data)
    {
        Data = data;
        // Gán QuestId cho mỗi ObjectiveData trước khi tạo instance
        foreach (var objData in data.ObjectiveData)
            objData.QuestId = data.Id;
        Objectives = data.ObjectiveData.Select(x => x.CreateInstance()).ToList();
    }
 
    public void Start()
    {
        Debug.Log($"Start quest {Data.Id}");
        Data.Status = QuestStatus.Active;
        foreach (var obj in Objectives)
        {
            obj.OnProgressChanged += CheckCompletion;
            obj.Register();
        }
    }
 
    private void CheckCompletion()
    {
        if (IsCompleted)
            Complete();
    }
 
    public void Complete()
    {
        Debug.Log($"Complete quest {Data.Id}");
        Data.Status = QuestStatus.Completed;
        foreach (var obj in Objectives)
        {
            obj.OnProgressChanged -= CheckCompletion;
            obj.Unregister();
        }
        OnQuestCompleted?.Invoke(this);
    }
}
