using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private Transform player;
    private AudioSource audioSource;
    private PlayerStats playerStats;

    [Header("Combat Settings")]
    public float attackRange = 2f;      
    public float attackCooldown = 2f;   
    private float lastAttackTime;

    [Header("Special Attack Chance")]
    [Range(0, 100)]
    public int attack4Chance = 20; // Tỉ lệ tung đòn 4 (Ví dụ: 20%)

    [Header("Damage Settings")]
    public Transform attackPoint;       
    public float attackRadius = 1f;     
    public LayerMask playerLayer;       
    public float damageAmount = 20f;   

    [Header("Audio Settings")]
    public AudioClip[] attackSounds;   
    public AudioClip footstepSound;     
    public AudioClip hitSound; 

    void Start()
    {
      agent = GetComponent<NavMeshAgent>();
      anim = GetComponent<Animator>();
     audioSource = GetComponent<AudioSource>();
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
        if (playerObj != null) 
        {
          player = playerObj.transform;
          playerStats = playerObj.GetComponent<PlayerStats>(); 
        }
    }

    void Update()
    {
        if (player == null || playerStats == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            // Trong tầm đánh -> Dừng lại và nhìn mặt người chơi
            agent.isStopped = true;
            FaceTarget();

            // Kiểm tra thời gian hồi chiêu
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                PerformRandomAttack(); // Gọi đòn đánh ngẫu nhiên 1, 2, 3 hoặc 4
                lastAttackTime = Time.time;
            }
        }
        else
        {
            // Ngoài tầm đánh -> Chạy theo
            // (Vì bạn dùng logic cũ, quái sẽ lập tức ngắt đòn đánh để chạy nếu Player lùi lại
            // trừ khi bạn cài đặt Animator chặn việc này)
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        if (playerStats.currentHealth <= 0) 
        {
           StopAllActions();
            return; 
        }
          

        // Cập nhật Animation di chuyển
        anim.SetFloat("Speed", agent.velocity.magnitude);
    }

    void FaceTarget()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; 
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void PerformRandomAttack()
    {
        // 1. Kiểm tra xem có trúng tỉ lệ ra đòn 4 (đòn đặc biệt) không
        int randomRoll = Random.Range(0, 100); 

        if (randomRoll < attack4Chance)
        {
            // Nếu trúng tỉ lệ, tung đòn 4
            anim.SetTrigger("Attack4");
            Debug.Log("Quái tung đòn 4 đặc biệt!");
        }
        else
        {
            // 2. Nếu không ra đòn 4, thì random đều giữa đòn 1, 2 và 3
            int randomNormalAttack = Random.Range(1, 4); // Random.Range(1, 4) sẽ trả về số nguyên 1, 2, hoặc 3
            
            if (randomNormalAttack == 1)
                anim.SetTrigger("Attack1");
            else if (randomNormalAttack == 2)
                anim.SetTrigger("Attack2");
            else if (randomNormalAttack == 3)
                anim.SetTrigger("Attack3");
        }
    }

    // === HÀM GÂY SÁT THƯƠNG GỌI TỪ ANIMATION EVENT ===
    public void DealDamageToPlayer()
    {
        if (attackPoint == null) return;

        Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRadius, playerLayer);
        
        foreach (Collider hit in hits)
    {
        // Thử lấy PlayerStats trực tiếp để truyền transform
        PlayerStats pStats = hit.GetComponent<PlayerStats>();
        if (pStats != null)
        {
            // TRUYỀN THÊM transform của quái ở đây
            pStats.TakeDamage(damageAmount, transform); 
            Debug.Log("Quái đã chém trúng Player và truyền vị trí!");
            break; 
        }
    }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }

    public void PlayAttackSound(int index)
    {
        if (audioSource != null && attackSounds.Length > index && attackSounds[index] != null)
        {
            audioSource.PlayOneShot(attackSounds[index]);
        }
    }

    // Hàm này dùng để phát tiếng bước chân
    public void PlayFootstep()
    {
        if (audioSource != null && footstepSound != null)
        {
            // Chỉnh âm lượng nhỏ lại một chút cho tiếng bước chân đỡ ồn
            audioSource.PlayOneShot(footstepSound, 0.5f);
        }
    }


   void StopAllActions()
   {
      agent.isStopped = true;
      anim.SetFloat("Speed", 0); // Về Idle
      // Bạn có thể thêm lệnh để tắt các trigger tấn công đang dở dang
      anim.ResetTrigger("Attack1");
      anim.ResetTrigger("Attack2");
   }
}