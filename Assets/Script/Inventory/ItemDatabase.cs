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
 
        // FIX: Báo lỗi rõ ràng thay vì crash
        if (_instance == null)
            Debug.LogError("[ItemDatabase] Không tìm thấy 'ItemDatabase' trong Resources! " +
                           "Hãy để file .asset vào thư mục Assets/Resources/ và đặt tên là 'ItemDatabase'.");
 
        return _instance;
    }
 
    // FIX: Null-guard trước khi gọi .allItems để tránh NullReferenceException
    public static ItemData GetById(string id) {
        var db = Get();
        if (db == null) return null;
        return db.allItems.FirstOrDefault(i => i.itemId == id);
    }
 
    // FIX: Tương tự cho GetDropPrefab
    public static GameObject GetDropPrefab(string id) {
        var db = Get();
        if (db == null) return null;
        return db.dropPrefabs.FirstOrDefault(e => e.itemId == id)?.prefab;
    }
}

