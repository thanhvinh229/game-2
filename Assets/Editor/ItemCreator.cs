#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
 

public class ItemCreator : EditorWindow
{
    [MenuItem("Tools/Create All RPG Items")]
    public static void CreateAllItems()
    {
        // Tạo thư mục nếu chưa có
        CreateFolder("Assets/Data");
        CreateFolder("Assets/Data/Items");
        CreateFolder("Assets/Data/Items/Weapons");
        CreateFolder("Assets/Data/Items/Armor");
        CreateFolder("Assets/Data/Items/Consumables");
        CreateFolder("Assets/Data/Items/Quest");
        CreateFolder("Assets/Data/Items/Materials");
 
        CreateWeapons();
        CreateArmor();
        CreateConsumables();
        CreateQuestItems();
        CreateMaterials();
 
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog(
            "Hoàn thành!",
            "Đã tạo tất cả item tại Assets/Data/Items/\nKéo vào ItemDatabase để hoàn tất.",
            "OK"
        );
    }
 
    // ─────────────────────────────────────────
    // VŨ KHÍ
    // ─────────────────────────────────────────
    static void CreateWeapons()
    {
        // ── Kiếm (Sword) ──
        Make("WP_Sword_Rusty", "Kiếm Gỉ Sét", ItemType.Weapon, EquipSlot.Weapon,
            "Thanh kiếm cũ kỹ, gỉ sét nhưng vẫn còn dùng được.",
            attack: 8, value: 20, weight: 2.5f,
            folder: "Assets/Data/Items/Weapons");
 
        Make("WP_Sword_Iron", "Kiếm Sắt", ItemType.Weapon, EquipSlot.Weapon,
            "Kiếm sắt tiêu chuẩn của lính canh.",
            attack: 18, value: 80, weight: 3f,
            folder: "Assets/Data/Items/Weapons");
 
        Make("WP_Sword_Steel", "Kiếm Thép", ItemType.Weapon, EquipSlot.Weapon,
            "Được rèn từ thép tốt, lưỡi kiếm bén và chắc chắn.",
            attack: 30, value: 250, weight: 3.2f,
            folder: "Assets/Data/Items/Weapons");
 
        Make("WP_Sword_Flamebrand", "Hỏa Kiếm", ItemType.Weapon, EquipSlot.Weapon,
            "Thanh kiếm được phong ấn ngọn lửa ma thuật. Sát thương rất cao.",
            attack: 52, value: 900, weight: 3.5f,
            folder: "Assets/Data/Items/Weapons");
 
        Make("WP_Sword_Excalibur", "Kiếm Huyền Thoại", ItemType.Weapon, EquipSlot.Weapon,
            "Vũ khí của anh hùng. Ánh sáng thánh thiêng tỏa ra từ lưỡi kiếm.",
            attack: 85, value: 5000, weight: 2.8f,
            folder: "Assets/Data/Items/Weapons");
 
        // ── Dao găm (Dagger) ──
        Make("WP_Dagger_Iron", "Dao Găm Sắt", ItemType.Weapon, EquipSlot.Weapon,
            "Nhỏ gọn, nhanh nhẹn. Thích hợp cho kẻ ẩn thân.",
            attack: 12, value: 60, weight: 0.8f,
            folder: "Assets/Data/Items/Weapons");
 
        Make("WP_Dagger_Poison", "Dao Găm Độc", ItemType.Weapon, EquipSlot.Weapon,
            "Lưỡi dao được tẩm độc dược. Kẻ thù sẽ dần suy yếu.",
            attack: 20, value: 380, weight: 0.9f,
            folder: "Assets/Data/Items/Weapons");
 
        // ── Rìu (Axe) ──
        Make("WP_Axe_Iron", "Rìu Sắt", ItemType.Weapon, EquipSlot.Weapon,
            "Vũ khí của chiến binh. Nặng nhưng sức công phá kinh khủng.",
            attack: 24, value: 120, weight: 5f,
            folder: "Assets/Data/Items/Weapons");
 
        Make("WP_Axe_Battle", "Chiến Rìu", ItemType.Weapon, EquipSlot.Weapon,
            "Rìu hai tay của dũng sĩ, mỗi nhát chém có thể hạ gục kẻ thù.",
            attack: 45, value: 600, weight: 6.5f,
            folder: "Assets/Data/Items/Weapons");
 
        // ── Gậy phép (Staff) ──
        Make("WP_Staff_Wood", "Gậy Gỗ", ItemType.Weapon, EquipSlot.Weapon,
            "Gậy phép đơn giản, phù hợp cho pháp sư mới học.",
            attack: 6, value: 40, weight: 1.5f,
            folder: "Assets/Data/Items/Weapons",
            mana: 20);
 
        Make("WP_Staff_Crystal", "Gậy Pha Lê", ItemType.Weapon, EquipSlot.Weapon,
            "Đầu gậy gắn tinh thể ma thuật, khuếch đại sức mạnh phép thuật.",
            attack: 15, value: 500, weight: 2f,
            folder: "Assets/Data/Items/Weapons",
            mana: 50);
 
        Make("WP_Staff_Arcane", "Gậy Huyền Bí", ItemType.Weapon, EquipSlot.Weapon,
            "Cổ vật của đại pháp sư. Mana hồi phục nhanh hơn khi cầm.",
            attack: 28, value: 2000, weight: 2.2f,
            folder: "Assets/Data/Items/Weapons",
            mana: 100);
    }
 
