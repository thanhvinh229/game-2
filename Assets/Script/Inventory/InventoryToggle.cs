using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    [Header("Kéo InventoryRoot vào đây (cha của cả 2 panel)")]
    [SerializeField] private GameObject inventoryRoot;
    [SerializeField] private KeyCode toggleKey = KeyCode.B;

    void Awake()
    {
        // Dùng Awake để ẩn trước khi bất kỳ Start() nào chạy
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

    void Toggle()
    {
        // Đọc trạng thái THẬT từ GameObject, không dùng bool
        bool willOpen = !inventoryRoot.activeSelf;
        inventoryRoot.SetActive(willOpen);

        Time.timeScale   = willOpen ? 0f : 1f;
        Cursor.visible   = willOpen;
        Cursor.lockState = willOpen
            ? CursorLockMode.None
            : CursorLockMode.Locked;

        Debug.Log($"Inventory: {(willOpen ? "MỞ" : "ĐÓNG")} | TimeScale: {Time.timeScale}");
    }

    public void ForceClose()
    {
        if (inventoryRoot != null) inventoryRoot.SetActive(false);
        Time.timeScale   = 1f;
        Cursor.visible   = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}

