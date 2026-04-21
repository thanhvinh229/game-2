using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class PlayerLevel : MonoBehaviour
{
    public static PlayerLevel Instance { get; private set; }
 
    [Header("Level Settings")]
    [SerializeField] private int   startLevel    = 1;
    [SerializeField] private int   maxLevel      = 50;
    [SerializeField] private float baseExp       = 100f;  // exp cần cho level 1
    [SerializeField] private float expMultiplier = 1.4f;  // mỗi level cần nhiều hơn 40%
 
    [Header("Stat tăng mỗi level")]
    [SerializeField] private float attackPerLevel  = 3f;
    [SerializeField] private float defensePerLevel = 2f;
    [SerializeField] private float hpPerLevel      = 15f;
 
    [Header("UI (có thể để trống)")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Slider          expSlider;
    
 
    public int   CurrentLevel { get; private set; }
    public float CurrentExp   { get; private set; }
    public float ExpToNextLevel => Mathf.Floor(baseExp * Mathf.Pow(expMultiplier, CurrentLevel - 1));
 
    public event Action<int> OnLevelUp;  // tham số: level mới
 
    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
 
    void Start()
    {
        CurrentLevel = startLevel;
        CurrentExp   = 0f;
        RefreshUI();
    }
 
    public void AddExp(float amount)
    {
        if (CurrentLevel >= maxLevel) return;
 
        CurrentExp += amount;
        Debug.Log($"[Level] +{amount} EXP  ({CurrentExp}/{ExpToNextLevel})");
 
        // Kiểm tra level up liên tục (có thể lên nhiều cấp cùng lúc)
        while (CurrentExp >= ExpToNextLevel && CurrentLevel < maxLevel)
        {
            CurrentExp -= ExpToNextLevel;
            LevelUp();
        }
 
        RefreshUI();
    }
 
    void LevelUp()
    {
        CurrentLevel++;
 
        // Cộng stat vào PlayerStats
        if (PlayerStats.Instance != null)
        {
            var mods = new System.Collections.Generic.List<StatModifier>
            {
                new StatModifier { statName = "Attack",  value = attackPerLevel  },
                new StatModifier { statName = "Defense", value = defensePerLevel },
                new StatModifier { statName = "HP",      value = hpPerLevel      },
            };
            PlayerStats.Instance.ApplyModifiers(mods, add: true);
            // Hồi máu khi lên cấp
            PlayerStats.Instance.Heal(hpPerLevel);
        }
 
        Debug.Log($"[Level] LÊN CẤP {CurrentLevel}! ATK+{attackPerLevel} DEF+{defensePerLevel} HP+{hpPerLevel}");
        OnLevelUp?.Invoke(CurrentLevel);
        RefreshUI();
    }
 
    void RefreshUI()
    {
        float needed = ExpToNextLevel;
 
        if (levelText != null)
           levelText.text = CurrentLevel.ToString();
 
        if (expSlider != null)
        {
            expSlider.maxValue = needed;
            expSlider.value    = CurrentExp;
        }
 
    }
 
    // Save / Load
    public void Save()
    {
        PlayerPrefs.SetInt  ("player_level", CurrentLevel);
        PlayerPrefs.SetFloat("player_exp",   CurrentExp);
    }
 
    public void Load()
    {
        CurrentLevel = PlayerPrefs.GetInt  ("player_level", startLevel);
        CurrentExp   = PlayerPrefs.GetFloat("player_exp",   0f);
        RefreshUI();
    }
}
