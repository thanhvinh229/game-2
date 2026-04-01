using UnityEngine;
using UnityEngine.UI;


public class EnemyStats : MonoBehaviour, IDamageable
{
    public float maxHealth = 50f;
    private float currentHealth;
    private bool isDead = false; // Biến kiểm tra xem quái đã chết chưa

    [Header("UI & Animation")]
    public Slider healthSlider;
    public GameObject healthBarUI;
    public Animator animator; // Kéo Animator của bộ xương vào đây

    void Start()
    {
        currentHealth = maxHealth;
        
        // Tự động tìm Animator nếu bạn quên kéo vào
        if (animator == null) animator = GetComponent<Animator>();

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        // Nếu quái đã chết rồi thì không nhận thêm sát thương nữa
        if (isDead) return;

        currentHealth -= amount;
        
        // Hiện và cập nhật thanh máu
        if (healthBarUI != null) healthBarUI.SetActive(true);
        if (healthSlider != null) healthSlider.value = currentHealth;

        // Nếu bạn có animation "bị giật lùi" (Hit React), có thể gọi ở đây:
        // animator.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log(gameObject.name + " đã bị tiêu diệt!");

        // 1. Chạy hoạt ảnh ngã gục
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // 2. Ẩn thanh máu ngay lập tức
        if (healthBarUI != null)
        {
            healthBarUI.SetActive(false);
        }

        // 3. Tắt Collider để vũ khí/kỹ năng bay xuyên qua xác chết, không bị vướng
        Collider enemyCollider = GetComponent<Collider>();
        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        // 4. Nếu quái có script di chuyển (ví dụ NavMeshAgent hoặc script EnemyAI do bạn tự viết), hãy tắt nó đi ở đây.
        // GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        // GetComponent<EnemyAI>().enabled = false;

        // 5. Tắt luôn script này để tránh các lỗi không mong muốn
        this.enabled = false;

        // 6. Xóa cái xác sau 5 giây để giải phóng bộ nhớ (bạn có thể tăng/giảm thời gian tùy ý)
        Destroy(gameObject, 5f);
    }
}
