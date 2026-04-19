using UnityEngine;
using TMPro;

public class WaveStarter : MonoBehaviour
{
    public WaveManager waveManager;
    public TextMeshProUGUI interactText; // Kéo thả cái Text "Ấn F để bắt đầu" vào đây
    private bool _canInteract = false;
    private bool _isWaveRunning = false;

    void Start()
    {
        if (interactText != null) interactText.gameObject.SetActive(false);
    }

    void Update()
    {
        // Chỉ cho phép ấn F khi đang đứng gần và Wave chưa chạy
        if (_canInteract && !_isWaveRunning)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                StartChallenge();
            }
        }
    }

    private void StartChallenge()
    {
        _isWaveRunning = true;
        if (interactText != null) interactText.gameObject.SetActive(false); // Ẩn text ngay khi bắt đầu
        waveManager.StartWave();
    }

    // Hàm này để WaveManager gọi lại khi xong Wave (để cho phép ấn tiếp Wave sau)
    public void EnableInteraction()
    {
        _isWaveRunning = false;
        // Nếu người chơi vẫn đang đứng trong vùng va chạm thì hiện lại text
        if (_canInteract) interactText.gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isWaveRunning)
        {
            _canInteract = true;
            if (interactText != null) interactText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _canInteract = false;
            if (interactText != null) interactText.gameObject.SetActive(false);
        }
    }
}