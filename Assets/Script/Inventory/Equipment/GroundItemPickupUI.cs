using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GroundItemPickupUI : MonoBehaviour
{
    public static GroundItemPickupUI Instance { get; private set; }
 
    [Header("References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform  itemListContainer; // ItemListContainer
    [SerializeField] private GameObject rowPrefab;         // RowPrefab
 
    [Header("Phím nhặt từng cái")]
    [SerializeField] private KeyCode pickupKey = KeyCode.F;
 
    private List<GroundItem> _nearbyItems = new();
    private List<GameObject> _rows        = new();
 
    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        panel.SetActive(false);
    }
 
    void Update()
    {
        if (_nearbyItems.Count == 0) return;
 
        // Mỗi lần nhấn F → nhặt item đầu tiên trong danh sách
        if (Input.GetKeyDown(pickupKey))
            PickupFirst();
    }
 
    // Gọi khi player vào gần GroundItem
    public void ShowNearbyItem(GroundItem item)
    {
        if (!_nearbyItems.Contains(item))
            _nearbyItems.Add(item);
        Refresh();
    }
 
    // Gọi khi player ra xa hoặc item đã được nhặt
    public void HideItem(GroundItem item)
    {
        _nearbyItems.Remove(item);
        Refresh();
    }
 
    // Nhặt item đầu tiên trong danh sách (nhấn F)
    void PickupFirst()
    {
        if (_nearbyItems.Count == 0) return;
        var first = _nearbyItems[0];
        if (first != null)
            first.Pickup(); // Pickup() tự gọi HideItem → Refresh
    }
 
    void Refresh()
    {
        // Xóa rows cũ
        foreach (var r in _rows) Destroy(r);
        _rows.Clear();
 
        // Ẩn panel nếu không còn item
        if (_nearbyItems.Count == 0)
        {
            panel.SetActive(false);
            return;
        }
 
        panel.SetActive(true);
 
        for (int i = 0; i < _nearbyItems.Count; i++)
        {
            var groundItem = _nearbyItems[i];
            if (groundItem == null) continue;
 
            var row = Instantiate(rowPrefab, itemListContainer);
            SetupRow(row, groundItem, i == 0); // item đầu tiên = sẽ nhặt khi nhấn F
            _rows.Add(row);
        }
    }
 
    void SetupRow(GameObject row, GroundItem groundItem, bool isNext)
    {
        var item = groundItem.item;
        if (item == null) return;
 
        // Icon
        var icon = row.transform.Find("IconContainer/ItemIcon")
                               ?.GetComponent<Image>();
        if (icon != null && item.icon != null)
            icon.sprite = item.icon;
 
        // Tên item
        var nameText = row.transform.Find("InfoContainer/ItemNameText")
                                   ?.GetComponent<TextMeshProUGUI>();
        if (nameText != null) nameText.text = item.itemName;
 
        // Sub text: loại + số lượng
        var subText = row.transform.Find("InfoContainer/ItemSubText")
                                  ?.GetComponent<TextMeshProUGUI>();
        if (subText != null)
            subText.text = groundItem.quantity > 1
                ? $"{item.type}  ·  x{groundItem.quantity}"
                : item.type.ToString();
 
        // Màu theo độ hiếm
        Color rarityColor = item.type switch {
            ItemType.Weapon   => new Color(0.65f, 0.55f, 0.98f), // tím
            ItemType.Armor    => new Color(0.20f, 0.83f, 0.60f), // xanh lá
            ItemType.Quest    => new Color(1.00f, 0.80f, 0.20f), // vàng
            _                 => new Color(0.61f, 0.64f, 0.67f)  // xám
        };
 
        var bar = row.transform.Find("RarityBar")?.GetComponent<Image>();
        if (bar != null) bar.color = rarityColor;
        if (nameText != null) nameText.color = rarityColor;
 
        // // Hint [F] chỉ hiện ở item đầu tiên
        // var fHint = row.transform.Find("FHint")?.GetComponent<TextMeshProUGUI>();
        // if (fHint != null) fHint.gameObject.SetActive(isNext);
 
        // Nút Nhặt → nhặt riêng item này
        var btn = row.GetComponentInChildren<Button>();
        var captured = groundItem;
        btn?.onClick.AddListener(() => captured.Pickup());
    }
}   
