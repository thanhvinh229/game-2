using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private int maxSlots = 30;
    [SerializeField] private List<InventorySlot> slots = new();

    public event Action OnInventoryChanged;

    void Awake() {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitSlots();
    }

    void InitSlots() {
        slots.Clear();
        for (int i = 0; i < maxSlots; i++)
            slots.Add(new InventorySlot());
    }

    public bool AddItem(ItemData item, int amount = 1) {
        // Thử stack vào slot đang có item cùng loại
        if (item.isStackable) {
            var existing = slots.FirstOrDefault(s =>
                s.item == item && s.CanAddMore);
            if (existing != null) {
                existing.quantity = Mathf.Min(existing.quantity + amount, item.maxStackSize);
                OnInventoryChanged?.Invoke();
                return true;
            }
        }
        // Tìm slot trống
        var empty = slots.FirstOrDefault(s => s.IsEmpty);
        if (empty == null) {
            Debug.Log("Inventory đầy!");
            return false;
        }
        empty.item = item;
        empty.quantity = amount;
        OnInventoryChanged?.Invoke();
        return true;
    }

    public void RemoveItem(int slotIndex, int amount = 1) {
        if (slotIndex < 0 || slotIndex >= slots.Count) return;
        var slot = slots[slotIndex];
        if (slot.IsEmpty) return;

        slot.quantity -= amount;
        if (slot.quantity <= 0) {
            slot.item = null;
            slot.quantity = 0;
        }
        OnInventoryChanged?.Invoke();
    }

    public void SwapSlots(int indexA, int indexB) {
        (slots[indexA], slots[indexB]) = (slots[indexB], slots[indexA]);
        OnInventoryChanged?.Invoke();
    }

    public List<InventorySlot> GetSlots() => slots;
}
