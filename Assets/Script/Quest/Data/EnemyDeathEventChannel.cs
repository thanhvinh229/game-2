using System;
using UnityEngine;

/// <summary>
/// Event channel để broadcast khi enemy chết
/// Tương tự như QuestEventChannel
/// </summary>
[CreateAssetMenu(fileName = "EnemyDeathEventChannel", menuName = "Events/Enemy Death Event Channel")]
public class EnemyDeathEventChannel : ScriptableObject
{
    /// <summary>
    /// Event fired khi enemy chết
    /// Parameters: enemyType, enemyGameObject
    /// </summary>
    public event Action<string, GameObject> OnEnemyDeath;

    /// <summary>
    /// Broadcast enemy death event
    /// </summary>
    /// <param name="enemyType">Loại enemy (ví dụ: "Skeleton", "Goblin")</param>
    /// <param name="enemyGameObject">GameObject của enemy</param>
    public void RaiseEnemyDeath(string enemyType, GameObject enemyGameObject)
    {
        OnEnemyDeath?.Invoke(enemyType, enemyGameObject);
        Debug.Log($"💀 Enemy Death Event: {enemyType} killed");
    }
}
