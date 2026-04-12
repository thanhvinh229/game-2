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
 
    public void UpdateDisplay(InventorySlot slot)
    {
        data = slot;
        bool hasItem = !slot.IsEmpty;
        itemIcon.enabled     = hasItem;
        quantityText.enabled = hasItem && slot.item.isStackable;
 
        if (hasItem)
        {
            itemIcon.sprite   = slot.item.icon;
            quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
        }
    }
 
    // ── Drag từ inventory ──
    public void OnBeginDrag(PointerEventData e)
    {
        if (data.IsEmpty) return;
        dragSource = this;
 
        dragGhost = new GameObject("DragGhost");
        dragGhost.transform.SetParent(transform.root, false);
        var img = dragGhost.AddComponent<Image>();
        img.sprite        = data.item.icon;
        img.raycastTarget = false;
        dragGhost.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 60);
    }
 
    public void OnDrag(PointerEventData e)
    {
        if (dragGhost != null)
            dragGhost.transform.position = e.position;
    }
 
    public void OnEndDrag(PointerEventData e)
    {
        Destroy(dragGhost);
        StartCoroutine(ResetDragSource());
    }
 
    private System.Collections.IEnumerator ResetDragSource()
    {
        yield return null;
        dragSource = null;
    }
 
    // ── Nhận drop ──
    public void OnDrop(PointerEventData e)
    {
        // Nhận từ inventory slot khác → swap
        if (dragSource != null && dragSource != this)
        {
            InventoryManager.Instance.SwapSlots(dragSource.slotIndex, slotIndex);
            return;
        }
 
        // Nhận từ equipment slot kéo ra → unequip vào đây
        if (EquipDropZone.DragSource != null)
        {
            var equipSlot = EquipDropZone.DragSource.slot;
            EquipmentManager.Instance.Unequip(equipSlot);
            // Unequip tự gọi AddItem → item vào slot trống đầu tiên
            // Không cần làm gì thêm
        }
    }
 
    // ── Hover tooltip ──
    public void OnPointerEnter(PointerEventData e)
    {
        if (highlight != null) highlight.enabled = true;
        if (!data.IsEmpty && ItemTooltip.Instance != null)
            ItemTooltip.Instance.Show(data.item, transform.position);
    }
 
    public void OnPointerExit(PointerEventData e)
    {
        if (highlight != null) highlight.enabled = false;
        if (ItemTooltip.Instance != null)
            ItemTooltip.Instance.Hide();
    }
 
    // ── Click phải context menu ──
    public void OnPointerClick(PointerEventData e)
    {
        if (e.button == PointerEventData.InputButton.Right && !data.IsEmpty)
            ItemContextMenu.Instance.Show(slotIndex, data, transform.position);
    }
}
