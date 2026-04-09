using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SlotUI : MonoBehaviour ,
IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Image highlight;

    private int slotIndex;
    private InventorySlot data;
    private static SlotUI dragSource;
    private static GameObject dragGhost;

    public void Init(int index) => slotIndex = index;
    public static SlotUI DragSource => dragSource;
    public int SlotIndex => slotIndex;

    public void UpdateDisplay(InventorySlot slot) {
        data = slot;
        bool hasItem = !slot.IsEmpty;
        itemIcon.enabled = hasItem;
        quantityText.enabled = hasItem && slot.item.isStackable;

        if (hasItem) {
            itemIcon.sprite = slot.item.icon;
            quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
        }
    }

    // --- Drag & Drop ---
    public void OnBeginDrag(PointerEventData e) {
        if (data.IsEmpty) return;
        dragSource = this;
        // Tạo ghost icon theo con trỏ
        dragGhost = new GameObject("DragGhost");
        dragGhost.transform.SetParent(transform.root, false);
        var img = dragGhost.AddComponent<Image>();
        img.sprite = data.item.icon;
        img.raycastTarget = false;
        dragGhost.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 60);
    }

    public void OnDrag(PointerEventData e) {
        if (dragGhost != null)
            dragGhost.transform.position = e.position;
    }

    public void OnEndDrag(PointerEventData e) {
        Destroy(dragGhost);
        dragSource = null;
    }

    public void OnDrop(PointerEventData e) {
        if (dragSource != null && dragSource != this)
            InventoryManager.Instance.SwapSlots(dragSource.slotIndex, slotIndex);
    }

    // --- Hover tooltip ---
    public void OnPointerEnter(PointerEventData e) {
        if (!data.IsEmpty)
            ItemTooltip.Instance.Show(data.item, transform.position);
        highlight.enabled = true;
    }

    public void OnPointerExit(PointerEventData e) {
        ItemTooltip.Instance.Hide();
        highlight.enabled = false;
    }

    // --- Right click context menu ---
    public void OnPointerClick(PointerEventData e) {
        if (e.button == PointerEventData.InputButton.Right && !data.IsEmpty)
            ItemContextMenu.Instance.Show(slotIndex, data, transform.position);
    }
}
