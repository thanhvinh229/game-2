using UnityEngine;

public class InventorySlot 
{
    public ItemData item;
    public int quantity;
    public bool IsEmpty => item == null;
    public bool CanAddMore => item != null && item.isStackable && quantity < item.maxStackSize;
}
