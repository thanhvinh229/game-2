using UnityEngine;
using System;
using System.Collections.Generic;

public class EquipmentManager : MonoBehaviour 
{
    public static EquipmentManager Instance { get; private set; }
 
    private Dictionary<EquipSlot, ItemData> equipped = new();
 
    public event Action<EquipSlot, ItemData> OnEquipped;
    public event Action<EquipSlot>           OnUnequipped;
 
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }
 
    public bool Equip(ItemData item) => Equip(item, item.equipSlot);
 
    public bool Equip(ItemData item, EquipSlot targetSlot)
    {
        if (item == null || targetSlot == EquipSlot.None) return false;
 
        if (equipped.ContainsKey(targetSlot))
            Unequip(targetSlot);
 
        equipped[targetSlot] = item;
        PlayerStats.Instance.ApplyModifiers(item.stats, add: true);
        OnEquipped?.Invoke(targetSlot, item);
        return true;
    }
 
    // Tháo đồ → trả về inventory (dùng khi player tháo bình thường)
    public void Unequip(EquipSlot slot)
    {
        if (!equipped.TryGetValue(slot, out var item)) return;
        PlayerStats.Instance.ApplyModifiers(item.stats, add: false);
        InventoryManager.Instance.AddItem(item);
        equipped.Remove(slot);
        OnUnequipped?.Invoke(slot);
    }
 
    // Tháo đồ KHÔNG trả về inventory (dùng khi swap giữa 2 ô equipment)
    public ItemData UnequipSilent(EquipSlot slot)
    {
        if (!equipped.TryGetValue(slot, out var item)) return null;
        PlayerStats.Instance.ApplyModifiers(item.stats, add: false);
        equipped.Remove(slot);
        OnUnequipped?.Invoke(slot);
        return item;
    }
 
    public ItemData GetEquipped(EquipSlot slot) =>
        equipped.TryGetValue(slot, out var item) ? item : null;
 
    public Dictionary<EquipSlot, ItemData> GetAllEquipped() => equipped;
}
 