    // ─────────────────────────────────────────
    // GIÁP
    // ─────────────────────────────────────────
    static void CreateArmor()
    {
        // ── Mũ (Head) ──
        Make("AR_Helmet_Leather", "Mũ Da", ItemType.Armor, EquipSlot.Head,
            "Mũ da thô sơ, bảo vệ đầu khỏi những đòn nhẹ.",
            defense: 5, value: 50, weight: 1f,
            folder: "Assets/Data/Items/Armor");
 
        Make("AR_Helmet_Iron", "Mũ Sắt", ItemType.Armor, EquipSlot.Head,
            "Mũ sắt tiêu chuẩn của lính canh vương quốc.",
            defense: 12, value: 150, weight: 2f,
            folder: "Assets/Data/Items/Armor");
 
        Make("AR_Helmet_Knight", "Mũ Kỵ Sĩ", ItemType.Armor, EquipSlot.Head,
            "Mũ giáp của kỵ sĩ, che kín toàn bộ khuôn mặt.",
            defense: 22, value: 500, weight: 3f,
            folder: "Assets/Data/Items/Armor");
 
        // ── Áo giáp (Chest) ──
        Make("AR_Chest_Cloth", "Áo Vải", ItemType.Armor, EquipSlot.Chest,
            "Áo vải bình thường, không có tác dụng bảo vệ nhiều.",
            defense: 3, value: 15, weight: 0.5f,
            folder: "Assets/Data/Items/Armor");
 
        Make("AR_Chest_Leather", "Áo Giáp Da", ItemType.Armor, EquipSlot.Chest,
            "Giáp da linh hoạt, phù hợp cho cung thủ và thích khách.",
            defense: 10, value: 120, weight: 2f,
            folder: "Assets/Data/Items/Armor");
 
        Make("AR_Chest_Chainmail", "Giáp Xích", ItemType.Armor, EquipSlot.Chest,
            "Lưới sắt đan chặt, bảo vệ tốt trước vũ khí sắc bén.",
            defense: 20, value: 400, weight: 5f,
            folder: "Assets/Data/Items/Armor");
 
        Make("AR_Chest_Plate", "Giáp Thép Tấm", ItemType.Armor, EquipSlot.Chest,
            "Bộ giáp của chiến binh hạng nặng. Phòng thủ vượt trội.",
            defense: 35, value: 1200, weight: 8f,
            folder: "Assets/Data/Items/Armor");
 
        Make("AR_Chest_Dragon", "Giáp Rồng", ItemType.Armor, EquipSlot.Chest,
            "Đúc từ vảy rồng huyền thoại. Gần như bất khả xâm phạm.",
            defense: 60, value: 8000, weight: 6f,
            folder: "Assets/Data/Items/Armor",
            hp: 50);
 
        // ── Quần giáp (Legs) ──
        Make("AR_Legs_Leather", "Quần Da", ItemType.Armor, EquipSlot.Legs,
            "Bảo vệ đôi chân, đủ linh hoạt để di chuyển nhanh.",
            defense: 7, value: 90, weight: 1.5f,
            folder: "Assets/Data/Items/Armor");
 
        Make("AR_Legs_Plate", "Quần Giáp Thép", ItemType.Armor, EquipSlot.Legs,
            "Giáp chân hạng nặng, giảm tốc độ nhưng bảo vệ tuyệt đối.",
            defense: 18, value: 350, weight: 4f,
            folder: "Assets/Data/Items/Armor");
 
        // ── Khiên (Shield) ──
        Make("AR_Shield_Wood", "Khiên Gỗ", ItemType.Armor, EquipSlot.Shield,
            "Khiên gỗ thô sơ. Không chắc nhưng tốt hơn không có gì.",
            defense: 8, value: 40, weight: 2f,
            folder: "Assets/Data/Items/Armor");
 
        Make("AR_Shield_Iron", "Khiên Sắt", ItemType.Armor, EquipSlot.Shield,
            "Khiên sắt tròn, tiêu chuẩn của lính bộ binh.",
            defense: 18, value: 200, weight: 4f,
            folder: "Assets/Data/Items/Armor");
 
        Make("AR_Shield_Tower", "Khiên Tháp", ItemType.Armor, EquipSlot.Shield,
            "Khiên khổng lồ che kín toàn thân. Phòng thủ tối thượng.",
            defense: 35, value: 800, weight: 7f,
            folder: "Assets/Data/Items/Armor");
 
        // ── Nhẫn (Ring) ──
        Make("AR_Ring_Copper", "Nhẫn Đồng", ItemType.Armor, EquipSlot.Ring,
            "Nhẫn đồng bình thường, có khắc bùa bảo vệ nhỏ.",
            defense: 3, value: 80, weight: 0.1f,
            folder: "Assets/Data/Items/Armor",
            hp: 10);
 
        Make("AR_Ring_Silver", "Nhẫn Bạc", ItemType.Armor, EquipSlot.Ring,
            "Nhẫn bạc được phù phép, tăng sức sống và phòng thủ.",
            defense: 8, value: 400, weight: 0.1f,
            folder: "Assets/Data/Items/Armor",
            hp: 30);
 
        Make("AR_Ring_Gold", "Nhẫn Vàng Ma Thuật", ItemType.Armor, EquipSlot.Ring,
            "Nhẫn vàng của pháp sư cổ đại, chứa đựng năng lượng huyền bí.",
            defense: 5, value: 1500, weight: 0.1f,
            folder: "Assets/Data/Items/Armor",
            hp: 50, mana: 40);
    }
 
