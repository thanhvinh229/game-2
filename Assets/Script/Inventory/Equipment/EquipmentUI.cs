using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;

public class EquipmentUI : MonoBehaviour 
{
    [System.Serializable]
    public class EquipSlotUI
    {
        public EquipSlot  slot;
        public Image      icon;       // Image hiện icon item khi đã mặc
        public Image      emptyIcon;  // Image icon gợi ý khi trống (có thể để None)
        public GameObject root;       // Kéo SlotHead / SlotWeapon / SlotChest... vào đây
    }
 
    [SerializeField] private List<EquipSlotUI> slotUIs;
 
    [Header("Stat Text (có thể để trống)")]
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI defenseText;
    [SerializeField] private TextMeshProUGUI hpText;
 
    void Start()
    {
        if (EquipmentManager.Instance == null)
        {
            Debug.LogError("[EquipmentUI] EquipmentManager chưa có trong scene!");
            return;
        }
 
        EquipmentManager.Instance.OnEquipped   += (_, __) => Refresh();
        EquipmentManager.Instance.OnUnequipped += (_)     => Refresh();
 
        foreach (var ui in slotUIs)
        {
            // Tự tìm root nếu chưa kéo vào Inspector
            if (ui.root == null && ui.icon != null)
                ui.root = ui.icon.transform.parent?.gameObject
                       ?? ui.icon.gameObject;
 
            if (ui.root == null)
            {
                Debug.LogWarning($"[EquipmentUI] Slot {ui.slot} không có root!");
                continue;
            }
 
            // Đảm bảo root có Image trong suốt để Raycast hit được
            var bg = ui.root.GetComponent<Image>();
            if (bg == null)
            {
                bg = ui.root.AddComponent<Image>();
                bg.color = Color.clear;
            }
            bg.raycastTarget = true;
 
            AddEventHandlers(ui);
        }
 
        Refresh();
    }
 
    void Refresh()
    {
        foreach (var ui in slotUIs)
        {
            var item = EquipmentManager.Instance.GetEquipped(ui.slot);
            bool has = item != null;
 
            if (ui.icon      != null) ui.icon     .enabled = has;
            if (ui.emptyIcon != null) ui.emptyIcon.enabled = !has;
            if (has && ui.icon != null) ui.icon.sprite = item.icon;
        }
 
        if (attackText  != null) attackText .text = $"ATK   {PlayerStats.Instance.Attack:F0}";
        if (defenseText != null) defenseText.text = $"DEF   {PlayerStats.Instance.Defense:F0}";
        if (hpText      != null) hpText     .text = $"HP    {PlayerStats.Instance.MaxHP:F0}";
    }
 
    void AddEventHandlers(EquipSlotUI ui)
    {
        var trigger = ui.root.GetComponent<EventTrigger>()
                   ?? ui.root.AddComponent<EventTrigger>();
 
        trigger.triggers.Clear();
 
        var capturedSlot = ui.slot;
 
        // ── DROP: kéo item từ inventory thả vào ──
        var dropEntry = new EventTrigger.Entry { eventID = EventTriggerType.Drop };
        dropEntry.callback.AddListener((_) =>
        {
            var src = SlotUI.DragSource;
            if (src == null)
            {
                Debug.Log("[EquipmentUI] Không có DragSource!");
                return;
            }
 
            var slots = InventoryManager.Instance.GetSlots();
            if (src.SlotIndex >= slots.Count) return;
 
            var item = slots[src.SlotIndex].item;
            if (item == null) return;
 
            if (item.equipSlot != capturedSlot)
            {
                Debug.Log($"[EquipmentUI] {item.itemName} cần slot [{item.equipSlot}], " +
                          $"không phải [{capturedSlot}]");
                return;
            }
 
            EquipmentManager.Instance.Equip(item);
            InventoryManager.Instance.RemoveItem(src.SlotIndex);
            Debug.Log($"[EquipmentUI] Trang bị: {item.itemName} → {capturedSlot}");
        });
        trigger.triggers.Add(dropEntry);
 
        // ── CLICK PHẢI: tháo đồ ──
        var clickEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        clickEntry.callback.AddListener((data) =>
        {
            var e = (PointerEventData)data;
            if (e.button != PointerEventData.InputButton.Right) return;
 
            var item = EquipmentManager.Instance.GetEquipped(capturedSlot);
            if (item == null) return;
 
            EquipmentManager.Instance.Unequip(capturedSlot);
            Debug.Log($"[EquipmentUI] Tháo: {item.itemName} từ {capturedSlot}");
        });
        trigger.triggers.Add(clickEntry);
    }
}