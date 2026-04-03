using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _descriptionText; // drag "Text (TMP)" vào đây
    [SerializeField] private Toggle _statusToggle;      // drag "Toggle" vào đây
 
    public void Initialize(string description, int current, int required)
    {
        UpdateProgress(current, required);
        // Hiển thị "Thu thập Herb (0/5)"
        _descriptionText.text = $"{description} ({current}/{required})";
    }
 
    public void UpdateProgress(int current, int required)
    {
        _descriptionText.text = _descriptionText.text.Split('(')[0].TrimEnd()
            + $" ({current}/{required})";
        _statusToggle.isOn = current >= required;
    }
}
