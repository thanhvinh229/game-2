using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Audio & Brightness")]
    public Slider volumeSlider;
    public Slider brightnessSlider;
    public Image  brightnessOverlay;
 
    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
    public GameObject pauseMenuPanel;
 
    [Header("Audio Settings")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip   _clickSound;
    [Range(0f, 1f)]
    [SerializeField] private float _clickVolume = 0.5f;
 
    private static bool _isPlaying = false;
    private static bool _isLoading = false;
    private bool _isPaused = false;
 
    void Start()
    {
        if (volumeSlider    != null) volumeSlider.value    = AudioListener.volume;
        if (brightnessSlider != null) brightnessSlider.value = 1f;
 
        if (!_isPlaying)
        {
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
 
            // FIX: Dùng coroutine để chờ 1 frame cho tất cả singleton (ItemDatabase,
            //      InventoryManager, EquipmentManager…) kịp Awake/Start xong
            if (_isLoading && PlayerPrefs.HasKey("HasSaveData"))
                StartCoroutine(LoadGameDataDelayed());
        }
    }
 
    // ════════════════════════════════════════════════════════
    // FIX: Chờ 1 frame rồi mới load để tránh NullReferenceException
    // ════════════════════════════════════════════════════════
    private IEnumerator LoadGameDataDelayed()
    {
        // Chờ đúng 1 frame – lúc này tất cả Start() đã chạy xong
        yield return null;
        LoadGameData();
    }
 
    void Update()
    {
        if (!_isPlaying) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionsPanel.activeSelf)  CloseOptions();
            else if (_isPaused)           ResumeGame();
            else                          PauseGame();
        }
    }
 
    // ════════════════════════════════════════════════════════
    // SAVE
    // ════════════════════════════════════════════════════════
    private void SaveGameData()
    {
        // 1. Player position + HP + Mana
        var player = FindFirstObjectByType<PlayerStats>();
        if (player != null)
        {
            PlayerPrefs.SetFloat("PlayerX",    player.transform.position.x);
            PlayerPrefs.SetFloat("PlayerY",    player.transform.position.y);
            PlayerPrefs.SetFloat("PlayerZ",    player.transform.position.z);
            PlayerPrefs.SetFloat("PlayerHP",   player.currentHealth);
            PlayerPrefs.SetFloat("PlayerMana", player.currentMana);
        }
 
        // 2. Level + EXP
        if (PlayerLevel.Instance != null)
            PlayerLevel.Instance.Save();
 
        // 3. Gold
        if (WalletManager.Instance != null)
            WalletManager.Instance.Save();
 
        // 4. Wave
        var waveManager = FindFirstObjectByType<WaveManager>();
        if (waveManager != null)
            PlayerPrefs.SetInt("WaveIndex", waveManager.currentWaveIndex);
 
        // 5. Inventory
        SaveInventory();
 
        // 6. Equipment
        SaveEquipment();
 
        PlayerPrefs.SetInt("HasSaveData", 1);
        PlayerPrefs.Save();
        Debug.Log("[Save] Đã lưu toàn bộ tiến trình!");
    }
 
    // ── Inventory ─────────────────────────────────────────────────────
    void SaveInventory()
    {
        if (InventoryManager.Instance == null) return;
        var slots = InventoryManager.Instance.GetSlots();
 
        PlayerPrefs.SetInt("InvCount", slots.Count);
        for (int i = 0; i < slots.Count; i++)
        {
            string id  = slots[i].IsEmpty ? "" : slots[i].item.itemId;
            int    qty = slots[i].IsEmpty ? 0  : slots[i].quantity;
            PlayerPrefs.SetString($"Inv_{i}_Id",  id);
            PlayerPrefs.SetInt   ($"Inv_{i}_Qty", qty);
        }
        Debug.Log("[Save] Inventory đã lưu.");
    }
 
    // ── Equipment ─────────────────────────────────────────────────────
    void SaveEquipment()
    {
        if (EquipmentManager.Instance == null) return;
        var all = EquipmentManager.Instance.GetAllEquipped();
 
        PlayerPrefs.SetInt("EquipCount", all.Count);
 
        int idx = 0;
        foreach (var kv in all)
        {
            PlayerPrefs.SetString($"Equip_{idx}_Slot",   kv.Key.ToString());
            PlayerPrefs.SetString($"Equip_{idx}_ItemId", kv.Value.itemId);
            idx++;
        }
        Debug.Log("[Save] Equipment đã lưu.");
    }
 
    // ════════════════════════════════════════════════════════
    // LOAD
    // ════════════════════════════════════════════════════════
    private void LoadGameData()
    {
        // 1. Player position + HP + Mana
        var player = FindFirstObjectByType<PlayerStats>();
        if (player != null)
        {
            player.transform.position = new Vector3(
                PlayerPrefs.GetFloat("PlayerX"),
                PlayerPrefs.GetFloat("PlayerY"),
                PlayerPrefs.GetFloat("PlayerZ"));
            player.currentHealth = PlayerPrefs.GetFloat("PlayerHP");
            player.currentMana   = PlayerPrefs.GetFloat("PlayerMana");
            player.HandleSmoothUI();
        }
 
        // 2. Level + EXP
        if (PlayerLevel.Instance != null)
            PlayerLevel.Instance.Load();
 
        // 3. Gold
        if (WalletManager.Instance != null)
            WalletManager.Instance.Load();
 
        // 4. Wave
        var waveManager = FindFirstObjectByType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.currentWaveIndex = PlayerPrefs.GetInt("WaveIndex");
            waveManager.ResetWaveUI();
        }
 
        // 5. Inventory
        LoadInventory();
 
        // 6. Equipment (sau inventory để item đã có trong DB)
        LoadEquipment();
 
        Debug.Log("[Load] Tải dữ liệu thành công!");
    }
 
    // ── Inventory ─────────────────────────────────────────────────────
    void LoadInventory()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("[Load] InventoryManager chưa sẵn sàng!");
            return;
        }
 
        int count = PlayerPrefs.GetInt("InvCount", 0);
        if (count == 0) return;
 
        var slots = InventoryManager.Instance.GetSlots();
        for (int i = 0; i < count && i < slots.Count; i++)
        {
            string id  = PlayerPrefs.GetString($"Inv_{i}_Id",  "");
            int    qty = PlayerPrefs.GetInt   ($"Inv_{i}_Qty", 0);
 
            if (string.IsNullOrEmpty(id))
            {
                slots[i].item     = null;
                slots[i].quantity = 0;
            }
            else
            {
                // FIX: Guard trước khi gọi ItemDatabase.GetById
                var item = ItemDatabase.GetById(id);
                if (item != null)
                {
                    slots[i].item     = item;
                    slots[i].quantity = qty;
                }
                else
                {
                    Debug.LogWarning($"[Load] Không tìm thấy item id: {id}");
                }
            }
        }
 
        Debug.Log("[Load] Inventory đã nạp.");
    }
 
    // ── Equipment ─────────────────────────────────────────────────────
    void LoadEquipment()
    {
        if (EquipmentManager.Instance == null)
        {
            Debug.LogWarning("[Load] EquipmentManager chưa sẵn sàng!");
            return;
        }
 
        int count = PlayerPrefs.GetInt("EquipCount", 0);
        for (int i = 0; i < count; i++)
        {
            string slotName = PlayerPrefs.GetString($"Equip_{i}_Slot",   "");
            string itemId   = PlayerPrefs.GetString($"Equip_{i}_ItemId", "");
 
            if (string.IsNullOrEmpty(slotName) || string.IsNullOrEmpty(itemId)) continue;
 
            if (!System.Enum.TryParse<EquipSlot>(slotName, out var equipSlot)) continue;
 
            var item = ItemDatabase.GetById(itemId);
            if (item != null)
                EquipmentManager.Instance.Equip(item, equipSlot);
            else
                Debug.LogWarning($"[Load] Không tìm thấy item trang bị id: {itemId}");
        }
        Debug.Log("[Load] Equipment đã nạp.");
    }
 
    // ════════════════════════════════════════════════════════
    // MENU BUTTONS
    // ════════════════════════════════════════════════════════
    public void StartNewGame()
    {
        PlayClickSound();
        PlayerPrefs.DeleteKey("HasSaveData");
        _isPlaying = true;
        _isLoading = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
 
    public void LoadGame()
    {
        PlayClickSound();
        if (PlayerPrefs.HasKey("HasSaveData"))
        {
            _isPlaying = true;
            _isLoading = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy dữ liệu save!");
        }
    }
 
    public void QuitToMainMenu()
    {
        PlayClickSound();
        SaveGameData();
        _isPlaying = false;
        _isPaused  = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
 
    public void PauseGame()
    {
        PlayClickSound();
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        _isPaused = true;
    }
 
    public void ResumeGame()
    {
        PlayClickSound();
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        _isPaused = false;
    }
 
    public void OpenOptions()
    {
        PlayClickSound();
        optionsPanel.SetActive(true);
        if (!_isPlaying) mainMenuPanel.SetActive(false);
        else             pauseMenuPanel.SetActive(false);
    }
 
    public void CloseOptions()
    {
        PlayClickSound();
        optionsPanel.SetActive(false);
        if (!_isPlaying) mainMenuPanel.SetActive(true);
        else             pauseMenuPanel.SetActive(true);
    }
 
    public void ChangeVolume()     { AudioListener.volume = volumeSlider.value; }
 
    public void ChangeBrightness()
    {
        if (brightnessOverlay == null) return;
        float alpha = 1f - brightnessSlider.value;
        Color color = brightnessOverlay.color;
        color.a     = Mathf.Clamp(alpha, 0f, 0.9f);
        brightnessOverlay.color = color;
    }
 
    public void PlayClickSound()
    {
        if (_audioSource != null && _clickSound != null)
            _audioSource.PlayOneShot(_clickSound, _clickVolume);
    }
 
    public void QuitGame()
    {
        PlayClickSound();
        Application.Quit();
    }
}

