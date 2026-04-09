using UnityEngine;
using System.Collections.Generic;

public class SaveSystem : MonoBehaviour
{
    [System.Serializable]
    class SlotSave { public string itemId; public int qty; }

    [System.Serializable]
    class InvSave { public List<SlotSave> slots = new(); }

    public static void Save() {
        var inv = new InvSave();
        foreach (var slot in InventoryManager.Instance.GetSlots())
            inv.slots.Add(new SlotSave {
                itemId = slot.IsEmpty ? "" : slot.item.itemId,
                qty = slot.quantity
            });
        PlayerPrefs.SetString("inventory_save", JsonUtility.ToJson(inv));
    }

    public static void Load() {
        var json = PlayerPrefs.GetString("inventory_save", "");
        if (string.IsNullOrEmpty(json)) return;
        var inv = JsonUtility.FromJson<InvSave>(json);
        var slots = InventoryManager.Instance.GetSlots();
        for (int i = 0; i < inv.slots.Count && i < slots.Count; i++) {
            var s = inv.slots[i];
            slots[i].item = string.IsNullOrEmpty(s.itemId)
                ? null : ItemDatabase.GetById(s.itemId);
            slots[i].quantity = s.qty;
        }
    }
}
