using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private int maxSlots = 30;

    // BỎ [SerializeField] — không để Unity serialize list này
    // vì sẽ bị ghi đè bằng list rỗng từ Inspector
    private List<InventorySlot> slots;

    public event Action OnInventoryChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitSlots();
    }

    void InitSlots()
    {
        slots = new List<InventorySlot>();
        for (int i = 0; i < maxSlots; i++)
            slots.Add(new InventorySlot());

        Debug.Log($"[InventoryManager] Khởi tạo {slots.Count} slots.");
    }

    public bool AddItem(ItemData item, int amount = 1)
    {
        if (item == null) return false;

        // Thử stack vào slot đang có item cùng loại
        if (item.isStackable)
        {
            var existing = slots.FirstOrDefault(s =>
                s.item == item && s.quantity < item.maxStackSize);
            if (existing != null)
            {
                existing.quantity = Mathf.Min(
                    existing.quantity + amount, item.maxStackSize);
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        // Tìm slot trống
        var empty = slots.FirstOrDefault(s => s.IsEmpty);
        if (empty == null)
        {
            Debug.Log("[InventoryManager] Inventory đầy!");
            return false;
        }

        empty.item     = item;
        empty.quantity = amount;
        OnInventoryChanged?.Invoke();
        return true;
    }

    public void RemoveItem(int slotIndex, int amount = 1)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count) return;
        var slot = slots[slotIndex];
        if (slot.IsEmpty) return;

        slot.quantity -= amount;
        if (slot.quantity <= 0)
        {
            slot.item     = null;
            slot.quantity = 0;
        }
        OnInventoryChanged?.Invoke();
    }

    public void SwapSlots(int indexA, int indexB)
    {
        if (indexA < 0 || indexB < 0 ||
            indexA >= slots.Count || indexB >= slots.Count) return;
        (slots[indexA], slots[indexB]) = (slots[indexB], slots[indexA]);
        OnInventoryChanged?.Invoke();
    }

    public List<InventorySlot> GetSlots() => slots;

    // Debug helper — xem trong Console
    [ContextMenu("Debug: Print Slot Count")]
    public void PrintSlotCount()
    {
        int used  = slots.Count(s => !s.IsEmpty);
        Debug.Log($"Slots: {used}/{slots.Count} đang dùng.");
    }
}