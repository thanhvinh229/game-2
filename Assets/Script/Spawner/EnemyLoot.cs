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
 
    [System.Serializable]
    public class RarityPool
    {
        public ItemRarity rarity;
        [Range(0f, 1f)]
        [Tooltip("Xác suất pool này được chọn")]
        public float poolChance = 0.5f;
        public List<LootEntry> entries = new();
    }
 
    [Header("Số item tối đa rơi ra mỗi lần")]
    [SerializeField] private int maxDrops = 3;
 
    [Header("Pool theo độ hiếm — mỗi pool có chance riêng")]
    [SerializeField] private List<RarityPool> rarityPools = new();
 
    public void Drop()
    {
        if (ItemDropManager.Instance == null) return;
 
        int dropCount = 0;
 
        foreach (var pool in rarityPools)
        {
            if (dropCount >= maxDrops) break;
            if (pool.entries.Count == 0) continue;
 
            // Roll xem pool này có được kích hoạt không
            if (Random.value > pool.poolChance) continue;
 
            // Trong pool đó, roll từng entry
            foreach (var entry in pool.entries)
            {
                if (dropCount >= maxDrops) break;
                if (entry.item == null) continue;
                if (Random.value > entry.dropChance) continue;
 
                int qty = Random.Range(entry.minQuantity, entry.maxQuantity + 1);

                 // Gọi ItemDropManager để rớt đồ ra đất tại vị trí quái chết
                if (ItemDropManager.Instance != null)
                {
                   ItemDropManager.Instance.DropItem(entry.item, qty, transform.position);
                    Debug.Log($"[Loot] Đã rơi ra đất: [{pool.rarity}] {entry.item.itemName} x{qty}");
                }
                else
                {
                  Debug.LogError("[Loot] Thiếu ItemDropManager trong Scene!");
                }
 
                dropCount++;
            }
        }
    }
}   
