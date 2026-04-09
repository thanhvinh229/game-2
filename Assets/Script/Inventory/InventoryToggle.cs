using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject inventoryPanel;   // Panel trái (túi đồ)
    [SerializeField] private GameObject equipmentPanel;  // Panel phải (trang bị)

    [Header("Phím bấm")]
    [SerializeField] private KeyCode toggleKey = KeyCode.B;

    // Dùng để pause game / khóa movement khi mở inventory
    private bool isOpen = false;

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            Toggle();
    }

    public void Toggle()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);
        equipmentPanel.SetActive(isOpen);

        // Tuỳ chọn: dừng thời gian khi mở inventory
        // Time.timeScale = isOpen ? 0f : 1f;

        // Tuỳ chọn: ẩn / hiện con trỏ
        Cursor.visible   = isOpen;
        Cursor.lockState = isOpen
            ? CursorLockMode.None
            : CursorLockMode.Locked;
    }

    public void ForceClose()
    {
        isOpen = false;
        inventoryPanel.SetActive(false);
        equipmentPanel.SetActive(false);
        Cursor.visible   = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public bool IsOpen => isOpen;
}
