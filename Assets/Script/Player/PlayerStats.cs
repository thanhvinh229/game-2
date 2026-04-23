using UnityEngine;
using UnityEngine.UI;   
using TMPro;
using System.Collections.Generic;


public class PlayerStats : MonoBehaviour , IDamageable

{
     public static PlayerStats Instance { get; private set; }

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
 
    // -------------------------------------------------------
    // THÊM MỚI: Base stats và Final stats cho Equipment System
    // -------------------------------------------------------
    [Header("Base Combat Stats")]
    public float baseAttack  = 10f;
    public float baseDefense = 5f;
 
    // Final stats = base + modifier từ equipment
    // EquipmentManager và EquipmentUI đọc các property này
    public float Attack  { get; private set; }
    public float Defense { get; private set; }
    public float MaxHP   { get; private set; }
    // -------------------------------------------------------
 
    // -------------------------------------------------------
    // THÊM MỚI: Awake — khởi tạo Singleton
    // (file gốc chưa có Awake nên Singleton chưa được set)
    // -------------------------------------------------------
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
 
        // Khởi tạo final stats từ base (chưa có equipment nào)
        RecalcStats();
    }
    // -------------------------------------------------------
 
    void Start()
    {
        currentHealth = maxHealth;
        currentMana   = maxMana;
 
        if (healthSlider != null) healthSlider.maxValue = maxHealth;
        if (manaSlider   != null) manaSlider.maxValue   = maxMana;
 
        if (healthSlider != null) healthSlider.value = currentHealth;
        if (manaSlider   != null) manaSlider.value   = currentMana;
    }
 
    void Update()
    {
        if (currentMana < maxMana)
            RegenerateMana(manaRegenRate * Time.deltaTime);
 
        if (Time.time - lastDamageTime > healthRegenDelay && currentHealth < maxHealth)
            Heal(healthRegenRate * Time.deltaTime);
 
        HandleSmoothUI();
    }
 
    void HandleSmoothUI()
    {
        if (healthSlider != null)
            healthSlider.value = Mathf.Lerp(healthSlider.value, currentHealth, Time.deltaTime * smoothSpeed);
 
        if (manaSlider != null)
            manaSlider.value = Mathf.Lerp(manaSlider.value, currentMana, Time.deltaTime * smoothSpeed);
 
        if (healthText != null)
            healthText.text = Mathf.RoundToInt(currentHealth) + " / " + Mathf.RoundToInt(maxHealth);
 
        if (manaText != null)
            manaText.text = Mathf.RoundToInt(currentMana) + " / " + Mathf.RoundToInt(maxMana);
    }
 
    // --- Máu ---
    public void TakeDamage(float amount)
    {
        if (IsDead) return;
 
        currentHealth -= amount;
        currentHealth  = Mathf.Clamp(currentHealth, 0, maxHealth);
        lastDamageTime = Time.time;
 
        if (currentHealth <= 0) Die();
    }
 
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth  = Mathf.Clamp(currentHealth, 0, maxHealth);
    }
 
    // --- Mana ---
    public bool UseMana(float amount)
    {
        if (currentMana < amount) return false;
        currentMana -= amount;
        return true;
    }
 
    public void RegenerateMana(float amount)
    {
        currentMana += amount;
        currentMana  = Mathf.Clamp(currentMana, 0, maxMana);
    }
 
    // --- Chết ---
    void Die()
    {
        IsDead = true;
        Debug.Log("Player has died!");
 
        DeathManager deathManager = FindFirstObjectByType<DeathManager>();
        if (deathManager != null)
            deathManager.TriggerDeath();
        else
            Debug.LogError("Không tìm thấy DeathManager trong Scene!");

            WaveManager waveManager = FindFirstObjectByType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.TriggerDefeat();
        }
    }
 
    public void ResetStats()
    {
        IsDead        = false;
        currentHealth = maxHealth;
        currentMana   = maxMana;
        HandleSmoothUI();

        WaveManager waveManager = FindFirstObjectByType<WaveManager>();
       if (waveManager != null)
      {
        waveManager.ResetWaveUI(); 
      }
    }
 
    // -------------------------------------------------------
    // THÊM MỚI: Equipment stat system
    // -------------------------------------------------------
 
    // Tính lại final stats từ base (gọi khi Awake)
    void RecalcStats()
    {
        Attack  = baseAttack;
        Defense = baseDefense;
        MaxHP   = maxHealth;
    }
 
    /// <summary>
    /// Gọi bởi EquipmentManager khi trang bị hoặc tháo đồ.
    /// add = true  → cộng modifier (equip)
    /// add = false → trừ modifier  (unequip)
    /// </summary>
    public void ApplyModifiers(List<StatModifier> mods, bool add)
    {
        if (mods == null) return;
 
        float sign = add ? 1f : -1f;
 
        foreach (var mod in mods)
        {
            switch (mod.statName)
            {
                case "Attack":
                    Attack  += mod.value * sign;
                    break;
 
                case "Defense":
                    Defense += mod.value * sign;
                    break;
 
                case "HP":
                    MaxHP     += mod.value * sign;
                    maxHealth += mod.value * sign;   // cập nhật luôn maxHealth thật
                    // Clamp currentHealth nếu maxHealth bị giảm
                    currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
                    if (healthSlider != null) healthSlider.maxValue = maxHealth;
                    break;
 
                case "Mana":
                    maxMana     += mod.value * sign;
                    currentMana  = Mathf.Clamp(currentMana, 0, maxMana);
                    if (manaSlider != null) manaSlider.maxValue = maxMana;
                    break;
 
                default:
                    Debug.LogWarning($"[PlayerStats] Stat không xác định: {mod.statName}");
                    break;
            }
        }
 
        // Đảm bảo Attack/Defense không âm
        Attack  = Mathf.Max(0, Attack);
        Defense = Mathf.Max(0, Defense);
        MaxHP   = Mathf.Max(1, MaxHP);
    }

     
}
