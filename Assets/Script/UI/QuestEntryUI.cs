using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Prefab: QuestItem có sẵn (Toggle + Text TMP)
// Dùng cho danh sách quest bên trái
public class QuestEntryUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Image _dotImage;

    [Header("Colors")]
    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _completedColor;
    [SerializeField] private Color _selectedBgColor;

    [Header("Audio")]
    [SerializeField] private AudioClip _clickSound;
    private AudioSource _audioSource;

    private Image _bgImage;
    private QuestData _data;
    private System.Action<QuestData> _onSelected;

    void Awake()
    {
        _bgImage = GetComponent<Image>();

        // Tự tạo AudioSource riêng, không cần kéo từ ngoài vào
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.ignoreListenerPause = true; // hoạt động khi timeScale = 0

        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void Initialize(QuestData data, System.Action<QuestData> onSelected)
    {
        _data = data;
        _onSelected = onSelected;
        _nameText.text = data.Id; // ← fix tên quest
        UpdateDotColor();
    }

    public void SetSelected(bool selected)
    {
        if (_bgImage != null)
            _bgImage.color = selected ? _selectedBgColor : Color.clear;
    }

    public void RefreshStatus() => UpdateDotColor();

    private void UpdateDotColor()
    {
        if (_dotImage == null) return;
        _dotImage.color = _data.Status == QuestStatus.Completed
            ? _completedColor : _activeColor;
    }

    private void OnClick()
    {
        if (_clickSound != null)
            _audioSource.PlayOneShot(_clickSound);

        _onSelected?.Invoke(_data);
    }
}
