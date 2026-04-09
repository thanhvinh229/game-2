using UnityEngine;
using System.Collections.Generic;

public enum ItemType { Weapon, Armor, Consumable, Quest, Material }
public enum EquipSlot { None, Head, Chest, Legs, Weapon, Shield, Ring }


[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string itemId;
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Type")]
    public ItemType type;
    public EquipSlot equipSlot;  // None nếu không equip được

    [Header("Stack")]
    public bool isStackable = false;
    public int maxStackSize = 1;

    [Header("Stats")]
    public float weight = 0.5f;
    public int value = 10;           // giá bán
    public List<StatModifier> stats = new();
}

[System.Serializable]
public class StatModifier {
    public string statName;   // "Attack", "Defense", "HP"...
    public float value;
}

