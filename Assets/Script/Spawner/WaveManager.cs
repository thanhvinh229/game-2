using System.Collections;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Wave
{
    public string waveName;
    public GameObject[] enemyPrefabs; // Chứa các loại quái sẽ xuất hiện trong đợt này
    public int totalEnemies;          // Tổng số quái cần tiêu diệt
    public float spawnDelay;          // Thời gian chờ giữa 2 lần đẻ quái
}

public class WaveManager : MonoBehaviour
{
    [Header("Wave Settings")]
    public Wave[] waves = new Wave[10];
    public int currentWaveIndex = 0;
    
    [Header("Spawn Settings")]
    public Transform[] spawnPoints; // Kéo thả các Empty Object nằm ngoài rìa map vào đây
    
    [Header("Events & UI")]
    public EnemyDeathEventChannel deathEventChannel; // Kéo SO Event Channel của bạn vào đây
    public TextMeshProUGUI waveStatusText;
    public WaveStarter waveStarterObject; // Object tảng đá tương tác

    private int _enemiesKilled;
    private int _enemiesToKill;
    private bool _waveIsActive = false;

    private void OnEnable()
    {
        if (deathEventChannel != null)
        {
            // Lắng nghe chung sự kiện chết với KillObjective
            deathEventChannel.OnEnemyDeath += HandleEnemyDeath; 
        }
    }

    private void OnDisable()
    {
        if (deathEventChannel != null)
        {
            deathEventChannel.OnEnemyDeath -= HandleEnemyDeath;
        }
    }

    public void StartWave()
    {
        if (_waveIsActive || currentWaveIndex >= waves.Length) return;
        
        StartCoroutine(SpawnWaveRoutine());
    }

    private IEnumerator SpawnWaveRoutine()
    {
        _waveIsActive = true;
        Wave currentWave = waves[currentWaveIndex];
        
        _enemiesToKill = currentWave.totalEnemies;
        _enemiesKilled = 0;
        
        UpdateUI($"Đang chiến đấu: {currentWave.waveName} ({_enemiesKilled}/{_enemiesToKill})");

        for (int i = 0; i < currentWave.totalEnemies; i++)
        {
            // Chọn ngẫu nhiên 1 điểm spawn và 1 loại quái
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemyPrefab = currentWave.enemyPrefabs[Random.Range(0, currentWave.enemyPrefabs.Length)];

            Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            
            // Đợi một khoảng thời gian trước khi đẻ con tiếp theo
            yield return new WaitForSeconds(currentWave.spawnDelay);
        }
    }

    private void HandleEnemyDeath(string enemyType, GameObject enemyGameObject)
    {
        if (!_waveIsActive) return;

        // Lưu ý: Ở đây ta đếm mọi con quái chết, không phân biệt enemyType. 
        // Nếu màn chơi yêu cầu giết một loại quái cụ thể, bạn có thể thêm if check ở đây.
        _enemiesKilled++;
        
        Wave currentWave = waves[currentWaveIndex];
        UpdateUI($"Đang chiến đấu: {currentWave.waveName} ({_enemiesKilled}/{_enemiesToKill})");

        if (_enemiesKilled >= _enemiesToKill)
        {
            EndWave();
        }
    }

    private void EndWave()
    {
        _waveIsActive = false;
        currentWaveIndex++;

        if (currentWaveIndex >= waves.Length)
        {
            UpdateUI("CHIẾN THẮNG!");
            // Gọi các logic kết thúc game ở đây
        }
        else
        {
            UpdateUI("Vượt ải thành công! Tương tác với Tảng Đá để tiếp tục.");
            waveStarterObject.EnableInteraction(); // Bật lại tảng đá cho màn sau
        }
    }

    private void UpdateUI(string message)
    {
        if (waveStatusText != null)
        {
            waveStatusText.text = message;
        }
    }
}
