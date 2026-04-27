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
    public GameObject pauseMenuPanel; // BẢNG MỚI: Kéo PauseMenuPanel vào đây

    private static bool _isPlaying = false;
    private bool _isPaused = false; // Biến kiểm tra xem game có đang tạm dừng không

    void Start()
    {
        if (volumeSlider != null) volumeSlider.value = AudioListener.volume;
        if (brightnessSlider != null) brightnessSlider.value = 1f;

        if (!_isPlaying)
        {
            // ĐANG Ở MAIN MENU
            mainMenuPanel.SetActive(true);
            optionsPanel.SetActive(false);
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
            Time.timeScale = 0f;
        }
        else
        {
            // VỪA ẤN NEW GAME / LOAD GAME
            mainMenuPanel.SetActive(false);
            optionsPanel.SetActive(false);
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    void Update()
    {
        // Nhấn ESC để bật/tắt Pause Menu (chỉ hoạt động khi đang chơi game)
        if (_isPlaying)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (optionsPanel.activeSelf)
                {
                    CloseOptions(); // Nếu đang mở Option thì đóng Option lại
                }
                else if (_isPaused)
                {
                    ResumeGame();   // Nếu đang Pause thì Resume
                }
                else
                {
                    PauseGame();    // Nếu đang chơi bình thường thì Pause
                }
            }
        }
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

    public void QuitToMainMenu()
    {
        Debug.Log("Quay về Main Menu");
        _isPlaying = false; 
        _isPaused = false;
        
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

   
    public void OpenOptions()
    {
        optionsPanel.SetActive(true);

        
        if (!_isPlaying) 
            mainMenuPanel.SetActive(false); 
        else 
            pauseMenuPanel.SetActive(false); 
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);

       
        if (!_isPlaying) 
            mainMenuPanel.SetActive(true); 
        else 
            pauseMenuPanel.SetActive(true); 
    }

    
    public void StartNewGame()
    {
        _isPlaying = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadGame()
    {
        _isPlaying = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }

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