    // ─────────────────────────────────────────
    // TIÊU THỤ
    // ─────────────────────────────────────────
    static void CreateConsumables()
    {
        MakeConsumable("CO_Potion_HP_Small", "Thuốc Hồi Máu Nhỏ",
            "Hồi phục 30 HP. Vị đắng nhưng hiệu quả tức thì.",
            hp: 30, value: 25, maxStack: 10,
            folder: "Assets/Data/Items/Consumables");
 
        MakeConsumable("CO_Potion_HP_Medium", "Thuốc Hồi Máu",
            "Hồi phục 80 HP. Được bán phổ biến ở các thị trấn.",
            hp: 80, value: 80, maxStack: 10,
            folder: "Assets/Data/Items/Consumables");
 
        MakeConsumable("CO_Potion_HP_Large", "Thuốc Hồi Máu Lớn",
            "Hồi phục 200 HP. Dùng trong tình huống nguy cấp.",
            hp: 200, value: 200, maxStack: 5,
            folder: "Assets/Data/Items/Consumables");
 
        MakeConsumable("CO_Potion_MP_Small", "Thuốc Hồi Mana Nhỏ",
            "Hồi phục 25 Mana. Cần thiết cho pháp sư.",
            mana: 25, value: 30, maxStack: 10,
            folder: "Assets/Data/Items/Consumables");
 
        MakeConsumable("CO_Potion_MP_Medium", "Thuốc Hồi Mana",
            "Hồi phục 60 Mana. Tiêu chuẩn cho cung pháp.",
            mana: 60, value: 90, maxStack: 10,
            folder: "Assets/Data/Items/Consumables");
 
        MakeConsumable("CO_Potion_MP_Large", "Thuốc Hồi Mana Lớn",
            "Hồi phục toàn bộ Mana. Cực kỳ quý hiếm.",
            mana: 150, value: 300, maxStack: 5,
            folder: "Assets/Data/Items/Consumables");
 
        MakeConsumable("CO_Elixir_Full", "Thần Dược",
            "Hồi phục toàn bộ HP và Mana. Bí phương của đại pháp sư.",
            hp: 500, mana: 200, value: 1000, maxStack: 3,
            folder: "Assets/Data/Items/Consumables");
 
        MakeConsumable("CO_Antidote", "Thuốc Giải Độc",
            "Loại bỏ mọi trạng thái trúng độc.",
            hp: 10, value: 40, maxStack: 10,
            folder: "Assets/Data/Items/Consumables");
 
        MakeConsumable("CO_Bread", "Bánh Mì",
            "Thức ăn đơn giản. Hồi phục HP chậm nhưng ổn định.",
            hp: 20, value: 10, maxStack: 20,
            folder: "Assets/Data/Items/Consumables");
 
        MakeConsumable("CO_Meat_Roasted", "Thịt Nướng",
            "Thịt thú săn được nướng thơm. Hồi khá nhiều HP.",
            hp: 60, value: 35, maxStack: 10,
            folder: "Assets/Data/Items/Consumables");
    }
 
