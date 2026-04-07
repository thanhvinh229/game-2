using UnityEngine;
using UnityEngine.UI;   
using TMPro;


public class PlayerStats : MonoBehaviour , IDamageable
{
    [Header("Health Settings")]
    public float maxHealth = 200f;
    public float currentHealth;
    public Slider healthSlider;
    public TextMeshProUGUI healthText;
    public bool IsDead { get; private set; }

    [Header("Mana Settings")]
    public float maxMana = 100f;
    public float currentMana;
    public float manaRegenRate = 5f; 
    public Slider manaSlider;
    public TextMeshProUGUI manaText;

    [Header("Smooth UI Settings")]
    public float smoothSpeed = 5f;

    private float lastDamageTime;
    public float healthRegenDelay = 5f; 
    public float healthRegenRate = 2f;

    void Start()
    {
        // Khởi tạo giá trị ban đầu
        currentHealth = maxHealth;
        currentMana = maxMana;

        // Cập nhật giá trị Max cho Slider UI
        if (healthSlider != null) healthSlider.maxValue = maxHealth;
        if (manaSlider != null) manaSlider.maxValue = maxMana;
        
        // Đặt giá trị thanh trượt bằng với máu/mana hiện tại (không bị trượt từ 0 lên)
        if (healthSlider != null) healthSlider.value = currentHealth;
        if (manaSlider != null) manaSlider.value = currentMana;
    }

    void Update()
    {
        // 1. Tự động hồi Mana theo thời gian
        if (currentMana < maxMana)
        {
            RegenerateMana(manaRegenRate * Time.deltaTime);
        }
        
        // 2. Tự động Heal nếu đã đủ thời gian sau khi bị đánh
        if (Time.time - lastDamageTime > healthRegenDelay && currentHealth < maxHealth)
        {
            Heal(healthRegenRate * Time.deltaTime);
        }

        // 3. Gọi hàm xử lý UI mượt mà liên tục mỗi khung hình
        HandleSmoothUI();
    }

    // --- Xử lý UI đồng bộ ---
    void HandleSmoothUI()
    {
        // Xử lý Slider trượt mượt mà (Lerp)
        if (healthSlider != null)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, currentHealth, Time.deltaTime * smoothSpeed);
        }

        if (manaSlider != null)
        {
            manaSlider.value = Mathf.Lerp(manaSlider.value, currentMana, Time.deltaTime * smoothSpeed);
        }

        // Cập nhật Text hiển thị số ngay lập tức
        if (healthText != null)
        {
            healthText.text = Mathf.RoundToInt(currentHealth).ToString() + " / " + Mathf.RoundToInt(maxHealth).ToString();
        }

        if (manaText != null)
        {
            manaText.text = Mathf.RoundToInt(currentMana).ToString() + " / " + Mathf.RoundToInt(maxMana).ToString();
        }
    }

    // --- Các hàm xử lý Máu ---
    public void TakeDamage(float amount)
    {
       if (IsDead) return; // Nếu đã chết rồi thì không nhận thêm sát thương nữa

          currentHealth -= amount;
          currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
          lastDamageTime = Time.time; 

        if (currentHealth <= 0)
        {
          Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    // --- Các hàm xử lý Mana ---
    public bool UseMana(float amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            return true; // Đủ mana để dùng chiêu
        }
        return false; // Không đủ mana
    }

    public void RegenerateMana(float amount)
    {
        currentMana += amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
    }

    void Die()
    {
       IsDead = true;
       Debug.Log("Player has died!");

    // 1. Gọi đến DeathManager để xử lý hiện UI và dịch chuyển
    // Tìm đối tượng DeathManager trong Scene và kích hoạt cái chết
        DeathManager deathManager = FindFirstObjectByType<DeathManager>();  
       if (deathManager != null)
       {
        deathManager.TriggerDeath();
       }
       else
       {
        Debug.LogError("Không tìm thấy DeathManager trong Scene!");
       }
    }
       
    public void ResetStats()
    {
       IsDead = false;
       currentHealth = maxHealth;
       currentMana = maxMana;
       
       // Cập nhật lại UI ngay lập tức
        HandleSmoothUI();
    }
}
