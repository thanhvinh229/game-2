// EquipDropZone.cs
// Gắn lên TỪNG ô trang bị: SlotHead, SlotChest, SlotWeapon, SlotLegs, SlotShield, SlotRing
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class EquipDropZone : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    [SerializeField] public EquipSlot slot;
    [SerializeField] private Image itemIcon;    // Image hiện icon item đã mặc

    void Start()
    {
        // Đảm bảo Image trên root nhận Raycast
        var img = GetComponent<Image>();
        img.raycastTarget = true;
        if (img.color.a == 0 || img.sprite == null)
            img.color = new Color(1, 1, 1, 0.01f); // gần trong suốt nhưng vẫn nhận raycast

        // Lắng nghe event equip/unequip để refresh icon
        EquipmentManager.Instance.OnEquipped   += OnEquipped;
        EquipmentManager.Instance.OnUnequipped += OnUnequipped;

        Refresh();
    }

    void OnDestroy()
    {
        if (EquipmentManager.Instance == null) return;
        EquipmentManager.Instance.OnEquipped   -= OnEquipped;
        EquipmentManager.Instance.OnUnequipped -= OnUnequipped;
    }

    // ── Kéo item từ inventory thả vào ──
    public void OnDrop(PointerEventData e)
    {
        Debug.Log($"[EquipDropZone] OnDrop fired trên slot: {slot}");

        var src = SlotUI.DragSource;
        if (src == null)
        {
            Debug.Log("[EquipDropZone] DragSource null!");
            return;
        }

        var slots = InventoryManager.Instance.GetSlots();
        var item  = slots[src.SlotIndex].item;

        if (item == null)
        {
            Debug.Log("[EquipDropZone] Item null!");
            return;
        }

        if (item.equipSlot != slot)
        {
            Debug.Log($"[EquipDropZone] {item.itemName} cần [{item.equipSlot}], ô này là [{slot}]");
            return;
        }

        EquipmentManager.Instance.Equip(item);
        InventoryManager.Instance.RemoveItem(src.SlotIndex);
        Debug.Log($"[EquipDropZone] Trang bị thành công: {item.itemName} → {slot}");
    }

    // ── Click chuột phải để tháo ──
    public void OnPointerClick(PointerEventData e)
    {
        if (e.button != PointerEventData.InputButton.Right) return;
        var item = EquipmentManager.Instance.GetEquipped(slot);
        if (item == null) return;
        EquipmentManager.Instance.Unequip(slot);
        Debug.Log($"[EquipDropZone] Tháo: {item.itemName} từ {slot}");
    }

    void Refresh()
    {
        var item = EquipmentManager.Instance.GetEquipped(slot);
        if (itemIcon != null)
        {
            itemIcon.enabled = item != null;
            if (item != null) itemIcon.sprite = item.icon;
        }
    }

    void OnEquipped(EquipSlot s, ItemData _)  { if (s == slot) Refresh(); }
    void OnUnequipped(EquipSlot s)             { if (s == slot) Refresh(); }
}
