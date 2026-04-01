using UnityEngine;
using UnityEngine.UI;   


public class PlayerStats : MonoBehaviour , IDamageable
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthSlider;

    [Header("Mana Settings")]
    public float maxMana = 100f;
    public float currentMana;
    public float manaRegenRate = 5f; // Tốc độ hồi mana mỗi giây
    public Slider manaSlider;


    [Header("Smooth UI Settings")]
    public float smoothSpeed = 5f;
    private float targetHealth;
    private float targetMana;


    private float lastDamageTime;
    public float healthRegenDelay = 5f; // 5 giây sau khi bị đánh mới bắt đầu hồi
    public float healthRegenRate = 2f;
    void Start()
    {
        // Khởi tạo giá trị ban đầu
        currentHealth = maxHealth;
        currentMana = maxMana;
         
        // Đặt mục tiêu ban đầu
        targetHealth = currentHealth;
        targetMana = currentMana;

        // Cập nhật giá trị Max cho Slider UI
        if (healthSlider != null) healthSlider.maxValue = maxHealth;
        if (manaSlider != null) manaSlider.maxValue = maxMana;
        
        UpdateUI();
    }

    void Update()
    {
        // Tự động hồi Mana theo thời gian
        if (currentMana < maxMana)
        {
            RegenerateMana(manaRegenRate * Time.deltaTime);
        }
        
        // Tự động Heal nếu đã đủ thời gian sau khi bị đánh
        if (Time.time - lastDamageTime > healthRegenDelay && currentHealth < maxHealth)
        {
          currentHealth += healthRegenRate * Time.deltaTime;
          targetHealth = currentHealth;
        }
    }

    // --- Các hàm xử lý Máu ---
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        targetHealth = currentHealth;
        UpdateUI();

        if (currentHealth <= 0) Die();
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();
    }

    // --- Các hàm xử lý Mana ---
    public bool UseMana(float amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            UpdateUI();
            return true; // Đủ mana để dùng chiêu
        }
        return false; // Không đủ mana
    }

    public void RegenerateMana(float amount)
    {
        currentMana += amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
        UpdateUI();
    }

    // Cập nhật giao diện
    void UpdateUI()
    {
        if (healthSlider != null) healthSlider.value = currentHealth;
        if (manaSlider != null) manaSlider.value = currentMana;
    }

    void HandleSmoothUI()
{
    if (healthSlider != null)
    {
        // Lerp giúp thanh chạy nhanh lúc đầu và chậm lại khi gần đến đích
        healthSlider.value = Mathf.Lerp(healthSlider.value, targetHealth, Time.deltaTime * smoothSpeed);
    }

    if (manaSlider != null)
    {
        manaSlider.value = Mathf.Lerp(manaSlider.value, targetMana, Time.deltaTime * smoothSpeed);
    }
}

    void Die()
    {
        Debug.Log("Player has died!");
        // Thêm logic xử lý khi chết (VD: load lại cảnh, chạy hoạt ảnh)
    }
}
