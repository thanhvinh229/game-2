using TMPro;
using UnityEngine;

public class InteractionPrompt : MonoBehaviour
{
    [SerializeField] private TMP_Text _promptText;
    public void SetPrompt(string prompt)
    {
        _promptText.text = prompt;
    }
}