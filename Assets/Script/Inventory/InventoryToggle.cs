using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject inventoryRoot;
    [SerializeField] private KeyCode toggleKey = KeyCode.B;
    
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    void Awake()
    {
       
        if (inventoryRoot == null)
        {
            Debug.LogError("[InventoryToggle] Chưa kéo InventoryRoot vào Inspector!");
            return;
        }
        inventoryRoot.SetActive(false);
        Time.timeScale   = 1f;
        Cursor.visible   = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (inventoryRoot == null) return;
        if (Input.GetKeyDown(toggleKey))
            Toggle();
    }

   public  void Toggle()
    {
        // Đọc trạng thái THẬT từ GameObject, không dùng bool
        bool willOpen = !inventoryRoot.activeSelf;
        inventoryRoot.SetActive(willOpen);

        if (audioSource != null)
        {
            AudioClip clipToPlay = willOpen ? openSound : closeSound;
            if (clipToPlay != null) audioSource.PlayOneShot(clipToPlay);
        }

        Time.timeScale   = willOpen ? 0f : 1f;
        Cursor.visible   = willOpen;
        Cursor.lockState = willOpen
            ? CursorLockMode.None
            : CursorLockMode.Locked;

        Debug.Log($"Inventory: {(willOpen ? "MỞ" : "ĐÓNG")} | TimeScale: {Time.timeScale}");
    }

   public void ForceClose()
    {
        if (inventoryRoot != null && inventoryRoot.activeSelf) 
        {
            inventoryRoot.SetActive(false); //[cite: 2]
            if (audioSource != null && closeSound != null) 
                audioSource.PlayOneShot(closeSound);
        }
        
        Time.timeScale   = 1f; //[cite: 2]
        Cursor.visible   = false; //[cite: 2]
        Cursor.lockState = CursorLockMode.Locked; //[cite: 2]
    }
}

