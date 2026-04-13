using UnityEngine;

public class GroundItem : MonoBehaviour
{
    [HideInInspector] public ItemData item;
    [HideInInspector] public int      quantity = 1;
 
    [SerializeField] private float    autoDestroyTime = 120f;
    [SerializeField] private float    pickupRadius    = 2.5f;
    [SerializeField] private ParticleSystem glowVFX;      // Particle System prefab/child
 
    private bool _playerNearby = false;
 
    void Start()
    {
        
        // Hiện icon item qua SpriteRenderer nếu có
        var sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null && item?.icon != null)
            sr.sprite = item.icon;
 
        // Bật glow VFX
        if (glowVFX != null) glowVFX.Play(true);
 
        Invoke(nameof(SelfDestroy), autoDestroyTime);
    }
 
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerNearby = true;
        GroundItemPickupUI.Instance?.ShowNearbyItem(this);
    }
 
    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerNearby = false;
        GroundItemPickupUI.Instance?.HideItem(this);
    }
 
    public void Pickup()
    {
        bool ok = InventoryManager.Instance.AddItem(item, quantity);
        if (ok)
        {
            GroundItemPickupUI.Instance?.HideItem(this);
            Destroy(gameObject);
        }
        else
            Debug.Log("Inventory đầy!");
    }
 
    void SelfDestroy()
    {
        GroundItemPickupUI.Instance?.HideItem(this);
        Destroy(gameObject);
    }
}
