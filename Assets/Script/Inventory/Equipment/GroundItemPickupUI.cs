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
 
    // In toàn bộ cây con của Transform để xem đúng tên node
    private void PrintHierarchy(Transform t, string indent = "")
    {
        Debug.Log($"{indent}[{t.name}] active={t.gameObject.activeSelf}");
        foreach (Transform child in t)
            PrintHierarchy(child, indent + "  ");
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
 
            // ── DEBUG: In hierarchy của row để xem đúng tên thật các node ──
            if (i == 0)
            {
                Debug.Log("===== [PickupUI] ROW HIERARCHY =====");
                PrintHierarchy(row.transform);
                Debug.Log("=====================================");
            }
 
            // 1. Thanh màu
            var bar = row.transform.Find("RarityBar")?.GetComponent<Image>();
            if (bar != null) bar.color = item.RarityColor;
 
            // 2. InfoContainer
            Transform info = row.transform.Find("InfoContainer");
            Debug.Log($"[PickupUI] InfoContainer found: {info != null}");
            if (info == null) continue;
 
            // 3. Icon – tìm trực tiếp trước, nếu không có thì tìm đệ quy
            Transform iconTf = info.Find("ItemIcon");
            if (iconTf == null)
            {
                iconTf = FindDeep(row.transform, "ItemIcon");
                Debug.Log($"[PickupUI] ItemIcon deep search: {(iconTf != null ? GetPath(iconTf) : "NOT FOUND")}");
            }
 
            var iconImg = iconTf?.GetComponent<Image>();
            Debug.Log($"[PickupUI] Image component: {iconImg != null} | item.icon: {(item.icon != null ? item.icon.name : "NULL")}");
 
            if (iconImg != null)
            {
                if (item.icon != null)
                {
                    iconImg.sprite  = item.icon;
                    iconImg.color   = Color.white;   // alpha = 1
                    iconImg.enabled = true;
                    iconImg.gameObject.SetActive(true);
 
                    // Kiểm tra kích thước RectTransform
                    var rect = iconImg.GetComponent<RectTransform>();
                    Debug.Log($"[PickupUI] Icon sizeDelta: {rect.sizeDelta}");
                    if (rect.sizeDelta.x <= 0 || rect.sizeDelta.y <= 0)
                    {
                        rect.sizeDelta = new Vector2(40f, 40f);
                        Debug.LogWarning("[PickupUI] Icon size = 0 → đã tự set 40x40!");
                    }
                }
                else
                {
                    iconImg.gameObject.SetActive(false);
                    Debug.LogWarning($"[PickupUI] '{item.itemName}' chưa gán sprite trong ScriptableObject!");
                }
            }
 
            // Tên item
            var nameTxt = info.Find("ItemNameText")?.GetComponent<TextMeshProUGUI>();
            if (nameTxt != null)
            {
                nameTxt.text  = item.itemName;
                nameTxt.color = item.RarityColor;
            }
 
            // Text phụ
            var subTxt = info.Find("ItemSubText")?.GetComponent<TextMeshProUGUI>();
            if (subTxt != null)
            {
                string qty  = gi.quantity > 1 ? $" · x{gi.quantity}" : "";
                subTxt.text = $"{item.rarity} · {item.type}{qty}";
            }
 
            // Phím F chỉ hiện ở item đầu tiên
            var fHint = info.Find("FHint");
            if (fHint != null) fHint.gameObject.SetActive(i == 0);
        }
    }
 
    // Tìm đệ quy theo tên
    private Transform FindDeep(Transform parent, string targetName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == targetName) return child;
            var result = FindDeep(child, targetName);
            if (result != null) return result;
        }
        return null;
    }
 
    // Lấy full path của Transform
    private string GetPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null) { t = t.parent; path = t.name + "/" + path; }
        return path;
    }
}