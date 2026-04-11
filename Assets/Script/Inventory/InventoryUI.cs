using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotContainer;
    [SerializeField] private GameObject inventoryPanel;

    private List<SlotUI> slotUIs = new();

    void Start() {
        InventoryManager.Instance.OnInventoryChanged += Refresh;
        GenerateSlots();
        
    }

    void Update() {
        // Toggle inventory bằng phím I hoặc Tab
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab))
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }

    void GenerateSlots() {
        var slots = InventoryManager.Instance.GetSlots();
        for (int i = 0; i < slots.Count; i++) {
            var go = Instantiate(slotPrefab, slotContainer);
            var slotUI = go.GetComponent<SlotUI>();
            slotUI.Init(i);
            slotUIs.Add(slotUI);
        }
        Refresh();
    }

    void Refresh() {
        var slots = InventoryManager.Instance.GetSlots();
        for (int i = 0; i < slotUIs.Count; i++)
            slotUIs[i].UpdateDisplay(slots[i]);
    }
}
