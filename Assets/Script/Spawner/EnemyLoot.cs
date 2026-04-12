using UnityEngine;
using System.Collections.Generic;

public class EnemyLoot : MonoBehaviour
{
    [System.Serializable]
    public class LootEntry
    {
        public ItemData item;
        [Range(0f, 1f)]
        public float dropChance  = 0.3f;
        public int   minQuantity = 1;
        public int   maxQuantity = 1;
    }
 
    [Header("Bảng loot — kéo ItemData vào")]
    [SerializeField] private List<LootEntry> lootTable = new();
 
    [Header("Số item tối đa rơi ra mỗi lần")]
    [SerializeField] private int maxDrops = 3;
 
    public void Drop()
    {
        if (InventoryManager.Instance == null) return;
 
        int dropCount = 0;
        foreach (var entry in lootTable)
        {
            if (dropCount >= maxDrops) break;
            if (entry.item == null)   continue;
            if (Random.value > entry.dropChance) continue;
 
            int qty = Random.Range(entry.minQuantity, entry.maxQuantity + 1);
            bool ok = InventoryManager.Instance.AddItem(entry.item, qty);
 
            Debug.Log(ok
                ? $"[Loot] {entry.item.itemName} x{qty}"
                : $"[Loot] Inventory đầy, mất: {entry.item.itemName}");
 
            dropCount++;
        }
    }
}