    // ─────────────────────────────────────────
    // QUEST ITEMS
    // ─────────────────────────────────────────
    static void CreateQuestItems()
    {
        MakeQuest("QT_Ancient_Scroll", "Cuộn Giấy Cổ",
            "Một cuộn giấy chứa bí mật về kho báu bị lãng quên. Không thể bán.",
            value: 0, "Assets/Data/Items/Quest");
 
        MakeQuest("QT_Kings_Seal", "Ấn Quốc Vương",
            "Con dấu của nhà vua. Ai sở hữu vật này sẽ có thể vào hoàng cung.",
            value: 0, "Assets/Data/Items/Quest");
 
        MakeQuest("QT_Dragon_Eye", "Mắt Rồng",
            "Được lấy từ rồng cổ đại. Tỏa ra ánh sáng kỳ lạ.",
            value: 0, "Assets/Data/Items/Quest");
    }
 
    // ─────────────────────────────────────────
    // NGUYÊN LIỆU
    // ─────────────────────────────────────────
    static void CreateMaterials()
    {
        MakeMaterial("MT_Iron_Ore", "Quặng Sắt",
            "Quặng sắt thô. Có thể nấu chảy để rèn vũ khí.",
            value: 15, maxStack: 50, "Assets/Data/Items/Materials");
 
        MakeMaterial("MT_Iron_Ingot", "Thỏi Sắt",
            "Sắt đã được nấu chảy và đúc thành thỏi. Nguyên liệu cơ bản.",
            value: 35, maxStack: 50, "Assets/Data/Items/Materials");
 
        MakeMaterial("MT_Steel_Ingot", "Thỏi Thép",
            "Thép tinh luyện cao cấp, cần thiết để rèn giáp hạng nặng.",
            value: 120, maxStack: 30, "Assets/Data/Items/Materials");
 
        MakeMaterial("MT_Dragon_Scale", "Vảy Rồng",
            "Vảy rồng cực kỳ cứng. Nguyên liệu để đúc giáp huyền thoại.",
            value: 2000, maxStack: 10, "Assets/Data/Items/Materials");
 
        MakeMaterial("MT_Magic_Crystal", "Pha Lê Ma Thuật",
            "Tinh thể chứa đựng năng lượng phép thuật. Dùng để chế tác gậy phép.",
            value: 500, maxStack: 20, "Assets/Data/Items/Materials");
 
        MakeMaterial("MT_Herb_Common", "Cỏ Dược Thường",
            "Cây thuốc mọc hoang. Nguyên liệu chính để bào chế thuốc hồi máu.",
            value: 8, maxStack: 99, "Assets/Data/Items/Materials");
 
        MakeMaterial("MT_Herb_Rare", "Cỏ Dược Quý",
            "Cây thuốc hiếm. Chỉ mọc ở vùng núi cao. Dùng để nấu thần dược.",
            value: 150, maxStack: 20, "Assets/Data/Items/Materials");
 
        MakeMaterial("MT_Bone_Fragment", "Mảnh Xương Quái",
            "Xương của quái vật. Có thể bán hoặc dùng để crafting.",
            value: 12, maxStack: 50, "Assets/Data/Items/Materials");
 
        MakeMaterial("MT_Monster_Core", "Lõi Quái Vật",
            "Tinh hoa năng lượng của quái vật mạnh. Rất có giá trị.",
            value: 300, maxStack: 20, "Assets/Data/Items/Materials");
    }
 
