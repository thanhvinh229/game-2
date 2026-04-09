using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;

public class EquipmentUI : MonoBehaviour 
{
    [System.Serializable]
    public class EquipSlotUI {
        public EquipSlot slot;
        public Image     icon;       // ItemIcon trong ô
        public Image     emptyIcon;  // icon gợi ý (hình đầu/kiếm...) khi trống
        public Button    button;     // nút tháo khi click
    }

    [SerializeField] private List<EquipSlotUI> slotUIs;
    [SerializeField] private TextMeshProUGUI   attackText;
    [SerializeField] private TextMeshProUGUI   defenseText;
    [SerializeField] private TextMeshProUGUI   hpText;

    void Start()
    {
        EquipmentManager.Instance.OnEquipped   += (_, __) => Refresh();
        EquipmentManager.Instance.OnUnequipped += (_)     => Refresh();

        // Click vào ô đang mặc đồ → tháo ra
        foreach (var ui in slotUIs) {
            var s = ui.slot;
            ui.button.onClick.AddListener(() =>
                EquipmentManager.Instance.Unequip(s));
        }

        // Gắn DropHandler lên từng ô để nhận drag từ SlotUI
        foreach (var ui in slotUIs)
            AddDropHandler(ui);

        Refresh();
    }

    void Refresh()
    {
        foreach (var ui in slotUIs) {
            var item    = EquipmentManager.Instance.GetEquipped(ui.slot);
            bool has    = item != null;
            ui.icon     .enabled = has;
            ui.emptyIcon.enabled = !has;
            if (has) ui.icon.sprite = item.icon;
        }

        // Cập nhật stat text
        if (attackText  != null) attackText .text = $"ATK  {PlayerStats.Instance.Attack:F0}";
        if (defenseText != null) defenseText.text = $"DEF  {PlayerStats.Instance.Defense:F0}";
        if (hpText      != null) hpText     .text = $"HP   {PlayerStats.Instance.MaxHP:F0}";
    }

    // --- Thêm Drop handler động, không cần script riêng ---
    void AddDropHandler(EquipSlotUI ui)
    {
        var trigger = ui.icon.gameObject.GetComponent<EventTrigger>()
                   ?? ui.icon.gameObject.AddComponent<EventTrigger>();

        var entry = new EventTrigger.Entry { eventID = EventTriggerType.Drop };
        var capturedSlot = ui.slot;

        entry.callback.AddListener((data) => {
            var src = SlotUI.DragSource;
            if (src == null) return;

            var slots = InventoryManager.Instance.GetSlots();
            var item  = slots[src.SlotIndex].item;
            if (item == null || item.equipSlot != capturedSlot) return;

            EquipmentManager.Instance.Equip(item);
            InventoryManager.Instance.RemoveItem(src.SlotIndex);
        });

        trigger.triggers.Add(entry);
    }
}
