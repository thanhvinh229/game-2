using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnDelay = 2f;
    public int maxEnemy = 10;

    private int currentEnemy = 0;

    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 1f, spawnDelay);
    }

    void SpawnEnemy()
    {
        if (currentEnemy >= maxEnemy) return;

        Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        currentEnemy++;
    }
}
