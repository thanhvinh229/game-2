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
    [Range(0f, 1f)] [SerializeField] private float _volume = 0.7f;
 
    private Coroutine _currentNotification;
 
    void Awake()
    {
        // Start hidden
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 0f;
        }
        gameObject.SetActive(false);
    }
 
    /// <summary>
    /// Hiển thị notification khi nhận quest mới
    /// </summary>
    public void ShowQuestReceived(string questTitle)
    {
        ShowNotification(
            _questReceivedIcon,
            "Nhiệm vụ mới",
            questTitle
        );
    }
 
    /// <summary>
    /// Hiển thị notification khi hoàn thành quest
    /// </summary>
    public void ShowQuestCompleted(string questTitle)
    {
        ShowNotification(
            _questCompletedIcon,
            "Hoàn thành",
            questTitle
        );
    }
 
    /// <summary>
    /// Hiển thị notification khi hoàn thành objective
    /// </summary>
    public void ShowObjectiveCompleted(string objectiveDescription)
    {
        ShowNotification(
            _objectiveCompletedIcon,
            "Mục tiêu hoàn thành",
            objectiveDescription
        );
    }
 
    /// <summary>
    /// Core notification display method
    /// </summary>
    public void ShowNotification(Sprite icon, string title, string message)
    {
        gameObject.SetActive(true);

        // Cancel previous notification if exists
        if (_currentNotification != null)
        {
            StopCoroutine(_currentNotification);
        }
 
        // Update content
        if (_iconImage != null && icon != null)
        {
            _iconImage.sprite = icon;
        }
 
        if (_titleText != null)
        {
            _titleText.text = title;
        }
 
        if (_messageText != null)
        {
            _messageText.text = message;
        }
 
        // Start display coroutine
        _currentNotification = StartCoroutine(DisplayNotificationCoroutine());

        // Phát âm thanh 
        if (_audioSource != null && _notificationSfx != null)
        {
            _audioSource.PlayOneShot(_notificationSfx, _volume);
        }

        if (_currentNotification != null) StopCoroutine(_currentNotification);
        
        if (_iconImage != null && icon != null) _iconImage.sprite = icon;
        if (_titleText != null) _titleText.text = title;
        if (_messageText != null) _messageText.text = message;
 
        _currentNotification = StartCoroutine(DisplayNotificationCoroutine());
    }
 
    private IEnumerator DisplayNotificationCoroutine()
    {
        
 
        // Fade in
        yield return FadeCanvasGroup(_canvasGroup, 0f, 1f, _fadeInDuration);
 
        // Display
        yield return new WaitForSeconds(_displayDuration);
 
        // Fade out
        yield return FadeCanvasGroup(_canvasGroup, 1f, 0f, _fadeOutDuration);
 
        gameObject.SetActive(false);
        _currentNotification = null;
    }
 
    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        if (canvasGroup == null) yield break;
 
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }
 
        canvasGroup.alpha = endAlpha;
    }
 
    #if UNITY_EDITOR
    void OnValidate()
    {
        // Auto-find components
        if (_canvasGroup == null)
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }
        if (_audioSource == null) 
        {
            _audioSource = GetComponent<AudioSource>();
        }
    }
    #endif
}
