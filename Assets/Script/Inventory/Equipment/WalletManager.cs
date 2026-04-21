using UnityEngine;
using TMPro;
using System;

public class WalletManager : MonoBehaviour
{
    public static WalletManager Instance { get; private set; }
 
    [Header("Starting Gold")]
    [SerializeField] private int startingGold = 100;
 
    [Header("UI (có thể để trống)")]
    [SerializeField] private TextMeshProUGUI goldText;
 
    public int Gold { get; private set; }
 
    public event Action<int> OnGoldChanged;
 
    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
 
    void Start()
    {
        Gold = startingGold;
        RefreshUI();
    }
 
    public bool Spend(int amount)
    {
        if (Gold < amount)
        {
            Debug.Log($"[Wallet] Không đủ gold! Cần {amount}, có {Gold}");
            return false;
        }
        Gold -= amount;
        Debug.Log($"[Wallet] Chi {amount} gold. Còn: {Gold}");
        OnGoldChanged?.Invoke(Gold);
        RefreshUI();
        return true;
    }
 
    public void Earn(int amount)
    {
        Gold += amount;
        Debug.Log($"[Wallet] +{amount} gold. Tổng: {Gold}");
        OnGoldChanged?.Invoke(Gold);
        RefreshUI();
    }
 
    void RefreshUI()
    {
        if (goldText != null)
            goldText.text = $"{Gold:N0} G";
    }
 
    public void Save()
    {
        PlayerPrefs.SetInt("player_gold", Gold);
    }
 
    public void Load()
    {
        Gold = PlayerPrefs.GetInt("player_gold", startingGold);
        OnGoldChanged?.Invoke(Gold);
        RefreshUI();
    }
}
