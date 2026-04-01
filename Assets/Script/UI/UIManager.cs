using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public PlayerStats playerStats;
    public Slider Health;
    public Slider Mana;

    void Update()
    {
        // Cập nhật giá trị Slider dựa trên tỉ lệ %
        Health.value = playerStats.currentHealth / playerStats.maxHealth;
        Mana.value = playerStats.currentMana / playerStats.maxMana;
    }
}
