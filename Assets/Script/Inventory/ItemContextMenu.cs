using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class  ItemContextMenu  : MonoBehaviour
{
    public static ItemContextMenu Instance { get; private set; }
 
    [SerializeField] private GameObject  panel;
    [SerializeField] private Transform   buttonContainer;
    [SerializeField] private GameObject  buttonPrefab;
    [SerializeField] private RectTransform menuRect;
    [SerializeField] private Canvas      rootCanvas;
 
    private List<GameObject> activeButtons = new();
 
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        panel.SetActive(false);
    }
 
    void Update()
    {
        // Click bất kỳ đâu bên ngoài thì đóng menu
        if (panel.activeSelf && Input.GetMouseButtonDown(0))
            Hide();
    }
 
    public void Show(int slotIndex, InventorySlot slot, Vector3 screenPos)
    {
        foreach (var b in activeButtons) Destroy(b);
        activeButtons.Clear();
 
        // Trang bị / tháo ra
        if (slot.item.equipSlot != EquipSlot.None)
        {
            bool isEquipped = EquipmentManager.Instance.GetEquipped(slot.item.equipSlot) == slot.item;
            if (isEquipped)
            {
                AddButton("Tháo ra", () =>
                {
                    EquipmentManager.Instance.Unequip(slot.item.equipSlot);
                    Hide();
                });
            }
            else
            {
                AddButton("Trang bị", () =>
                {
                    EquipmentManager.Instance.Equip(slot.item);
                    InventoryManager.Instance.RemoveItem(slotIndex);
                    Hide();
                });
            }
        }
 
        // Sử dụng (chỉ Consumable)
        if (slot.item.type == ItemType.Consumable)
        {
            AddButton("Sử dụng", () =>
            {
                PlayerStats.Instance.ApplyModifiers(slot.item.stats, add: true);
                InventoryManager.Instance.RemoveItem(slotIndex, 1);
                Hide();
            });
        }
 
        // Vứt bỏ (không cho vứt Quest item)
        if (slot.item.type != ItemType.Quest)
        {
            AddButton("Vứt bỏ", () =>
            {
                InventoryManager.Instance.RemoveItem(slotIndex);
                Hide();
            });
        }
 
        panel.SetActive(true);
        PositionMenu(screenPos);
    }
 
    public void Hide() => panel.SetActive(false);
 
    void AddButton(string label, System.Action onClick)
    {
        var go  = Instantiate(buttonPrefab, buttonContainer);
        go.GetComponentInChildren<TextMeshProUGUI>().text = label;
        go.GetComponent<Button>().onClick.AddListener(() => onClick());
        activeButtons.Add(go);
    }
 
    void PositionMenu(Vector3 screenPos)
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.GetComponent<RectTransform>(),
            screenPos,
            rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera,
            out localPos
        );
 
        // Flip sang trái / lên trên nếu tràn màn hình
        var cv   = rootCanvas.GetComponent<RectTransform>().rect;
        var size = menuRect.sizeDelta;
        if (localPos.x + size.x > cv.xMax) localPos.x -= size.x;
        if (localPos.y - size.y < cv.yMin) localPos.y += size.y;
 
        menuRect.anchoredPosition = localPos;
    }
}
