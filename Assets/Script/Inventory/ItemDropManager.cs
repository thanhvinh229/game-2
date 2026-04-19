using UnityEngine;
using System.Collections.Generic;

public class ItemDropManager : MonoBehaviour
{
    public static ItemDropManager Instance { get; private set; }
 
    [Header("Prefab GroundItem — kéo prefab vào đây")]
    [SerializeField] private GameObject groundItemPrefab;
 
    [Header("Khoảng cách và độ cao khi vứt")]
    [SerializeField] private float dropDistance = 1.5f;
    [SerializeField] private float dropHeight   = 0.3f;
 
    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }
 
    public void DropItem(ItemData item, int quantity = 1, Vector3 spawnPos = default)
    {
        if (item == null || groundItemPrefab == null) return;

        Vector3 baseDropPos;

        // 1. Xác định tâm rơi (vị trí quái hoặc trước mặt player)
        if (spawnPos == Vector3.zero)
        {
            var player = GameObject.FindWithTag("Player");
            baseDropPos = player != null 
                ? player.transform.position + player.transform.forward * dropDistance 
                : Vector3.zero;
        }
        else
        {
            baseDropPos = spawnPos;
        }

        // --- 2. TẠO VỊ TRÍ RƠI RANDOM XUNG QUANH TÂM ---
        // Tạo một điểm ngẫu nhiên trong vòng tròn bán kính 1 mét
        Vector2 randomCircle = Random.insideUnitCircle * 1.0f; 
        
        Vector3 finalDropPos = new Vector3(
            baseDropPos.x + randomCircle.x,
            baseDropPos.y + dropHeight, // Cộng thêm độ cao để không lún đất
            baseDropPos.z + randomCircle.y
        );

        // 3. Tạo item tại vị trí ĐÃ LỆCH
        var go = Instantiate(groundItemPrefab, finalDropPos, Quaternion.identity);
        var gi = go.GetComponent<GroundItem>();
        if (gi != null)
        {
            gi.item = item;
            gi.quantity = quantity;
        }

        // 4. Vẫn cho nảy nhẹ lên trên một chút cho sinh động
        Rigidbody rb = go.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Tạo hướng nảy lên trên và hơi văng ra xa tâm một chút
            Vector3 popDirection = new Vector3(randomCircle.x, 2f, randomCircle.y).normalized;
            
            // Tự động tính lực dựa trên Mass (Nếu Mass=50 thì lực sẽ tự nhân lên cho đủ mạnh)
            float popForce = rb.mass * 4f; 
            
            rb.AddForce(popDirection * popForce, ForceMode.Impulse);
        }
    }
}