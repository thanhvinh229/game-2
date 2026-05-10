using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
 
public class ItemDatabaseBuilder
{
    [MenuItem("Tools/Auto Fill Item Database")]
    static void AutoFillDatabase()
    {
        // Tìm file ItemDatabase.asset trong Resources
        var db = Resources.Load<ItemDatabase>("ItemDatabase");
        if (db == null)
        {
            Debug.LogError("Không tìm thấy ItemDatabase trong Assets/Resources/! " +
                           "Hãy tạo file bằng cách: chuột phải vào Resources → Create → Inventory → Item Database → đặt tên 'ItemDatabase'");
            return;
        }
 
        // Tìm tất cả ItemData trong toàn bộ project
        string[] guids = AssetDatabase.FindAssets("t:ItemData", new[] { "Assets/Data/Items" });
        var items = new List<ItemData>();
 
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var item = AssetDatabase.LoadAssetAtPath<ItemData>(path);
            if (item != null)
                items.Add(item);
        }
 
        // Gán vào database và lưu
        SerializedObject so = new SerializedObject(db);
        SerializedProperty allItemsProp = so.FindProperty("allItems");
 
        allItemsProp.ClearArray();
        for (int i = 0; i < items.Count; i++)
        {
            allItemsProp.InsertArrayElementAtIndex(i);
            allItemsProp.GetArrayElementAtIndex(i).objectReferenceValue = items[i];
        }
 
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(db);
        AssetDatabase.SaveAssets();
 
        Debug.Log($"[ItemDatabase] Đã tự động thêm {items.Count} items vào database!");
    }
}

