using UnityEngine;
using System.Collections.Generic;

public enum ItemType
{
    Weapon,
    Armor,
    Consumable,
    Quest,
    Material      // <-- THÊM MỚI
}
 
public enum EquipSlot
{
    None,
    Head,
    Chest,
    Legs,
    Weapon,
    Shield,
    Ring
}
 
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Thông tin cơ bản")]
    public string itemId;
    public string itemName;
    [TextArea(2, 4)]
    public string description;
    public Sprite icon;
 
    [Header("Phân loại")]
    public ItemType  type;
    public EquipSlot equipSlot;
 
    [Header("Stack & Trọng lượng")]
    public bool  isStackable  = false;
    public int   maxStackSize = 1;
    public float weight       = 1f;
 
    [Header("Giá trị")]
    public int value = 10;
 
    [Header("Chỉ số (Stats)")]
    public List<StatModifier> stats = new();
}
 
[System.Serializable]
public class StatModifier
{
    public string statName;  // "Attack" | "Defense" | "HP" | "Mana"
    public float  value;
}

