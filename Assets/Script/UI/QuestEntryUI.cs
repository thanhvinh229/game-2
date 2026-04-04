using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Prefab: QuestItem có sẵn (Toggle + Text TMP)
// Dùng cho danh sách quest bên trái
public class QuestEntryUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Image _dotImage;           // dot chấm tròn nhỏ bên trái

    [Header("Colors")]
    [SerializeField] private Color _activeColor;        // xanh dương
    [SerializeField] private Color _completedColor;     // xanh lá
    [SerializeField] private Color _selectedBgColor;    // highlight khi chọn

    private Image _bgImage;
    private QuestData _data;
    private System.Action<QuestData> _onSelected;

    void Awake()
    {
        _bgImage = GetComponent<Image>();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void Initialize(QuestData data, System.Action<QuestData> onSelected)
    {
        _data = data;
        _onSelected = onSelected;
        _nameText.text = data.Description;
        UpdateDotColor();
    }

    public void SetSelected(bool selected)
    {
        if (_bgImage != null)
            _bgImage.color = selected ? _selectedBgColor : Color.clear;
    }

    public void RefreshStatus()
    {
        UpdateDotColor();
    }

    private void UpdateDotColor()
    {
        if (_dotImage == null) return;
        _dotImage.color = _data.Status == QuestStatus.Completed
            ? _completedColor
            : _activeColor;
    }

    private void OnClick()
    {
        _onSelected?.Invoke(_data);
    }
}
