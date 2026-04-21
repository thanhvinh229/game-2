using UnityEngine;
using System.Collections.Generic;

public enum ItemType
{
    Weapon,
    Armor,
    Jewelry,
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
    Hand,
    Legs2,
    Weapon,
     Weapon2,
    Shield,
    Ring,
    Ring2 
}
public enum ItemRarity
{
    Common,     // Trắng   — đồ thường
    Uncommon,   // Xanh lá — đồ tốt hơn
    Rare,       // Xanh dương — đồ hiếm
    Epic,       // Tím     — đồ rất hiếm
    Legendary   // Vàng    — đồ huyền thoại
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
    public ItemType    type;
    public EquipSlot   equipSlot;
    public ItemRarity  rarity = ItemRarity.Common; // ← chọn trong Inspector
 
    [Header("Stack & Trọng lượng")]
    public bool  isStackable   = false;
    public int   maxStackSize  = 1;
    public float weight        = 1f;
 
    [Header("Giá trị")]
    public int value = 10;
 
    [Header("Chỉ số (Stats)")]
    public List<StatModifier> stats = new();
 
    // Màu theo rarity — dùng cho UI
    public Color RarityColor => rarity switch {
        ItemRarity.Common    => new Color(0.80f, 0.80f, 0.80f), // trắng xám
        ItemRarity.Uncommon  => new Color(0.30f, 0.85f, 0.30f), // xanh lá
        ItemRarity.Rare      => new Color(0.30f, 0.60f, 1.00f), // xanh dương
        ItemRarity.Epic      => new Color(0.65f, 0.35f, 1.00f), // tím
        ItemRarity.Legendary => new Color(1.00f, 0.80f, 0.10f), // vàng
        _                    => Color.white
    };
 
    public string RarityName => rarity switch {
        ItemRarity.Common    => "Thường",
        ItemRarity.Uncommon  => "Tốt",
        ItemRarity.Rare      => "Hiếm",
        ItemRarity.Epic      => "Sử thi",
        ItemRarity.Legendary => "Huyền thoại",
        _                    => ""
    };
}
 
[System.Serializable]
public class StatModifier
{
    public string statName;
    public float  value;
}

