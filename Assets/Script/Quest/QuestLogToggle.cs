    using UnityEngine;
using UnityEngine.InputSystem;

public class QuestLogToggle : MonoBehaviour
{
    [SerializeField] private GameObject _questLogPanel;
    [SerializeField] private Key _toggleKey = Key.Q;

    [Header("Cursor Settings")]
    [SerializeField] private bool _showCursorWhenOpen = true;
    [SerializeField] private bool _pauseGameWhenOpen = false;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _openSound;
    [SerializeField] private AudioClip _closeSound;
    [SerializeField] private AudioClip _buttonClickSound;

    private bool _previousCursorVisible;
    private CursorLockMode _previousLockMode;

     void Update()
    {
        if (Keyboard.current[_toggleKey].wasPressedThisFrame)
            ToggleQuestLog();

        if (!_questLogPanel.activeSelf)
        {
            if (Keyboard.current.altKey.wasPressedThisFrame)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            if (Keyboard.current.altKey.wasReleasedThisFrame)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    public void ToggleQuestLog()
{
    bool isOpen = !_questLogPanel.activeSelf;

    if (isOpen)
    {
        _questLogPanel.SetActive(true);
        OnQuestLogOpened();
    }
    else
    {
        OnQuestLogClosed();          
        _questLogPanel.SetActive(false); 
    }
}

    public void OnQuestLogOpened()
    {
        GameStateManager.SetUIOpen(true);
        Time.timeScale = 0f;

        // Dùng unscaledTime vì timeScale = 0
        PlaySoundUnscaled(_openSound);

        if (_showCursorWhenOpen)
        {
            _previousCursorVisible = Cursor.visible;
            _previousLockMode = Cursor.lockState;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void OnQuestLogClosed()
    {
        GameStateManager.SetUIOpen(false);
        Time.timeScale = 1f;

        PlaySoundUnscaled(_closeSound);

        if (_showCursorWhenOpen)
        {
            Cursor.visible = _previousCursorVisible;
            Cursor.lockState = _previousLockMode;
        }
    }

    // Gọi hàm này từ các Button trong bảng quest
    public void PlayButtonClick()
    {
        PlaySoundUnscaled(_buttonClickSound);
    }

    // Phải dùng PlayOneShot trực tiếp thay vì Play()
    // vì timeScale = 0 làm AudioSource.Play() bị câm
    public void PlaySoundUnscaled(AudioClip clip)
    {
        if (_audioSource == null || clip == null) return;
        _audioSource.ignoreListenerPause = true;
        _audioSource.PlayOneShot(clip);
    }

    public void OpenQuestLog()
    {
        if (!_questLogPanel.activeSelf)
        {
            _questLogPanel.SetActive(true);
            OnQuestLogOpened();
        }
    }

    public void CloseQuestLog()
    {
        Debug.Log("Đã bấm nút X!");
        if (_questLogPanel.activeSelf)
        {
            OnQuestLogClosed();
            _questLogPanel.SetActive(false);
            
        }   
        
    }
}
