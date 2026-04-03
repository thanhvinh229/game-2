using System;
using UnityEngine;

public class CollectObjective :Objective
{
    private CollectObjectiveData _collectData;
    private int _currentAmount;
 
    public override bool IsCompleted => _currentAmount >= _collectData.RequiredAmount;
    public int CurrentAmount => _currentAmount;
 
    public CollectObjective(CollectObjectiveData data) : base(data)
    {
        _collectData = data;
    }
 
    public override void Register()
    {
        Debug.Log($"Registered objective {_data.Id}");
        _collectData.Status = QuestStatus.Active;
        _collectData.EventChannel.OnCollectItem += OnCollectItem;
    }
 
    public override void Unregister()
    {
        Debug.Log($"Unregistered objective {_data.Id}");
        _collectData.Status = QuestStatus.Completed;
        _collectData.EventChannel.OnCollectItem -= OnCollectItem; // fix: += -> -=
    }
 
    private void OnCollectItem(string itemId)
    {
        if (itemId != _collectData.TargetId) return;
 
        _currentAmount++;
        Debug.Log($"Objective {_data.Id}: {_currentAmount}/{_collectData.RequiredAmount}");
 
        // Thông báo tiến độ để UI cập nhật
        _collectData.EventChannel.OnObjectiveProgress?.Invoke(
            _collectData.QuestId,
            _collectData.Id,
            _currentAmount,
            _collectData.RequiredAmount
        );
 
        NotifyProgressChanged();
    }
}
