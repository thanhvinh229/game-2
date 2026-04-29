using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Audio & Brightness")]
    public Slider volumeSlider;
    public Slider brightnessSlider;
    public Image brightnessOverlay;

    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
    public GameObject pauseMenuPanel;

    private static bool _isPlaying = false;
    private static bool _isLoading = false; 
    private bool _isPaused = false;

    void Start()
    {
        if (volumeSlider != null) volumeSlider.value = AudioListener.volume;
        if (brightnessSlider != null) brightnessSlider.value = 1f;

        if (!_isPlaying)
        {
            // MAIN MENU
            mainMenuPanel.SetActive(true);
            optionsPanel.SetActive(false);
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
            Time.timeScale = 0f;
        }
        else
        {
            
            mainMenuPanel.SetActive(false);
            optionsPanel.SetActive(false);
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
            Time.timeScale = 1f;

            
            if (_isLoading && PlayerPrefs.HasKey("HasSaveData"))
            {
                LoadGameData();
            }
        }
    }

    void Update()
    {
        if (_isPlaying)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (optionsPanel.activeSelf) CloseOptions();
                else if (_isPaused) ResumeGame();
                else PauseGame();
            }
        }
    }

    // --- LOGIC HỆ THỐNG SAVE / LOAD ---
    private void SaveGameData()
    {
        // 1. Lưu thông tin Player
        PlayerStats player = FindFirstObjectByType<PlayerStats>();
        if (player != null)
        {
            PlayerPrefs.SetFloat("PlayerX", player.transform.position.x);
            PlayerPrefs.SetFloat("PlayerY", player.transform.position.y);
            PlayerPrefs.SetFloat("PlayerZ", player.transform.position.z);
            PlayerPrefs.SetFloat("PlayerHP", player.currentHealth);
            PlayerPrefs.SetFloat("PlayerMana", player.currentMana);
        }

        // 2. Lưu tiến trình Wave
        WaveManager waveManager = FindFirstObjectByType<WaveManager>();
        if (waveManager != null)
        {
            PlayerPrefs.SetInt("WaveIndex", waveManager.currentWaveIndex);
        }

        PlayerPrefs.SetInt("HasSaveData", 1); // Cờ đánh dấu đã có file save
        PlayerPrefs.Save();
        Debug.Log("Đã Auto-Save tiến trình game!");
    }

    private void LoadGameData()
    {
        // 1. Áp dụng thông tin Player
        PlayerStats player = FindFirstObjectByType<PlayerStats>();
        if (player != null)
        {
            player.transform.position = new Vector3(
                PlayerPrefs.GetFloat("PlayerX"), 
                PlayerPrefs.GetFloat("PlayerY"), 
                PlayerPrefs.GetFloat("PlayerZ")
            );
            player.currentHealth = PlayerPrefs.GetFloat("PlayerHP");
            player.currentMana = PlayerPrefs.GetFloat("PlayerMana");
            player.HandleSmoothUI(); // Cập nhật lại thanh máu trên UI
        }

        // 2. Áp dụng tiến trình Wave
        WaveManager waveManager = FindFirstObjectByType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.currentWaveIndex = PlayerPrefs.GetInt("WaveIndex");
            waveManager.ResetWaveUI(); // Dọn dẹp quái và update text (Ví dụ: Chuẩn bị Wave 4)
        }
        
        Debug.Log("Tải dữ liệu thành công!");
    }

    // --- CÁC NÚT ĐIỀU KHIỂN ---
    public void StartNewGame()
    {
        PlayerPrefs.DeleteKey("HasSaveData"); // Xóa file save cũ
        _isPlaying = true;
        _isLoading = false; // Báo cho hệ thống biết là KHÔNG load data
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("HasSaveData"))
        {
            _isPlaying = true;
            _isLoading = true; // Báo cho hệ thống biết LÀ CẦN load data
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy dữ liệu save nào!");
        }
    }

    public void QuitToMainMenu()
    {
        SaveGameData(); // TỰ ĐỘNG LƯU TRƯỚC KHI THOÁT RA MENU

        _isPlaying = false;
        _isPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        _isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        _isPaused = false;
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
        if (!_isPlaying) mainMenuPanel.SetActive(false); 
        else pauseMenuPanel.SetActive(false); 
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        if (!_isPlaying) mainMenuPanel.SetActive(true); 
        else pauseMenuPanel.SetActive(true); 
    }

    public void ChangeVolume() { AudioListener.volume = volumeSlider.value; }

    public void ChangeBrightness()
    {
        if (brightnessOverlay != null)
        {
            float alpha = 1f - brightnessSlider.value;
            Color color = brightnessOverlay.color;
            color.a = Mathf.Clamp(alpha, 0f, 0.9f); 
            brightnessOverlay.color = color;
        }
    }

    public void QuitGame() 
    {
        Application.Quit(); 
    }
}