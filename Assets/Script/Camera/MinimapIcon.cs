using UnityEngine;
using UnityEngine.UI;

public class MinimapIcon : MonoBehaviour
{
    public enum IconType { Player, Enemy, Boss, NPC }
 
    [Header("Icon Settings")]
    public IconType iconType = IconType.Enemy;
 
    [Tooltip("Sprite icon trên minimap (Nên dùng ảnh mũi tên/nón hướng lên trên).")]
    public Sprite iconSprite;
 
    [Range(6, 20)]
    public int iconSize = 10;

    [Tooltip("Bật tùy chọn này để icon xoay theo hướng nhìn của object.")]
    public bool showDirection = false;
 
    // ── Màu mặc định theo loại ───────────────────────────────────────────
    static readonly Color ColorPlayer = new Color(0.29f, 0.91f, 0.63f);
    static readonly Color ColorEnemy  = new Color(1f,    0.24f, 0.24f);
    static readonly Color ColorBoss   = new Color(1f,    0.55f, 0f);
    static readonly Color ColorNPC    = new Color(0.4f,  0.8f,  1f);
 
    // ── Internal ──────────────────────────────────────────────────────────
    private RectTransform  iconRect;
    private RectTransform  mapRect;
    private Camera         minimapCam;
    private GameObject     iconGO;
    private MinimapCamera  minimapCamScript;
 
    void Start()
    {
        MinimapUI mapUI = Object.FindFirstObjectByType<MinimapUI>();
        if (mapUI == null) return;
 
        minimapCamScript = Object.FindFirstObjectByType<MinimapCamera>();
        if (minimapCamScript == null) return;
 
        minimapCam = minimapCamScript.GetComponent<Camera>();
        mapRect    = mapUI.minimapDisplay.rectTransform;
 
        iconGO = new GameObject($"Icon_{gameObject.name}");
        iconGO.transform.SetParent(mapRect, false);
 
        Image img         = iconGO.AddComponent<Image>();
        if (iconSprite != null) img.sprite = iconSprite;
        img.color         = GetColor();
        img.raycastTarget = false;
 
        iconRect           = iconGO.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.5f, 0.5f);
        iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconRect.pivot     = new Vector2(0.5f, 0.5f);
        
        // Giữ nguyên logic kích thước cũ của bạn
        iconRect.sizeDelta = iconType == IconType.Player
                             ? new Vector2(14, 14)
                             : new Vector2(iconSize, iconSize);

        // Mặc định bật xoay hướng cho Player
        if (iconType == IconType.Player) showDirection = true;
    }
 
    void LateUpdate()
    {
        if (iconRect == null || minimapCam == null) return;
 
        Vector3 viewportPos = minimapCam.WorldToViewportPoint(transform.position);
 
        bool inView = viewportPos.x >= 0f && viewportPos.x <= 1f &&
                      viewportPos.y >= 0f && viewportPos.y <= 1f &&
                      viewportPos.z > 0f;
 
        iconGO.SetActive(inView);
        if (!inView) return;
 
        float mapW = mapRect.rect.width;
        float mapH = mapRect.rect.height;
        iconRect.anchoredPosition = new Vector2(
            (viewportPos.x - 0.5f) * mapW,
            (viewportPos.y - 0.5f) * mapH
        );
 
        // Xoay icon theo hướng nhìn nếu được bật
        if (showDirection)
        {
            float angle = transform.eulerAngles.y - minimapCamScript.transform.eulerAngles.y;
            iconRect.localRotation = Quaternion.Euler(0f, 0f, -angle);
        }
        else
        {
            // Cố định icon nếu không cần hiển thị hướng (dành cho NPC, item...)
            iconRect.localRotation = Quaternion.identity; 
        }
    }
 
    void OnDestroy()
    {
        if (iconGO != null) Destroy(iconGO);
    }
 
    Color GetColor()
    {
        return iconType switch
        {
            IconType.Player => ColorPlayer,
            IconType.Enemy  => ColorEnemy,
            IconType.Boss   => ColorBoss,
            IconType.NPC    => ColorNPC,
            _               => Color.white
        };
    }
}