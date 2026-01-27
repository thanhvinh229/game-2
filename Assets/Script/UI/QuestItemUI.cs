using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Toggle _statusToggle;

    public void Initialize(string name , bool isCompleted)
    {
        _nameText.text = name;
        _statusToggle.isOn = isCompleted;       

    }

    public bool UpdateStatus(bool isCompleted)
    {
        _statusToggle.isOn= isCompleted;
    }
}
