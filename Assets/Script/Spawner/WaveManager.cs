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
    
    [Header("Events & Status UI")]
    public EnemyDeathEventChannel deathEventChannel; // Kéo SO Event Channel của bạn vào đây
    public TextMeshProUGUI waveStatusText;           // Text hiển thị góc màn hình (VD: Wave 1: 5/10)
    public WaveStarter waveStarterObject;            // Object tảng đá tương tác

    [Header("Announcement UI (Chữ lớn giữa màn hình)")]
    public TextMeshProUGUI waveAnnounceText; 
    public CanvasGroup announcementCanvasGroup;      // Thêm component Canvas Group vào UI Text để làm mờ
    public Color normalWaveColor = Color.white;
    public Color victoryColor = Color.yellow;
    public Color defeatColor = Color.red;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip warHornClip;      // Tiếng tù và khi bắt đầu (WAVE X)
    public AudioClip waveCompleteClip; // Tiếng khi xong màn 1-9 (Tùy chọn)
    public AudioClip victoryClip;      // Nhạc chiến thắng màn 10
    public AudioClip defeatClip;       // Nhạc thất bại

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
        
        StartCoroutine(StartWaveRoutine());
    }

    private IEnumerator StartWaveRoutine()
    {
        _waveIsActive = true;
        Wave currentWave = waves[currentWaveIndex];
        
        _enemiesToKill = currentWave.totalEnemies;
        _enemiesKilled = 0;
        
        UpdateUI($"Đang chiến đấu: {currentWave.waveName} ({_enemiesKilled}/{_enemiesToKill})");

        // Gọi UI và âm thanh thông báo bắt đầu màn
        yield return StartCoroutine(ShowAnnouncement($"WAVE {currentWaveIndex + 1}", normalWaveColor, warHornClip, false));

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
        
        // Kiểm tra xem đây có phải là màn 10 (index 9) không
        if (currentWaveIndex >= waves.Length - 1)
        {
            StartCoroutine(ShowVictoryEffect());
        }
        else
        {
            currentWaveIndex++;
            StartCoroutine(ShowWaveCompleteEffect());
            waveStarterObject.EnableInteraction();
        }
    }

    // Hàm này gọi khi Player hết máu (Defeat)
    public void TriggerDefeat()
    {
        _waveIsActive = false;
        StopAllCoroutines(); // Ngay lập tức dừng sinh quái
        StartCoroutine(ShowAnnouncement("DEFEAT", defeatColor, defeatClip, true));

        AudioSource audioSource = GetComponent<AudioSource>();
    }

    // --- CÁC HÀM XỬ LÝ HIỆU ỨNG UI & AUDIO ---

    // 1. Hiệu ứng Fade In/Out dùng cho WAVE X và DEFEAT
    private IEnumerator ShowAnnouncement(string text, Color textColor, AudioClip clip, bool isDefeat)
    {
        if (waveAnnounceText == null || announcementCanvasGroup == null) yield break;

        waveAnnounceText.text = text;
        waveAnnounceText.color = textColor;
        
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);

        // Nếu là Thất bại, làm chậm game (Slow Motion)
        if (isDefeat) Time.timeScale = 0.5f;

        announcementCanvasGroup.alpha = 0;
        waveAnnounceText.transform.localScale = Vector3.one * 0.8f;
        waveAnnounceText.gameObject.SetActive(true);
        
        float elapsed = 0f;
        float duration = 0.5f;

        // Hiệu ứng Fade In
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // Dùng unscaled để Slow Motion không làm lỗi animation UI
            float percent = elapsed / duration;
            announcementCanvasGroup.alpha = percent;
            waveAnnounceText.transform.localScale = Vector3.Lerp(Vector3.one * 0.8f, Vector3.one, percent);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(2f);

        // Nếu Game Over, giữ nguyên chữ DEFEAT. Nếu không thì Fade Out.
        if (!isDefeat)
        {
            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                announcementCanvasGroup.alpha = 1 - (elapsed / duration);
                yield return null;
            }
            waveAnnounceText.gameObject.SetActive(false);
        }
    }

    // 2. Hiệu ứng hoàn thành màn bình thường (1-9)
    private IEnumerator ShowWaveCompleteEffect()
    {
        if (waveAnnounceText == null) yield break;

        if (audioSource != null && waveCompleteClip != null)
            audioSource.PlayOneShot(waveCompleteClip);

        waveAnnounceText.text = "WAVE COMPLETE";
        waveAnnounceText.color = normalWaveColor;
        waveAnnounceText.gameObject.SetActive(true);
        
        if(announcementCanvasGroup != null) announcementCanvasGroup.alpha = 1f;

        // Hiệu ứng phóng to nhẹ chữ
        waveAnnounceText.transform.localScale = Vector3.one * 0.5f;
        float timer = 0;
        while (timer < 0.5f)
        {
            timer += Time.deltaTime;
            waveAnnounceText.transform.localScale = Vector3.Lerp(Vector3.one * 0.5f, Vector3.one, timer / 0.5f);
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);
        waveAnnounceText.gameObject.SetActive(false);
    }

    // 3. Hiệu ứng đặc biệt cho màn 10 (Chiến thắng cuối cùng)
    private IEnumerator ShowVictoryEffect()
    {
        if (waveAnnounceText == null) yield break;

        if (audioSource != null && victoryClip != null)
            audioSource.PlayOneShot(victoryClip);

        waveAnnounceText.text = "VICTORY";
        waveAnnounceText.color = victoryColor;
        waveAnnounceText.gameObject.SetActive(true);
        
        if(announcementCanvasGroup != null) announcementCanvasGroup.alpha = 1f;

        // Hiệu ứng chữ nhấp nháy hoặc rung lắc (Shake)
        Vector3 originalPos = waveAnnounceText.transform.localPosition;
        float elapsed = 0f;
        while (elapsed < 3f) // Hiệu ứng kéo dài 3 giây
        {
            float x = Random.Range(-5f, 5f);
            float y = Random.Range(-5f, 5f);
            waveAnnounceText.transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            
            // Đổi kích thước liên tục tạo hiệu ứng nhấn mạnh
            waveAnnounceText.transform.localScale = Vector3.one * (1f + Mathf.Sin(Time.time * 10f) * 0.1f);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        waveAnnounceText.transform.localPosition = originalPos;
        waveAnnounceText.text = "BẠN ĐÃ CHINH PHỤC THỬ THÁCH!";
    }

    private void UpdateUI(string message)
    {
        if (waveStatusText != null)
        {
            waveStatusText.text = message;
        }
    }

    public void ForceVictory()
    {
        _waveIsActive = false;
        StopAllCoroutines(); // Dừng đẻ quái ngay lập tức
        StartCoroutine(ShowVictoryEffect()); // Bật UI Chiến thắng
    }


    public void ResetWaveUI()
{
    if (waveAnnounceText != null)
    {
        waveAnnounceText.gameObject.SetActive(false);
    }
    
    // Nếu bạn muốn reset luôn cả số quái đếm được về 0 để đánh lại wave đó:
    _enemiesKilled = 0;
    Wave currentWave = waves[currentWaveIndex];
    UpdateUI($"Đang chiến đấu: {currentWave.waveName} ({_enemiesKilled}/{_enemiesToKill})");
}
}