// EquipDropZone.cs
// Gắn lên TỪNG ô trang bị: SlotHead, SlotChest, SlotWeapon, SlotLegs, SlotShield, SlotRing
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class EquipDropZone : MonoBehaviour,
    IDropHandler, IPointerClickHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] public EquipSlot slot;
 
    [Tooltip("Loại item được chấp nhận. Để None = dùng chính slot bên trên.")]
    [SerializeField] private EquipSlot acceptedItemSlot = EquipSlot.None;
 
    [SerializeField] private GameObject emptyIconObj;
 
    private Image  _image;
    private Sprite _originalSprite;
    private Color  _originalColor;
 
    private static GameObject    _dragGhost;
    public  static EquipDropZone DragSource { get; private set; }
 
    EquipSlot AcceptedSlot => acceptedItemSlot == EquipSlot.None ? slot : acceptedItemSlot;
 
    void Start()
    {
        _image = GetComponent<Image>();
        _image.enabled       = true;
        _image.raycastTarget = true;
        _originalSprite      = _image.sprite;
        _originalColor       = _image.color;
 
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
 
    // ── Kéo item ra khỏi ô ──────────────────────────────────────────────
    public void OnBeginDrag(PointerEventData e)
    {
        var item = EquipmentManager.Instance.GetEquipped(slot);
        if (item == null) return;
 
        DragSource = this;
        _dragGhost = new GameObject("EquipDragGhost");
        _dragGhost.transform.SetParent(transform.root, false);
        var img = _dragGhost.AddComponent<Image>();
        img.sprite        = item.icon != null ? item.icon : _originalSprite;
        img.raycastTarget = false;
        _dragGhost.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 60);
    }
 
    public void OnDrag(PointerEventData e)
    {
        if (_dragGhost != null) _dragGhost.transform.position = e.position;
    }
 
    public void OnEndDrag(PointerEventData e)
    {
        if (_dragGhost != null) { Destroy(_dragGhost); _dragGhost = null; }
        DragSource = null;
    }
 
    // ── Nhận drop ───────────────────────────────────────────────────────
    public void OnDrop(PointerEventData e)
    {
        // Trường hợp 1: Kéo từ INVENTORY vào ô equipment
        var invSrc = SlotUI.DragSource;
        if (invSrc != null)
        {
            var slots = InventoryManager.Instance.GetSlots();
            if (invSrc.SlotIndex >= slots.Count) return;
 
            var item = slots[invSrc.SlotIndex].item;
            if (item == null) return;
 
            if (item.equipSlot != AcceptedSlot)
            {
                Debug.Log($"{item.itemName} cần [{item.equipSlot}], ô này nhận [{AcceptedSlot}]");
                return;
            }
 
            // Equip vào đúng slot đích (Weapon2, Ring2...)
            EquipmentManager.Instance.Equip(item, slot);
            InventoryManager.Instance.RemoveItem(invSrc.SlotIndex);
            return;
        }
 
        // Trường hợp 2: Kéo từ ô EQUIPMENT KHÁC sang ô này (swap)
        if (DragSource != null && DragSource != this)
        {
            var srcSlot  = DragSource.slot;
            var dragItem = EquipmentManager.Instance.GetEquipped(srcSlot);
 
            if (dragItem == null || dragItem.equipSlot != AcceptedSlot) return;
 
            // Lấy item đang ở ô đích (nếu có)
            var dstItem = EquipmentManager.Instance.GetEquipped(slot);
 
            // Dùng UnequipSilent để KHÔNG trả về inventory
            EquipmentManager.Instance.UnequipSilent(srcSlot);
            if (dstItem != null)
                EquipmentManager.Instance.UnequipSilent(slot);
 
            // Equip lại đúng vị trí — không qua inventory nên không bị nhân bản
            EquipmentManager.Instance.Equip(dragItem, slot);
            if (dstItem != null)
                EquipmentManager.Instance.Equip(dstItem, srcSlot);
        }
    }
 
    // ── Click phải tháo đồ (trả về inventory) ───────────────────────────
    public void OnPointerClick(PointerEventData e)
    {
        if (e.button != PointerEventData.InputButton.Right) return;
        var item = EquipmentManager.Instance.GetEquipped(slot);
        if (item == null) return;
        EquipmentManager.Instance.Unequip(slot);
        Debug.Log($"Tháo: {item.itemName} từ {slot}");
    }
 
    void Refresh()
    {
        var item = EquipmentManager.Instance.GetEquipped(slot);
        bool has = item != null;
 
        if (has)
        {
            _image.sprite = item.icon;
            _image.color  = Color.white;
            if (emptyIconObj != null) emptyIconObj.SetActive(false);
        }
        else
        {
            _image.sprite = _originalSprite;
            _image.color  = _originalColor;
            if (emptyIconObj != null) emptyIconObj.SetActive(true);
        }
    }
 
    void OnEquipped(EquipSlot s, ItemData _)  { if (s == slot) Refresh(); }
    void OnUnequipped(EquipSlot s)             { if (s == slot) Refresh(); }
}
 