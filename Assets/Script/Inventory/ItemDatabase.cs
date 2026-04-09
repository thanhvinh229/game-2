using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject {
    private static ItemDatabase _instance;

     [SerializeField] private List<ItemData> allItems = new();
    [SerializeField] private List<DropEntry> dropPrefabs = new();

    [System.Serializable]
    public class DropEntry {
        public string itemId;
        public GameObject prefab;
    }

    // Gọi Resources.Load một lần rồi cache
    static ItemDatabase Get() {
        if (_instance == null)
            _instance = Resources.Load<ItemDatabase>("ItemDatabase");
        return _instance;
    }

    public static ItemData GetById(string id) =>
        Get().allItems.FirstOrDefault(i => i.itemId == id);

    public static GameObject GetDropPrefab(string id) =>
        Get().dropPrefabs.FirstOrDefault(e => e.itemId == id)?.prefab;
}