    // ─────────────────────────────────────────
    // HELPER METHODS
    // ─────────────────────────────────────────
    static void Make(string id, string itemName, ItemType type, EquipSlot equipSlot,
        string desc, float attack = 0, float defense = 0, float hp = 0, float mana = 0,
        int value = 0, float weight = 1f, string folder = "Assets/Data/Items")
    {
        var item = ScriptableObject.CreateInstance<ItemData>();
        item.itemId      = id;
        item.itemName    = itemName;
        item.type        = type;
        item.equipSlot   = equipSlot;
        item.description = desc;
        item.value       = value;
        item.weight      = weight;
        item.isStackable = false;
        item.maxStackSize = 1;
        item.stats       = new System.Collections.Generic.List<StatModifier>();
 
        if (attack  > 0) item.stats.Add(new StatModifier { statName = "Attack",  value = attack  });
        if (defense > 0) item.stats.Add(new StatModifier { statName = "Defense", value = defense });
        if (hp      > 0) item.stats.Add(new StatModifier { statName = "HP",      value = hp      });
        if (mana    > 0) item.stats.Add(new StatModifier { statName = "Mana",    value = mana    });
 
        AssetDatabase.CreateAsset(item, $"{folder}/{id}.asset");
    }
 
    static void MakeConsumable(string id, string itemName, string desc,
        float hp = 0, float mana = 0, int value = 0, int maxStack = 10,
        string folder = "Assets/Data/Items/Consumables")
    {
        var item = ScriptableObject.CreateInstance<ItemData>();
        item.itemId       = id;
        item.itemName     = itemName;
        item.type         = ItemType.Consumable;
        item.equipSlot    = EquipSlot.None;
        item.description  = desc;
        item.value        = value;
        item.weight       = 0.3f;
        item.isStackable  = true;
        item.maxStackSize = maxStack;
        item.stats        = new System.Collections.Generic.List<StatModifier>();
 
        if (hp   > 0) item.stats.Add(new StatModifier { statName = "HP",   value = hp   });
        if (mana > 0) item.stats.Add(new StatModifier { statName = "Mana", value = mana });
 
        AssetDatabase.CreateAsset(item, $"{folder}/{id}.asset");
    }
 
    static void MakeQuest(string id, string itemName, string desc,
        int value, string folder)
    {
        var item = ScriptableObject.CreateInstance<ItemData>();
        item.itemId       = id;
        item.itemName     = itemName;
        item.type         = ItemType.Quest;
        item.equipSlot    = EquipSlot.None;
        item.description  = desc;
        item.value        = value;
        item.weight       = 0.1f;
        item.isStackable  = false;
        item.maxStackSize = 1;
        item.stats        = new System.Collections.Generic.List<StatModifier>();
        AssetDatabase.CreateAsset(item, $"{folder}/{id}.asset");
    }
 
    static void MakeMaterial(string id, string itemName, string desc,
        int value, int maxStack, string folder)
    {
        var item = ScriptableObject.CreateInstance<ItemData>();
        item.itemId       = id;
        item.itemName     = itemName;
        item.type         = ItemType.Material;
        item.equipSlot    = EquipSlot.None;
        item.description  = desc;
        item.value        = value;
        item.weight       = 0.2f;
        item.isStackable  = true;
        item.maxStackSize = maxStack;
        item.stats        = new System.Collections.Generic.List<StatModifier>();
        AssetDatabase.CreateAsset(item, $"{folder}/{id}.asset");
    }
 
    static void CreateFolder(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string parent = Path.GetDirectoryName(path).Replace("\\", "/");
            string folder = Path.GetFileName(path);
            AssetDatabase.CreateFolder(parent, folder);
        }
    }
}
#endif
