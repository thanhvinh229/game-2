using UnityEngine;
using UnityEngine.UI;   
using TMPro;
using System.Collections.Generic;
using System.Collections;


public class PlayerStats : MonoBehaviour, IDamageable
{
    public static PlayerStats Instance { get; private set; }

    [Header("Health Settings")]
    public float maxHealth = 200f;
    public float currentHealth;
    public Slider healthSlider;
    public TextMeshProUGUI healthText;
    public bool IsAlive => currentHealth > 0;
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
    public float healthRegenRate  = 2f;

    [Header("Base Combat Stats")]
    public float baseAttack  = 10f;
    public float baseDefense = 5f;

    [Header("Damage UI")]
    public GameObject damagePopupPrefab;

    public float Attack  { get; private set; }
    public float Defense { get; private set; }
    public float MaxHP   { get; private set; }

    // Buff tạm thời
    private Coroutine _buffCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        RecalcStats();
    }

    void Start()
    {
        currentHealth = maxHealth;
        currentMana   = maxMana;

        if (healthSlider != null) healthSlider.maxValue = maxHealth;
        if (manaSlider   != null) manaSlider.maxValue   = maxMana;
        if (healthSlider != null) healthSlider.value    = currentHealth;
        if (manaSlider   != null) manaSlider.value      = currentMana;
    }

    void Update()
    {
        if (currentMana < maxMana)
            RegenerateMana(manaRegenRate * Time.deltaTime);

        if (Time.time - lastDamageTime > healthRegenDelay && currentHealth < maxHealth)
            Heal(healthRegenRate * Time.deltaTime);

        HandleSmoothUI();
    }

    public void HandleSmoothUI()
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

    // ── Nhận sát thương — Defense giảm damage ──────────────────────────
    public void TakeDamage(float rawDamage)
    {
      TakeDamage(rawDamage, null);
    }
    public void TakeDamage(float rawDamage, Transform attackerTransform = null )
    {
        if (IsDead) return;

        // Công thức: damage thực = rawDamage * 100 / (100 + Defense)
        // Defense = 10  → giảm ~9%  | Defense = 50  → giảm ~33%
        // Defense = 100 → giảm ~50% | Defense = 200 → giảm ~67%
        float actualDamage = rawDamage * 100f / (100f + Defense);   
        actualDamage = Mathf.Max(1f, actualDamage); // tối thiểu 1 damage

        currentHealth  -= actualDamage;
        currentHealth   = Mathf.Clamp(currentHealth, 0, maxHealth);
        lastDamageTime  = Time.time;

        if (healthSlider != null) healthSlider.value = currentHealth;
        
        if (damagePopupPrefab != null)
        {
         Vector3 spawnPos = transform.position + Vector3.up * 2f; // Bay trên đầu player
         GameObject popup = Instantiate(damagePopupPrefab, spawnPos, Quaternion.identity);
        
         var dp = popup.GetComponent<DamagePopup>();
        if (dp != null) dp.Setup(actualDamage, true);
        }

        PlayerController pc = GetComponent<PlayerController>();
       if (pc != null && attackerTransform != null)
       {
         pc.OnHit(attackerTransform); 
       }

        Debug.Log($"[PlayerStats] Nhận {rawDamage:F0} raw → {actualDamage:F1} thực (DEF:{Defense:F0})");

        if (currentHealth <= 0 && !IsDead) 
        {
           Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth  = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

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

    void Die()
    {
        IsDead = true;
        Debug.Log("Player has died!");
        DeathManager deathManager = FindFirstObjectByType<DeathManager>();
        if (deathManager != null) deathManager.TriggerDeath();
        else Debug.LogError("Không tìm thấy DeathManager!");
    }

    public void ResetStats()
    {
        IsDead        = false;
        currentHealth = maxHealth;
        currentMana   = maxMana;
        HandleSmoothUI();
    }

    void RecalcStats()
    {
        Attack  = baseAttack;
        Defense = baseDefense;
        MaxHP   = maxHealth;
    }

    public void ApplyModifiers(List<StatModifier> mods, bool add)
    {
        if (mods == null) return;
        float sign = add ? 1f : -1f;

        foreach (var mod in mods)
        {
            switch (mod.statName)
            {
                case "Attack":  Attack  += mod.value * sign; break;
                case "Defense": Defense += mod.value * sign; break;
                case "HP":
                    MaxHP     += mod.value * sign;
                    maxHealth += mod.value * sign;
                    currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
                    if (healthSlider != null) healthSlider.maxValue = maxHealth;
                    break;
                case "Mana":
                    maxMana    += mod.value * sign;
                    currentMana = Mathf.Clamp(currentMana, 0, maxMana);
                    if (manaSlider != null) manaSlider.maxValue = maxMana;
                    break;
                default:
                    Debug.LogWarning($"[PlayerStats] Stat không xác định: {mod.statName}");
                    break;
            }
        }

        Attack  = Mathf.Max(0, Attack);
        Defense = Mathf.Max(0, Defense);
        MaxHP   = Mathf.Max(1, MaxHP);
    }

    // ── Buff tạm thời ATK + DEF ─────────────────────────────────────────
    /// <summary>
    /// Tăng ATK và DEF trong thời gian duration giây rồi tự hoàn nguyên
    /// </summary>
    public void ApplyTempBuff(float atkBonus, float defBonus, float duration)
    {
        if (_buffCoroutine != null)
            StopCoroutine(_buffCoroutine);
        _buffCoroutine = StartCoroutine(TempBuffCoroutine(atkBonus, defBonus, duration));
    }

    IEnumerator TempBuffCoroutine(float atkBonus, float defBonus, float duration)
    {
        Attack  += atkBonus;
        Defense += defBonus;
        Debug.Log($"[Buff] +{atkBonus} ATK +{defBonus} DEF trong {duration}s");

        yield return new WaitForSeconds(duration);

        Attack  -= atkBonus;
        Defense -= defBonus;
        Attack   = Mathf.Max(0, Attack);
        Defense  = Mathf.Max(0, Defense);
        Debug.Log("[Buff] Hết hiệu lực buff");
        _buffCoroutine = null;
    }
}

