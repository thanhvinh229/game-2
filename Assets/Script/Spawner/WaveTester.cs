using UnityEngine;

public class WaveTester : MonoBehaviour
{
    [Header("Kéo WaveManager vào đây")]
    public WaveManager waveManager;

    void Update()
    {
        // Nhấn F1 để thắng ngay lập tức
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (waveManager != null)
            {
                Debug.Log("CHEAT: Đã kích hoạt Chiến Thắng!");
                waveManager.ForceVictory();
            }
        }

        // Nhấn F2 để giả vờ Player chết (Thất bại)
        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (waveManager != null)
            {
                Debug.Log("CHEAT: Đã kích hoạt Thất Bại!");
                waveManager.TriggerDefeat();
            }
        }
    }
}
