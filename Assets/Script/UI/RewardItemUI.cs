using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardItemUI : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _quantityText;

    public void Initialize(ItemReward item)
    {
        _nameText.text = item.ItemName;
        _quantityText.text = $"x{item.Quantity}";

        if (item.Icon != null)
            _iconImage.sprite = item.Icon;
    }
}
