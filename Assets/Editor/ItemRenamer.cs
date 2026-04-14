// ItemRenamer.cs
// Đặt vào: Assets/Editor/ItemRenamer.cs
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class ItemRenamer : EditorWindow
{
    // Prefix theo rarity
    private static string GetPrefix(ItemRarity rarity) => rarity switch {
        ItemRarity.Common    => "C",
        ItemRarity.Uncommon  => "U",
        ItemRarity.Rare      => "R",
        ItemRarity.Epic      => "E",
        ItemRarity.Legendary => "L",
        _                    => "C"
    };

    [MenuItem("Tools/Rename Items by Rarity")]
    public static void RenameAll()
    {
        // Tìm tất cả ItemData trong project
        var guids = AssetDatabase.FindAssets("t:ItemData");
        int renamed = 0;
        int skipped = 0;

        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var item = AssetDatabase.LoadAssetAtPath<ItemData>(path);
            if (item == null) continue;

            string prefix    = GetPrefix(item.rarity);
            string oldFile   = Path.GetFileNameWithoutExtension(path);
            string folder    = Path.GetDirectoryName(path);
            string ext       = Path.GetExtension(path);

            // Bỏ qua nếu đã có prefix đúng rồi
            if (oldFile.StartsWith(prefix + "_"))
            {
                skipped++;
                continue;
            }

            // Xóa prefix cũ nếu có (C_, U_, R_, E_, L_)
            string cleanName = oldFile;
            foreach (var p in new[]{"C_","U_","R_","E_","L_"})
            {
                if (cleanName.StartsWith(p))
                {
                    cleanName = cleanName.Substring(p.Length);
                    break;
                }
            }

            string newFile = $"{prefix}_{cleanName}";
            string newPath = Path.Combine(folder, newFile + ext).Replace("\\", "/");

            // Đổi itemName trong asset luôn
            string oldItemName = item.itemName;
            if (!item.itemName.StartsWith($"[{prefix}] "))
            {
                // Xóa tag cũ nếu có
                foreach (var p in new[]{"[C] ","[U] ","[R] ","[E] ","[L] "})
                    if (item.itemName.StartsWith(p))
                    { item.itemName = item.itemName.Substring(p.Length); break; }

                item.itemName = $"[{prefix}] {item.itemName}";
                EditorUtility.SetDirty(item);
            }

            // Đổi tên file asset
            var error = AssetDatabase.RenameAsset(path, newFile);
            if (string.IsNullOrEmpty(error))
            {
                renamed++;
                Debug.Log($"Đổi tên: {oldFile} → {newFile} | itemName: {oldItemName} → {item.itemName}");
            }
            else
            {
                Debug.LogError($"Lỗi đổi tên {oldFile}: {error}");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(
            "Hoàn thành!",
            $"Đã đổi tên: {renamed} item\nBỏ qua (đã đúng): {skipped} item",
            "OK"
        );
    }

    // Menu thứ 2: chỉ xem preview, không đổi thật
    [MenuItem("Tools/Preview Item Rename (Dry Run)")]
    public static void PreviewRename()
    {
        var guids = AssetDatabase.FindAssets("t:ItemData");
        Debug.Log($"=== PREVIEW RENAME — {guids.Length} items ===");

        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var item = AssetDatabase.LoadAssetAtPath<ItemData>(path);
            if (item == null) continue;

            string prefix  = GetPrefix(item.rarity);
            string oldFile = Path.GetFileNameWithoutExtension(path);

            if (oldFile.StartsWith(prefix + "_"))
            {
                Debug.Log($"[OK] {oldFile} — đã đúng");
                continue;
            }

            string cleanName = oldFile;
            foreach (var p in new[]{"C_","U_","R_","E_","L_"})
                if (cleanName.StartsWith(p)) { cleanName = cleanName.Substring(p.Length); break; }

            Debug.Log($"[SẼ ĐỔI] {oldFile}  →  {prefix}_{cleanName}  |  rarity: {item.rarity}");
        }

        Debug.Log("=== HẾT PREVIEW — Vào Tools/Rename Items by Rarity để thực hiện ===");
    }
}
#endif
