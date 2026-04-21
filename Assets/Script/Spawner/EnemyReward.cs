using UnityEngine;

public class EnemyReward : MonoBehaviour
{
    [Header("EXP")]
    [SerializeField] private float minExp = 20f;
    [SerializeField] private float maxExp = 40f;
 
    [Header("Gold")]
    [SerializeField] private int minGold = 5;
    [SerializeField] private int maxGold = 15;
 
    [Header("Nhân hệ số theo level enemy (tuỳ chọn)")]
    [SerializeField] private bool  scaleWithLevel = false;
    [SerializeField] private int   enemyLevel     = 1;
    [SerializeField] private float levelMultiplier = 1.1f; // +10% mỗi level
 
    public void GiveReward()
    {
        float scale = scaleWithLevel
            ? Mathf.Pow(levelMultiplier, enemyLevel - 1)
            : 1f;
 
        float exp  = Random.Range(minExp, maxExp)  * scale;
        int   gold = Mathf.RoundToInt(Random.Range(minGold, maxGold) * scale);
 
        if (PlayerLevel.Instance != null)
            PlayerLevel.Instance.AddExp(exp);
 
        if (WalletManager.Instance != null)
            WalletManager.Instance.Earn(gold);
 
        Debug.Log($"[Reward] +{exp:F0} EXP  +{gold} Gold");
    }
}
