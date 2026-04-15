using UnityEngine;
using System.Collections;

[System.Serializable]
public class SkillSlot
{
    public KeyCode activationKey;
    public SkillData skillData; // Nắm giữ Asset thông tin kỹ năng
    
    [HideInInspector] 
    public float nextAvailableTime = 0f; // Thời gian runtime, nằm ngoài Scriptable Object
 
    public LayerMask Enemy;
}
 
public class PlayerSkills : MonoBehaviour
{
    [Header("References")]
    private PlayerController playerController;
    public PlayerStats playerStats;
    public Animator animator;
    // Lấy trực tiếp từ playerController, không cần assign tay trong Inspector
    private PlayerCombatState combatState => playerController.combatState;
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
    
 
    void Awake()
    {
     playerController = GetComponent<PlayerController>();
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
    currentActiveSkill = slot.skillData;
 
    // 1. Vào trạng thái chiến đấu
    if (combatState != null)
    {
        combatState.EnterCombatState();
    }
    if (playerController != null) 
    {
        playerController.ToggleWeaponVisibility(0); 
    }
 
    // 2. Chạy Animation
    if (animator != null && !string.IsNullOrEmpty(currentActiveSkill.animationTriggerName))
    {
        animator.SetTrigger(currentActiveSkill.animationTriggerName);
    }
 
    // 3. Thiết lập hồi chiêu (chỉ set 1 lần)
    slot.nextAvailableTime = Time.time + currentActiveSkill.cooldown;
 
    // 4. Phát âm thanh
    if (audioSource != null && currentActiveSkill.attackSound != null)
    {
        audioSource.PlayOneShot(currentActiveSkill.attackSound);
    }
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
        // KIỂM TRA: Nếu là chiêu Buff thì KHÔNG  gây sát thương 
        if (currentActiveSkill.isBuffSkill) return;
 
        // 5. Kiểm tra va chạm gây sát thương
          int enemyLayer = LayerMask.GetMask("Enemy");
          Collider[] hitEnemies = Physics.OverlapSphere(hitPoint.position, 2f,enemyLayer);
    
        foreach (Collider Enemy in hitEnemies)
        {
            if (Enemy.CompareTag("Player")) continue;
 
            IDamageable damageable = Enemy.GetComponent<IDamageable>();
             if (damageable != null)
        {
            damageable.TakeDamage(10f * currentActiveSkill.damageMultiplier);
            if (audioSource != null && currentActiveSkill.hitSound != null)
                audioSource.PlayOneShot(currentActiveSkill.hitSound);
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
 
    // Gọi cho Skill 1 — Chém vùng rộng hình cung trước mặt
    [Header("Slash Arc Settings")]
    [Tooltip("Bán kính chém (m)")]
    public float slashRadius = 4f;
    [Tooltip("Góc cung chém (độ), ví dụ 120 = quạt 120° trước mặt")]
    [Range(10f, 360f)]
    public float slashAngle = 120f;
    [Tooltip("Thời gian freeze frame khi chém trúng (giây)")]
    public float slashFreezeFrameDuration = 0.05f;
    [Tooltip("Offset xoay VFX (độ). Chỉnh nếu VFX bị ngược hoặc lệch hướng")]
    public Vector3 vfxRotationOffset = new Vector3(0f, 180f, 0f);
 
    public void OnSlashArc()
    {
        if (currentActiveSkill == null) return;
 
        // 1. Spawn VFX tại hitPoint (hoặc gốc nhân vật nếu không có)
        Vector3 vfxOrigin = hitPoint != null ? hitPoint.position : transform.position;
        if (currentActiveSkill.vfxPrefab != null)
        {
            // Xoay theo hướng nhân vật + offset để căn chỉnh VFX cho đúng
            Quaternion vfxRot = transform.rotation * Quaternion.Euler(vfxRotationOffset);
            Instantiate(currentActiveSkill.vfxPrefab, vfxOrigin, vfxRot);
        }
 
        if (currentActiveSkill.isBuffSkill) return;
 
        // 2. Lấy tất cả collider trong bán kính chém
        int enemyLayer = LayerMask.GetMask("Enemy");
        Collider[] candidates = Physics.OverlapSphere(vfxOrigin, slashRadius, enemyLayer);
 
        bool hitAny = false;
        foreach (Collider col in candidates)
        {
            if (col.CompareTag("Player")) continue;
 
            // 3. Lọc theo góc cung — chỉ kẻ địch nằm trong vùng quạt trước mặt
            Vector3 dirToEnemy = (col.transform.position - transform.position).normalized;
            float dot          = Vector3.Dot(transform.forward, dirToEnemy);
            float halfAngleCos = Mathf.Cos(slashAngle * 0.5f * Mathf.Deg2Rad);
 
            if (dot < halfAngleCos) continue; // Nằm ngoài cung → bỏ qua
 
            // 4. Gây sát thương (dùng Attack thật của player)
            IDamageable damageable = col.GetComponent<IDamageable>();
            if (damageable != null)
            {
                float baseDmg = playerStats != null ? playerStats.Attack : 10f;
                damageable.TakeDamage(baseDmg * currentActiveSkill.damageMultiplier);
 
                if (audioSource != null && currentActiveSkill.hitSound != null)
                    audioSource.PlayOneShot(currentActiveSkill.hitSound);
 
                hitAny = true;
            }
        }
 
        // 5. Freeze frame game feel khi chém trúng ít nhất 1 kẻ
        if (hitAny)
            RequestFreezeFrame(slashFreezeFrameDuration);
    }
 
    // Gọi cho Skill 2
    public void ActivateBuff()
    {
        if (currentActiveSkill == null) return;
 
        // Thêm hiệu ứng hào quang
        OnBuff();
        Instantiate(currentActiveSkill.vfxPrefab, transform.position, Quaternion.identity, transform);
        Debug.Log("Buff sát thương đã kích hoạt!");
    }
 
    // Gọi cho Skill 3 (AOE)
    public void OnAOEHit()
    {
    if (currentActiveSkill == null) return;
 
    if (currentActiveSkill.vfxPrefab != null)
        Instantiate(currentActiveSkill.vfxPrefab, transform.position, Quaternion.identity);
 
    // KIỂM TRA: Nếu là chiêu Buff thì KHÔNG  gây sát thương 
    if (currentActiveSkill.isBuffSkill) return;
 
    // Chỉ quái vật mới nhận sát thương
    int enemyLayer = LayerMask.GetMask("Enemy");
    Collider[] enemies = Physics.OverlapSphere(transform.position, 6f, enemyLayer);
    foreach (Collider e in enemies)
    {
         if (e.CompareTag("Player") || e.gameObject == this.gameObject) continue;
 
        IDamageable d = e.GetComponent<IDamageable>();
        if (d != null) d.TakeDamage(20f * currentActiveSkill.damageMultiplier);
    }
}
 
    
}