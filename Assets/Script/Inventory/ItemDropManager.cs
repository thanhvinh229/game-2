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

    Vector3 dropPos;

    // Nếu spawnPos không được truyền vào (bằng zero), thì mới tìm player
    if (spawnPos == Vector3.zero)
    {
        var player = GameObject.FindWithTag("Player");
        dropPos = player != null 
            ? player.transform.position + player.transform.forward * dropDistance 
            : Vector3.zero;
    }
    else
    {
        // Nếu có vị trí quái chết, cho rơi tại đó
        dropPos = spawnPos;
    }

    dropPos += Vector3.up * dropHeight; // Cộng thêm độ cao để item không lún dưới đất

    var go = Instantiate(groundItemPrefab, dropPos, Quaternion.identity);
    var gi = go.GetComponent<GroundItem>();
    if (gi != null)
    {
        gi.item = item;
        gi.quantity = quantity;
    }
}
}