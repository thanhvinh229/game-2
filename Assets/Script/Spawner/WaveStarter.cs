using UnityEngine;

public class WaveStarter : MonoBehaviour
{
    public WaveManager waveManager;
    private bool _canInteract = true;

    // Hàm này được gọi khi người chơi bấm nút tương tác (hoặc OnTriggerEnter)
    public void Interact()
    {
        if (_canInteract)
        {
            _canInteract = false;
            waveManager.StartWave();
            Debug.Log("Bắt đầu đợt quái mới!");
            
            // Bạn có thể thêm Audio phát âm thanh đá ma thuật ở đây để tăng độ "đã"
        }
    }

    // WaveManager sẽ gọi hàm này khi dọn sạch quái
    public void EnableInteraction()
    {
        _canInteract = true;
    }

    // Ví dụ đơn giản: Tự động chạy màn khi người chơi chạm vào collider của Tảng đá
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Interact();
        }
    }
}
