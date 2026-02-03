using System;
using UnityEngine;

public class CollectObjective :Objective
{
    CollectObjectiveData _collectData;
    int _currentData;
    public override bool IsCompleted => _currentData >= _collectData.RequiredAmount;


    public CollectObjective(CollectObjectiveData data) : base(data)
    {
        _collectData = data;
    }

    public override void Register()
    {
        Debug.Log($"Registed objective {_data.Id}");
        _collectData.Status = QuestStatus.Active;
        _collectData.EventChannel.OnTarget += OnTarget; 
    }

    

    public override void Unregister()
    {
        Debug.Log($"Unregisted objective {_data.Id}");
        _collectData.Status = QuestStatus.Completed;
        _collectData.EventChannel.OnTarget += OnTarget;

    }

    private void OnTarget()
    {
        _currentData++;
    }
}
