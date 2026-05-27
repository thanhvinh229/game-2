using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestNotificationUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _messageText;
 
    [Header("Settings")]
    [SerializeField] private float _displayDuration = 3f;
    [SerializeField] private float _fadeInDuration = 0.3f;
    [SerializeField] private float _fadeOutDuration = 0.3f;
 
    [Header("Icons")]
    [SerializeField] private Sprite _questReceivedIcon;
    [SerializeField] private Sprite _questCompletedIcon;
    [SerializeField] private Sprite _objectiveCompletedIcon;
 
    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _notificationSfx;
    [Range(0f, 1f)][SerializeField] private float _volume = 0.7f;
 
    private Coroutine _currentNotification;
 
    void Awake()
    {
       
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
    }
 
    public void ShowQuestReceived(string questTitle)
    {
        ShowNotification(_questReceivedIcon, "Nhiệm vụ mới", questTitle);
    }
 
    public void ShowQuestCompleted(string questTitle)
    {
        ShowNotification(_questCompletedIcon, "Hoàn thành", questTitle);
    }
 
    public void ShowObjectiveCompleted(string objectiveDescription)
    {
        ShowNotification(_objectiveCompletedIcon, "Mục tiêu hoàn thành", objectiveDescription);
    }
 
    public void ShowNotification(Sprite icon, string title, string message)
    {
        
        if (_currentNotification != null)
        {
            StopCoroutine(_currentNotification);
            _currentNotification = null;
        }
 
        // Cập nhật nội dung
        if (_iconImage != null && icon != null)
            _iconImage.sprite = icon;
 
        if (_titleText != null)
            _titleText.text = title;
 
        if (_messageText != null)
            _messageText.text = message;
 
        // Phát âm thanh
        if (_audioSource != null && _notificationSfx != null)
            _audioSource.PlayOneShot(_notificationSfx, _volume);
 
       
        _currentNotification = StartCoroutine(DisplayNotificationCoroutine());
    }
 
    private IEnumerator DisplayNotificationCoroutine()
    {
        
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }
 
        // Fade in
        yield return FadeCanvasGroup(0f, 1f, _fadeInDuration);
 
        // Hiển thị
        yield return new WaitForSeconds(_displayDuration);
 
        // Fade out
        yield return FadeCanvasGroup(1f, 0f, _fadeOutDuration);
 
        if (_canvasGroup != null)
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
 
        _currentNotification = null;
    }
 
    private IEnumerator FadeCanvasGroup(float startAlpha, float endAlpha, float duration)
    {
        if (_canvasGroup == null) yield break;
 
        float elapsed = 0f;
        _canvasGroup.alpha = startAlpha;
 
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }
 
        _canvasGroup.alpha = endAlpha;
    }
 
#if UNITY_EDITOR
    void OnValidate()
    {
        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>();
        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();
    }
#endif
}
