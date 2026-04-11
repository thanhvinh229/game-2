using UnityEngine;
// Replace 'YourNamespace' with the actual namespace containing EnemyDeathEventChannel

[CreateAssetMenu(fileName = "KillObjectiveData", menuName = "Scriptable Objects/KillObjectiveData")]
public class KillObjectiveData : ObjectiveData
{
    [Header("Kill Objective Settings")]
    [Tooltip("Loại enemy cần giết (phải khớp với enemyType trong EnemyStats)")]
    public string targetEnemyType = "Skeleton";
    
    
    [Tooltip("Số lượng enemy cần giết")]
    public int requiredKills = 5;

    [Header("Enemy Death Event")]
    [Tooltip("Event channel để listen enemy death events")]
    public EnemyDeathEventChannel deathEventChannel;

    public override Objective CreateInstance()
    {
        return new KillObjective(this);
    }

    #if UNITY_EDITOR
    void OnValidate()
    {
        // Auto-generate description nếu trống
        if (string.IsNullOrEmpty(Description))
        {
            Description = $"Kill {requiredKills} {targetEnemyType}";
        }
    }
    #endif
}
