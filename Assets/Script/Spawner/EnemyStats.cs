using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class EnemyStats : MonoBehaviour, IDamageable
{
    public float maxHealth = 50f;
    private float currentHealth;
    private bool IsDead = false;

    [Header("UI & Animation")]
    public Slider healthSlider;
    public GameObject healthBarUI;
    public Animator animator;

    [Header("Knockback Settings")]
    public float knockbackForce = 12f;
    public float knockbackDuration = 0.12f;

    [Header("Enemy Type & Events")]
    public string enemyType = "Enemy";
    public EnemyDeathEventChannel deathEventChannel;

    [Header("Damage UI")]
    public GameObject damagePopupPrefab;
    public Transform popupSpawnPoint;

    void Start()
    {
        currentHealth = maxHealth;
        if (animator == null) animator = GetComponent<Animator>();
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(float amount)
    {
     if (IsDead) return;

     currentHealth -= amount;
    
     if (damagePopupPrefab != null)
     {
        // Lấy vị trí sinh text (nếu không gán popupSpawnPoint thì lấy vị trí quái + 1.5m lên trên)
        Vector3 spawnPos = popupSpawnPoint != null ? popupSpawnPoint.position : transform.position + Vector3.up * 1.5f;
        
        // Sinh ra Prefab
        GameObject popup = Instantiate(damagePopupPrefab, spawnPos, Quaternion.identity);
        
        // Truyền sát thương vào script, set isPlayerHit = false
        popup.GetComponent<DamagePopup>().Setup(amount, false);
     }
     // Cập nhật thanh máu UI
     if (healthBarUI != null) healthBarUI.SetActive(true);
     if (healthSlider != null) healthSlider.value = currentHealth;

     // Kích hoạt giật lùi
     StopAllCoroutines();
     StartCoroutine(KnockbackRoutine());

     // Chạy Animation bị đánh (nếu có Trigger "Hit" trong Animator)
     if (animator != null) animator.SetTrigger("Hit");

     if (currentHealth <= 0) Die();
    }
    
    private IEnumerator KnockbackRoutine()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) yield break;

        Vector3 direction = (transform.position - player.transform.position).normalized;
        direction.y = 0;

        float timer = 0;
        while (timer < knockbackDuration)
        {
            transform.position += direction * knockbackForce * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }
    }

    void Die()
    {
        GetComponent<EnemyLoot>()?.Drop();
        GetComponent<EnemyReward>()?.GiveReward();
        if (IsDead) return;
        IsDead = true;
        if (animator != null) animator.SetTrigger("Die");
        if (healthBarUI != null) healthBarUI.SetActive(false);

        if (deathEventChannel != null)
        {
          deathEventChannel.RaiseEnemyDeath(enemyType, gameObject);
        }
        
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
         
        //  EXP
        if (PlayerLevel.Instance != null)
       {
        PlayerLevel.Instance.AddExp(20f); // Thay 20 bằng lượng Exp bạn muốn
       }

       //  Gold
      if (WalletManager.Instance != null)
      {
        WalletManager.Instance.Earn(10); // Thay 10 bằng lượng Vàng bạn muốn
      }

        Destroy(gameObject, 2f);
    }
}
