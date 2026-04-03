using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private Transform player;

    [Header("Combat Settings")]
    public float attackRange = 2f;      // Khoảng cách bắt đầu đánh
    public float attackCooldown = 2f;   // Thời gian nghỉ giữa 2 lần đánh
    private float lastAttackTime;

    [Header("Damage Settings")]
    public Transform attackPoint;       // Điểm gắn ở tay cầm vũ khí của quái
    public float attackRadius = 1f;     // Bán kính vùng sát thương
    public LayerMask playerLayer;       // Layer của Player
    public float damageAmount = 20f;    // Lượng sát thương quái gây ra

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        
        // Tự động tìm Player trong Scene (đảm bảo Player có tag là "Player")
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            // Trong tầm đánh -> Dừng lại và nhìn mặt người chơi
            agent.isStopped = true;
            FaceTarget();

            // Kiểm tra xem đã hết thời gian hồi chiêu chưa
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                PerformRandomAttack();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            // Ngoài tầm đánh -> Chạy theo
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }

        // Cập nhật Animation di chuyển
        anim.SetFloat("Speed", agent.velocity.magnitude);
    }

    void FaceTarget()
    {
        // Tính hướng nhìn về phía người chơi nhưng bỏ qua trục Y (để quái không bị ngửa lên trời)
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; 
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void PerformRandomAttack()
    {
        // Tỉ lệ 50-50 để tung ra đòn 1 hoặc đòn 2
        int randomAttack = Random.Range(1, 3); // Lấy ngẫu nhiên số 1 hoặc 2
        
        if (randomAttack == 1)
            anim.SetTrigger("Attack1");
        else
            anim.SetTrigger("Attack2");
    }

    // === HÀM GÂY SÁT THƯƠNG GỌI TỪ ANIMATION EVENT ===
    public void DealDamageToPlayer()
    {
        if (attackPoint == null) return;

        // Quét vùng hình cầu xem có trúng Player không
        Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRadius, playerLayer);
        
        foreach (Collider hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damageAmount);
                Debug.Log("Quái đã chém trúng Player!");
                // (Chỉ trừ máu 1 lần mỗi đòn đánh nên ta có thể break luôn)
                break; 
            }
        }
    }

    // Vẽ hình cầu màu đỏ trong Scene để bạn dễ căn chỉnh vùng đánh
    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}