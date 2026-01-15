using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionActionUILayer : MonoBehaviour
{
    [SerializeField] private TMP_Text _interactionPromptText;
    [SerializeField] private Image _progressImage;
    public void SetInteractionPromptText(string promptText) => _interactionPromptText.text = promptText;
    public void SetProgress(float currentProgress)
    {
        currentProgress = Mathf.Clamp01(currentProgress);
        _progressImage.fillAmount = currentProgress;
    }
    void OnEnable()
    {
        SetProgress(0f);
    }
    void OnDisable()
    {
        SetProgress(0f);
    }
}
