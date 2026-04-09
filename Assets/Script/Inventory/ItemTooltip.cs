using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemTooltip : MonoBehaviour
{
    public static ItemTooltip Instance { get; private set; }

     [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private RectTransform tooltipRect;
    [SerializeField] private Canvas rootCanvas;

    // Offset để tooltip không che con trỏ
    private Vector2 offset = new Vector2(16f, -16f);

    void Awake() {
        Instance = this;
        panel.SetActive(false);
    }

    void Update() {
        if (!panel.activeSelf) return;
        FollowMouse();
    }

    public void Show(ItemData item, Vector3 anchorPos) {
        nameText.text  = item.itemName;
        typeText.text  = item.type.ToString();
        descText.text  = item.description;
        valueText.text = $"Giá: {item.value} gold";

        // Build stats text
        if (item.stats.Count > 0) {
            var sb = new System.Text.StringBuilder();
            foreach (var s in item.stats)
                sb.AppendLine($"{s.statName}: +{s.value}");
            statsText.text = sb.ToString().TrimEnd();
            statsText.gameObject.SetActive(true);
        } else {
            statsText.gameObject.SetActive(false);
        }

        panel.SetActive(true);
        FollowMouse();
    }

    public void Hide() => panel.SetActive(false);

    void FollowMouse() {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.GetComponent<RectTransform>(),
            Input.mousePosition,
            rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera,
            out pos
        );
        tooltipRect.anchoredPosition = ClampToScreen(pos + offset);
    }

    Vector2 ClampToScreen(Vector2 pos) {
        var canvasRect = rootCanvas.GetComponent<RectTransform>().rect;
        var tip = tooltipRect.rect;

        pos.x = Mathf.Clamp(pos.x, canvasRect.xMin, canvasRect.xMax - tip.width);
        pos.y = Mathf.Clamp(pos.y, canvasRect.yMin + tip.height, canvasRect.yMax);
        return pos;
    }
}
