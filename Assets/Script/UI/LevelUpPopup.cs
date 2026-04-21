using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LevelUpPopup : MonoBehaviour
{
    [SerializeField] private GameObject      panel;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI statText;
    [SerializeField] private float           displayTime = 3f;
 
    void Start()
    {
        panel.SetActive(false);
 
        if (PlayerLevel.Instance != null)
            PlayerLevel.Instance.OnLevelUp += ShowPopup;
    }
 
    void OnDestroy()
    {
        if (PlayerLevel.Instance != null)
            PlayerLevel.Instance.OnLevelUp -= ShowPopup;
    }
 
    void ShowPopup(int newLevel)
    {
        if (levelText != null)
           levelText.text = $"Level Up! {newLevel}";
 
        if (statText != null)
            statText.text = "ATK + DEF + HP tăng!";
 
        panel.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(HideAfter(displayTime));
    }
 
    IEnumerator HideAfter(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        panel.SetActive(false);
    }
}
