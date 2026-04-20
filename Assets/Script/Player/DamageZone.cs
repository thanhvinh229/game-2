using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DamageZone : MonoBehaviour
{
    // ─── Được set từ PlayerSkills khi Spawn ────────────────
    [HideInInspector] public float radius       = 6f;   // Bán kính vùng
    [HideInInspector] public float duration     = 4f;   // Tổng thời gian tồn tại (s)
    [HideInInspector] public float tickInterval = 0.5f; // Gây damage mỗi X giây
    [HideInInspector] public float damagePerTick = 5f;  // Sát thương mỗi tick
    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public AudioClip  tickSound;
 
    // ─── Internal ───────────────────────────────────────────
    private float   _elapsed   = 0f;
    private float   _nextTick  = 0f;
    private int     _enemyMask;
 
    // Tránh hit cùng 1 kẻ địch 2 lần trong cùng 1 tick
    private readonly List<IDamageable> _hitThisTick = new();
 
    void Start()
    {
        _enemyMask = LayerMask.GetMask("Enemy");
        Destroy(gameObject, duration);
    }
 
    void Update()
    {
        _elapsed += Time.deltaTime;
 
        if (Time.time >= _nextTick)
        {
            _nextTick = Time.time + tickInterval;
            ApplyTickDamage();
        }
    }
 
    void ApplyTickDamage()
    {
        _hitThisTick.Clear();
 
        Collider[] cols = Physics.OverlapSphere(transform.position, radius, _enemyMask);
        foreach (Collider col in cols)
        {
            if (col.CompareTag("Player")) continue;
 
            IDamageable dmg = col.GetComponent<IDamageable>();
            if (dmg == null || _hitThisTick.Contains(dmg)) continue;
 
            dmg.TakeDamage(damagePerTick);
            _hitThisTick.Add(dmg);
        }
 
        if (_hitThisTick.Count > 0 && audioSource != null && tickSound != null)
            audioSource.PlayOneShot(tickSound);
    }
 
    // ─── Gizmo debug trong Scene view ──────────────────────
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        // Màu chuyển từ vàng → đỏ theo thời gian
        float t = _elapsed / Mathf.Max(duration, 0.01f);
        Gizmos.color = new Color(1f, 1f - t, 0f, 0.25f);
        Gizmos.DrawSphere(transform.position, radius);
 
        Gizmos.color = new Color(1f, 1f - t, 0f, 0.8f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}