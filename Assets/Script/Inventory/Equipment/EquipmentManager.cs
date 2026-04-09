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
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
 
    public bool Equip(ItemData item)
    {
        if (item.equipSlot == EquipSlot.None) return false;
 
        // Tháo đồ cũ trả về inventory trước
        if (equipped.ContainsKey(item.equipSlot))
            Unequip(item.equipSlot);
 
        equipped[item.equipSlot] = item;
        PlayerStats.Instance.ApplyModifiers(item.stats, add: true);
        OnEquipped?.Invoke(item.equipSlot, item);
        return true;
    }
 
    public void Unequip(EquipSlot slot)
    {
        if (!equipped.TryGetValue(slot, out var item)) return;
        PlayerStats.Instance.ApplyModifiers(item.stats, add: false);
        InventoryManager.Instance.AddItem(item);
        equipped.Remove(slot);
        OnUnequipped?.Invoke(slot);
    }
 
    public ItemData GetEquipped(EquipSlot slot) =>
        equipped.TryGetValue(slot, out var item) ? item : null;
 
    public Dictionary<EquipSlot, ItemData> GetAllEquipped() => equipped;
}