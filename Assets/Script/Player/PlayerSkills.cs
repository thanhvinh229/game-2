using UnityEngine;
using System.Collections;

[System.Serializable]
public class SkillSlot
{
    public KeyCode activationKey;
    public SkillData skillData; // Nắm giữ Asset thông tin kỹ năng
    
    [HideInInspector] 
    public float nextAvailableTime = 0f; // Thời gian runtime, nằm ngoài Scriptable Object
}

public class PlayerSkills : MonoBehaviour
{
    [Header("References")]
    public PlayerStats playerStats;
    public Animator animator;
    public PlayerCombatState combatState;
    public AudioSource audioSource; 
    

    [Header("Assigned Skills")]
    public Transform hitPoint;
    public SkillSlot[] skillSlots;
    private SkillData currentActiveSkill;
     // Các slot kỹ năng nhân vật đang trang bị

    void Start()
    {
        if (playerStats == null) playerStats = GetComponent<PlayerStats>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        foreach (SkillSlot slot in skillSlots)
        {
            // Bỏ qua nếu slot trống
            if (slot.skillData == null) continue;

            if (Input.GetKeyDown(slot.activationKey))
            {
                TryUseSkill(slot);
            }
        }
    }

    void TryUseSkill(SkillSlot slot)
    {
        if (Time.time < slot.nextAvailableTime)
        {
            Debug.Log($"[{slot.skillData.skillName}] đang hồi chiêu!");
            return;
        }

        if (playerStats.UseMana(slot.skillData.manaCost))
        {
            ExecuteSkill(slot);
        }
        else
        {
            Debug.Log($"Không đủ Mana cho [{slot.skillData.skillName}]!");
        }
    }

    void ExecuteSkill(SkillSlot slot)
    {
        
        // 1. Ép nhân vật vào trạng thái chiến đấu và reset timer 10s
        if (combatState != null)
        {
            combatState.EnterCombatState();
        }

        // 2. Chạy Animation
        if (animator != null && !string.IsNullOrEmpty(slot.skillData.animationTriggerName))
        {
            animator.SetTrigger(slot.skillData.animationTriggerName);
        }

        // 3. Thiết lập hồi chiêu
        slot.nextAvailableTime = Time.time + slot.skillData.cooldown;
        
        // 4. Phát âm thanh kích hoạt (vung kiếm)
        if (audioSource != null && currentActiveSkill.attackSound != null)
        {
            audioSource.PlayOneShot(currentActiveSkill.attackSound);
        }

        slot.nextAvailableTime = Time.time + currentActiveSkill.cooldown;

        // Lưu thông tin kỹ năng hiện tại để dùng cho Animation Event (gây sát thương)
        currentActiveSkill = slot.skillData;
    }

    

    // PHẦN QUAN TRỌNG: Animation Event sẽ gọi hàm này
    public void OnHit()
    {
        if (currentActiveSkill == null) return;

        // 4. Tạo hiệu ứng VFX tại vị trí hitPoint
        if (currentActiveSkill.vfxPrefab != null && hitPoint != null)
        {
            Instantiate(currentActiveSkill.vfxPrefab, hitPoint.position, hitPoint.rotation);
        }

        // 5. Kiểm tra va chạm gây sát thương
        Collider[] hitEnemies = Physics.OverlapSphere(hitPoint.position, 2f);
        foreach (Collider enemy in hitEnemies)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(10f * currentActiveSkill.damageMultiplier);
                
                // Phát âm thanh khi trúng mục tiêu
                if (audioSource != null && currentActiveSkill.hitSound != null)
                {
                    audioSource.PlayOneShot(currentActiveSkill.hitSound);
                }
            }
        }
    }  

    public void OnBuff() {
    // Tăng damageMultiplier của các kỹ năng khác tạm thời
    // Hoặc hồi máu/mana ngay lập tức
    playerStats.Heal(20f);    
}  


// --- CẢM GIÁC HÀNH ĐỘNG (GAME FEEL) ---
    public void RequestFreezeFrame(float duration = 0.1f)
    {
        StartCoroutine(DoFreezeFrame(duration));
    }

    IEnumerator DoFreezeFrame(float duration)
    {
        Time.timeScale = 0.05f; // Khựng lại gần như đứng im
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }

    // --- ANIMATION EVENTS ---

    // Gọi cho Skill 1
    public void LaunchProjectile()
    {
        if (currentActiveSkill.vfxPrefab != null)
        {
            GameObject projectile = Instantiate(currentActiveSkill.vfxPrefab, hitPoint.position, hitPoint.rotation);

            Projectile projScript = projectile.GetComponent<Projectile>();
        if (projScript != null)
        {
            // Giả sử sát thương gốc là 10, nhân với hệ số của Skill
            projScript.damage = 10f * currentActiveSkill.damageMultiplier;  
        }
            // Thêm lực bay cho projectile
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if(rb) rb.linearVelocity = hitPoint.forward * 15f; 
            Destroy(projectile, 4f); // Tự hủy sau 4s 
        }
    }

    // Gọi cho Skill 2
    public void ActivateBuff()
    {
        // Thêm hiệu ứng hào quang
        Instantiate(currentActiveSkill.vfxPrefab, transform.position, Quaternion.identity, transform);
        Debug.Log("Buff sát thương đã kích hoạt!");
    }

    // Gọi cho Skill 3 (AOE)
    public void OnAOEHit()
    {
        RequestFreezeFrame(0.15f); // Khựng hình mạnh hơn cho chiêu cuối
        if (currentActiveSkill.vfxPrefab != null)
            Instantiate(currentActiveSkill.vfxPrefab, transform.position, Quaternion.identity);

        Collider[] enemies = Physics.OverlapSphere(transform.position, 6f);
        foreach (Collider e in enemies)
        {
            IDamageable d = e.GetComponent<IDamageable>();
            if (d != null) d.TakeDamage(20f * currentActiveSkill.damageMultiplier);
        }
    }
}