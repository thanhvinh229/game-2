using System;
using UnityEngine;

public class KillObjective : Objective
{
    private KillObjectiveData _killData;
    private int _currentKills;
 
    public override bool IsCompleted => _currentKills >= _killData.requiredKills;
    public int CurrentKills => _currentKills;
 
    public KillObjective(KillObjectiveData data) : base(data)
    {
        _killData = data;
    }
 
    public override void Register()
    {
        Debug.Log($"Registered objective {_data.Id}");
        _killData.Status = QuestStatus.Active;
        
        if (_killData.deathEventChannel != null)
        {
            _killData.deathEventChannel.OnEnemyDeath += OnEnemyKilled;
        }
        else
        {
            Debug.LogError($"KillObjective: deathEventChannel is null for {_data.Id}");
        }
    }
 
    public override void Unregister()
    {
        Debug.Log($"Unregistered objective {_data.Id}");
        _killData.Status = QuestStatus.Completed;
        
        if (_killData.deathEventChannel != null)
        {
            _killData.deathEventChannel.OnEnemyDeath -= OnEnemyKilled;
        }
    }
 
    private void OnEnemyKilled(string enemyType, GameObject enemyGameObject)
    {
        if (enemyType != _killData.targetEnemyType) return;
 
        _currentKills++;
        Debug.Log($"Objective {_data.Id}: {_currentKills}/{_killData.requiredKills}");
 
        // Thông báo tiến độ để UI cập nhật
        _killData.EventChannel.OnObjectiveProgress?.Invoke(
            _killData.QuestId,
            _killData.Id,
            _currentKills,
            _killData.requiredKills
        );
 
        NotifyProgressChanged();
    }
}
