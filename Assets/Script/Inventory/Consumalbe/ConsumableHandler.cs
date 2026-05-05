using UnityEngine;
using System.Collections;
using TMPro;

public class ConsumableHandler : MonoBehaviour
{
    public static ConsumableHandler Instance { get; private set; }
 
    [Header("Popup thông báo (có thể để trống)")]
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private float           feedbackDuration = 1.5f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip   potionSound;
 
    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }
 
    void Start()
    {
        // Tự ẩn lúc bắt đầu 
        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);
    }
 
    public bool UseConsumable(ItemData item, int slotIndex)
    {
        if (item == null || item.type != ItemType.Consumable)
        {
            Debug.LogWarning("[Consumable] Item không phải consumable!");
            return false;
        }
 
        var stats = PlayerStats.Instance;
        if (stats == null) return false;
 
        bool used = false;
        string feedbackMessage = "";
        Color  feedbackColor   = Color.white;
 
        foreach (var mod in item.stats)
        {
            switch (mod.statName)
            {
                case "HP":
                    if (stats.currentHealth >= stats.maxHealth)
                    {
                        ShowFeedback("Máu đã đầy!", Color.red);
                        return false;
                    }
                    stats.Heal(mod.value);
                    feedbackMessage = $"+{mod.value:F0} HP";
                    feedbackColor   = new Color(1f, 0.2f, 0.2f); // đỏ
                    used = true;
                    break;
 
                case "Mana":
                    if (stats.currentMana >= stats.maxMana)
                    {
                        ShowFeedback("Mana đã đầy!", new Color(0.3f, 0.5f, 1f));
                        return false;
                    }
                    stats.RegenerateMana(mod.value);
                    feedbackMessage = $"+{mod.value:F0} Mana";
                    feedbackColor   = new Color(0.3f, 0.5f, 1f); // xanh biển
                    used = true;
                    break;
 
                default:
                    Debug.LogWarning($"[Consumable] Stat không xử lý: {mod.statName}");
                    break;
            }
        }
 
        if (used)
        {
            InventoryManager.Instance.RemoveItem(slotIndex, 1);
            ShowFeedback(feedbackMessage, feedbackColor);
            Debug.Log($"[Consumable] {item.itemName} → {feedbackMessage}");
        }
        if (audioSource != null && potionSound != null)
            {
                audioSource.PlayOneShot(potionSound);
            }
 
        return used;
    }
 
    void ShowFeedback(string message, Color color)
    {
        if (feedbackText == null) return;
        StopAllCoroutines();
        StartCoroutine(ShowFeedbackCoroutine(message, color));
    }
 
    IEnumerator ShowFeedbackCoroutine(string message, Color color)
    {
        feedbackText.text  = message;
        feedbackText.color = color;
        feedbackText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(feedbackDuration);
        feedbackText.gameObject.SetActive(false);
    }
}