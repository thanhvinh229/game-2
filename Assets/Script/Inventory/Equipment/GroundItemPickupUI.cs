using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GroundItemPickupUI : MonoBehaviour
{
    public static GroundItemPickupUI Instance { get; private set; }
 
    [Header("References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform  itemListContainer;
    [SerializeField] private GameObject rowPrefab;
 
    [Header("Settings")]
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
        if (Input.GetKeyDown(pickupKey)) PickupFirst();
    }
 
    public void ShowNearbyItem(GroundItem item)
    {
        if (!_nearbyItems.Contains(item)) _nearbyItems.Add(item);
        _nearbyItems.Sort((a, b) => b.item.rarity.CompareTo(a.item.rarity));
        Refresh();
    }
 
    public void HideItem(GroundItem item)
    {
        if (_nearbyItems.Contains(item)) _nearbyItems.Remove(item);
        Refresh();
    }
 
    private void PickupFirst()
    {
        if (_nearbyItems.Count > 0) _nearbyItems[0].Pickup();
    }
 
    private void Refresh()
    {
        foreach (var r in _rows) if (r != null) Destroy(r);
        _rows.Clear();
 
        if (_nearbyItems.Count == 0) { panel.SetActive(false); return; }
        panel.SetActive(true);
 
        for (int i = 0; i < _nearbyItems.Count; i++)
        {
            var gi   = _nearbyItems[i];
            var item = gi.item;
            var row  = Instantiate(rowPrefab, itemListContainer);
            _rows.Add(row);
 
            // 1. Thanh màu (RarityBar)
            var bar = row.transform.Find("RarityBar")?.GetComponent<Image>();
            if (bar != null) bar.color = item.RarityColor;
 
            // 2. InfoContainer
            Transform info = row.transform.Find("InfoContainer");
            if (info == null) continue;
 
            // ✅ FIX: Kiểm tra icon trước, dùng SetActive thay vì enabled
            var iconImg = info.Find("ItemIcon")?.GetComponent<Image>();
            if (iconImg != null)
            {
                if (item.icon != null)
                {
                    iconImg.sprite = item.icon;
                    iconImg.color  = Color.white; // đảm bảo alpha = 1, không bị trong suốt
                    iconImg.gameObject.SetActive(true);
                }
                else
                {
                    iconImg.gameObject.SetActive(false); // ẩn hẳn, tránh hiện ô trắng
                    Debug.LogWarning($"[PickupUI] Item '{item.itemName}' không có icon!");
                }
            }
 
            // Tên item
            var nameTxt = info.Find("ItemNameText")?.GetComponent<TextMeshProUGUI>();
            if (nameTxt != null)
            {
                nameTxt.text  = item.itemName;
                nameTxt.color = item.RarityColor;
            }
 
            // Text phụ (rarity · type · số lượng)
            var subTxt = info.Find("ItemSubText")?.GetComponent<TextMeshProUGUI>();
            if (subTxt != null)
            {
                string qty  = gi.quantity > 1 ? $" · x{gi.quantity}" : "";
                subTxt.text = $"{item.rarity} · {item.type}{qty}";
            }
 
            // Gợi ý phím F chỉ hiện ở item đầu tiên
            var fHint = info.Find("FHint");
            if (fHint != null) fHint.gameObject.SetActive(i == 0);
        }
    }
}