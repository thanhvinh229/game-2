using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

public class QuickUseSlot : MonoBehaviour,  IPointerEnterHandler, IPointerExitHandler, IScrollHandler

{
    public static QuickUseSlot Instance { get; private set; }
 
    [Header("UI References")]
    [SerializeField] private Image           itemIcon;        // icon consumable
    [SerializeField] private TextMeshProUGUI quantityText;    // số lượng
    [SerializeField] private Image           cooldownOverlay; // overlay tối khi cooldown
    [SerializeField] private Image           hoverHighlight;  // viền sáng khi hover
 
    [Header("Settings")]
    [SerializeField] private KeyCode useKey   = KeyCode.Q;
    [SerializeField] private float   cooldown = 0.5f;
 
    [Header("Khởi đầu với item này")]
    [SerializeField] private ItemData startingItem;
    [SerializeField] private int      startingAmount = 3;
 
    private int   _selectedIndex = 0;
    private float _cooldownTimer = 0f;
    private bool  _isHovered     = false;
    private bool  _onCooldown   => _cooldownTimer > 0f;
 
    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }
 
    void Start()
    {
        if (startingItem != null)
            InventoryManager.Instance.AddItem(startingItem, startingAmount);
 
        InventoryManager.Instance.OnInventoryChanged += Refresh;
 
        if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0f;
        if (hoverHighlight  != null) hoverHighlight.enabled     = false;
 
        Refresh();
    }
 
    void Update()
    {
        // Cooldown overlay
        if (_cooldownTimer > 0f)
        {
            _cooldownTimer -= Time.unscaledDeltaTime;
            if (cooldownOverlay != null)
                cooldownOverlay.fillAmount = Mathf.Clamp01(_cooldownTimer / cooldown);
            if (_cooldownTimer <= 0f)
            {
                _cooldownTimer = 0f;
                if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0f;
            }
        }
 
        // Phím Q để dùng (không dùng khi UI inventory đang mở)
        if (Input.GetKeyDown(useKey) && !_onCooldown && !GameStateManager.IsUIOpen)
            UseSelected();
    }
 
    // ── Scroll chỉ hoạt động khi hover lên ô ──────────────────────────────
    public void OnPointerEnter(PointerEventData e)
    {
        _isHovered = true;
        if (hoverHighlight != null) hoverHighlight.enabled = true;
    }
 
    public void OnPointerExit(PointerEventData e)
    {
        _isHovered = false;
        if (hoverHighlight != null) hoverHighlight.enabled = false;
    }
 
    public void OnScroll(PointerEventData e)
    {
        var list = GetConsumableSlots();
        if (list.Count <= 1) return;
 
        // Scroll lên → item trước, scroll xuống → item sau
        int dir = e.scrollDelta.y > 0 ? -1 : 1;
        _selectedIndex = (_selectedIndex + dir % list.Count + list.Count) % list.Count;
        Refresh();
    }
    // ──────────────────────────────────────────────────────────────────────
 
    void UseSelected()
    {
        var list = GetConsumableSlots();
        if (list.Count == 0) return;
 
        _selectedIndex = Mathf.Clamp(_selectedIndex, 0, list.Count - 1);
        var (slotIndex, slot) = list[_selectedIndex];
 
        bool ok = ConsumableHandler.Instance.UseConsumable(slot.item, slotIndex);
        if (ok) _cooldownTimer = cooldown;
    }
 
    List<(int index, InventorySlot slot)> GetConsumableSlots()
    {
        var result = new List<(int, InventorySlot)>();
        var slots  = InventoryManager.Instance.GetSlots();
        for (int i = 0; i < slots.Count; i++)
            if (!slots[i].IsEmpty && slots[i].item.type == ItemType.Consumable)
                result.Add((i, slots[i]));
        return result;
    }
 
    public void Refresh()
    {
        var list = GetConsumableSlots();
 
        if (list.Count == 0)
        {
            if (itemIcon     != null) itemIcon.enabled  = false;
            if (quantityText != null) quantityText.text = "";
            return;
        }
 
        _selectedIndex = Mathf.Clamp(_selectedIndex, 0, list.Count - 1);
        var (_, slot)  = list[_selectedIndex];
 
        if (itemIcon != null)
        {
            itemIcon.enabled = slot.item.icon != null;
            if (slot.item.icon != null) itemIcon.sprite = slot.item.icon;
        }
 
        if (quantityText != null)
            quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
    }
 
    void OnDestroy()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged -= Refresh;
    }
}
 
