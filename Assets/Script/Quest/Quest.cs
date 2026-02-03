using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Quest : MonoBehaviour
{
   public QuestData Data;

    public List<Objective> Objectives = new();

    public bool IsCompleted => Objectives.All(x=> x.IsCompleted);

    public Quest(QuestData data)
    {
        Data = data;
        Objectives = data.ObjectiveData.Select(x => x.CreateInstance()).ToList();
    }

    public void Start()
    {
        Debug.Log($"St  art quest {Data.Id}");
        Data.Status = QuestStatus.Active;
        foreach (var obj in Objectives)
        {
            obj.Register();
        }
    }
    public void Complete()
    {
        Data.Status = IsCompleted ? QuestStatus.Completed : QuestStatus.Failed;
        foreach (var obj in Objectives)
        {
            obj.Unregister();
        }
    }

    
}
