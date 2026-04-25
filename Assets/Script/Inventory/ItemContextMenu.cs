using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class  ItemContextMenu  : MonoBehaviour
{
    public static ItemContextMenu Instance { get; private set; }
 
    [SerializeField] private GameObject    panel;
    [SerializeField] private Transform     buttonContainer;
    [SerializeField] private GameObject    buttonPrefab;
    [SerializeField] private RectTransform menuRect;
    [SerializeField] private Canvas        rootCanvas;
 
    private List<GameObject> activeButtons = new();
 
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        panel.SetActive(false);
    }
 
    void Update()
    {
        if (panel.activeSelf && Input.GetMouseButtonDown(0))
            Hide();
    }
 
    public void Show(int slotIndex, InventorySlot slot, Vector3 screenPos)
    {
        foreach (var b in activeButtons) Destroy(b);
        activeButtons.Clear();
 
        var item = slot.item;
 
        // ── Trang bị / Tháo ra ──
        if (item.equipSlot != EquipSlot.None)
        {
            bool isEquipped = EquipmentManager.Instance?.GetEquipped(item.equipSlot) == item;
            if (isEquipped)
                AddButton("Tháo ra", () => {
                    EquipmentManager.Instance.Unequip(item.equipSlot);
                    Hide();
                });
            else
                AddButton("Trang bị", () => {
                    EquipmentManager.Instance.Equip(item);
                    InventoryManager.Instance.RemoveItem(slotIndex);
                    Hide();
                });
        }
 
        // ── Sử dụng (Consumable) ──
        if (item.type == ItemType.Consumable)
        {
            AddButton("Sử dụng", () => {
                // ConsumableHandler xử lý logic và tự RemoveItem nếu thành công
                ConsumableHandler.Instance?.UseConsumable(item, slotIndex);
                Hide();
            });
        }
 
        // ── Vứt bỏ (không vứt Quest item) ──
        if (item.type != ItemType.Quest)
            AddButton("Vứt bỏ", () => {
                ItemDropManager.Instance?.DropItem(item, slot.quantity);
                InventoryManager.Instance.RemoveItem(slotIndex, slot.quantity);
                Hide();
            });
 
        panel.SetActive(true);
        PositionMenu(screenPos);
    }
 
    public void Hide() => panel.SetActive(false);
 
    void AddButton(string label, System.Action onClick)
    {
        var go = Instantiate(buttonPrefab, buttonContainer);
        go.GetComponentInChildren<TextMeshProUGUI>().text = label;
        go.GetComponent<Button>().onClick.AddListener(() => onClick());
        activeButtons.Add(go);
    }
 
    void PositionMenu(Vector3 screenPos)
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.GetComponent<RectTransform>(), screenPos,
            rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera,
            out localPos);
 
        var cv   = rootCanvas.GetComponent<RectTransform>().rect;
        var size = menuRect.sizeDelta;
        if (localPos.x + size.x > cv.xMax) localPos.x -= size.x;
        if (localPos.y - size.y < cv.yMin) localPos.y += size.y;
 
        menuRect.anchoredPosition = localPos;
    }
}
