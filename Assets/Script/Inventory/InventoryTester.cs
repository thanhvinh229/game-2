using UnityEngine;
using System.Collections.Generic;

public class InventoryTester : MonoBehaviour
{
    [System.Serializable]
    public class TestEntry {
        public ItemData item;
        public int quantity = 1;
    }

    [Header("Kéo item vào đây để test")]
    public List<TestEntry> itemsToAdd = new();

    [Header("Phím bấm để thêm")]
    public KeyCode addKey = KeyCode.T;

    void Update()
    {
        if (Input.GetKeyDown(addKey))
            AddAll();
    }

    [ContextMenu("Add All Items Now")]
    public void AddAll()
    {
        // Kiểm tra InventoryManager tồn tại chưa
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("Không tìm thấy InventoryManager trong scene! " +
                           "Tạo GameObject và gắn InventoryManager.cs vào.");
            return;
        }

        foreach (var entry in itemsToAdd)
        {
            if (entry.item == null)
            {
                Debug.LogWarning("Có entry bị null — bỏ qua.");
                continue;
            }
            bool ok = InventoryManager.Instance.AddItem(entry.item, entry.quantity);
            Debug.Log(ok
                ? $"Đã thêm: {entry.item.itemName} x{entry.quantity}"
                : $"Thất bại: {entry.item.itemName} — inventory có thể đầy.");
        }
    }
}
