using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;

public class EquipmentUI : MonoBehaviour 
{
    // Chỉ cần slot enum để Refresh đúng — icon do EquipDropZone xử lý
    [System.Serializable]
    public class EquipSlotUI
    {
        public EquipSlot slot;
        // KHÔNG CÓ icon field nữa — tránh vô tình disable Image
    }
 
    [SerializeField] private List<EquipSlotUI> slotUIs;
 
    [Header("Stat Text (có thể để trống)")]
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI defenseText;
    [SerializeField] private TextMeshProUGUI hpText;
 
    void Start()
    {
        if (EquipmentManager.Instance == null)
        {
            Debug.LogError("[EquipmentUI] EquipmentManager chưa có trong scene!");
            return;
        }
 
        EquipmentManager.Instance.OnEquipped   += (_, __) => RefreshStats();
        EquipmentManager.Instance.OnUnequipped += (_)     => RefreshStats();
 
        RefreshStats();
    }
 
    void RefreshStats()
    {
        if (attackText  != null) attackText .text = $"ATK   {PlayerStats.Instance.Attack:F0}";
        if (defenseText != null) defenseText.text = $"DEF   {PlayerStats.Instance.Defense:F0}";
        if (hpText      != null) hpText     .text = $"HP    {PlayerStats.Instance.MaxHP:F0}";
    }
}