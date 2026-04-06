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
    [SerializeField] private Color _activeColor = new Color(0.2f, 0.6f, 1f);        // xanh dương
    [SerializeField] private Color _completedColor = new Color(0.2f, 0.8f, 0.3f);   // xanh lá
    [SerializeField] private Color _selectedBgColor = new Color(1f, 1f, 1f, 0.1f);  // highlight khi chọn
 
    private Image _bgImage;
    private Button _button;
    private QuestData _data;
    private System.Action<QuestData> _onSelected;
 
    void Awake()
    {
        // Get components
        _bgImage = GetComponent<Image>();
        _button = GetComponent<Button>();
        
        // Validation
        if (_button == null)
        {
            Debug.LogError($"QuestEntryUI: Missing Button component on {gameObject.name}!");
            return;
        }
 
        if (_nameText == null)
        {
            Debug.LogWarning($"QuestEntryUI: _nameText not assigned on {gameObject.name}");
        }
 
        // Setup click listener
        _button.onClick.AddListener(OnClick);
        
        Debug.Log($"QuestEntryUI: Initialized on {gameObject.name}, Button interactable: {_button.interactable}");
    }
 
    /// <summary>
    /// Khởi tạo quest entry với data và callback
    /// </summary>
    public void Initialize(QuestData data, System.Action<QuestData> onSelected)
    {
        if (data == null)
        {
            Debug.LogError("QuestEntryUI: Cannot initialize with null QuestData!");
            return;
        }
 
        _data = data;
        _onSelected = onSelected;
        
        if (_nameText != null)
        {
            _nameText.text = data.Description;
        }
        
        UpdateDotColor();
        
        Debug.Log($"QuestEntryUI: Initialized quest '{data.Description}' (ID: {data.Id})");
    }
 
    /// <summary>
    /// Highlight/unhighlight quest entry khi được chọn
    /// </summary>
    public void SetSelected(bool selected)
    {
        if (_bgImage != null)
        {
            _bgImage.color = selected ? _selectedBgColor : Color.clear;
        }
        
        Debug.Log($"QuestEntryUI: Quest '{_data?.Description}' selected = {selected}");
    }
 
    /// <summary>
    /// Refresh trạng thái quest (Active/Completed)
    /// </summary>
    public void RefreshStatus()
    {
        UpdateDotColor();
    }
 
    /// <summary>
    /// Update màu dot dựa trên trạng thái quest
    /// </summary>
    private void UpdateDotColor()
    {
        if (_dotImage == null || _data == null) return;
        
        _dotImage.color = _data.Status == QuestStatus.Completed
            ? _completedColor
            : _activeColor;
    }
 
    /// <summary>
    /// Handler khi quest entry được click
    /// </summary>
    private void OnClick()
    {
        if (_data == null)
        {
            Debug.LogError("QuestEntryUI: Cannot click - _data is null!");
            return;
        }
 
        Debug.Log($"🔵 QuestEntryUI: CLICKED quest '{_data.Description}' (ID: {_data.Id})");
        
        _onSelected?.Invoke(_data);
    }
 
    #if UNITY_EDITOR
    /// <summary>
    /// Validation trong Editor
    /// </summary>
    void OnValidate()
    {
        // Auto-find components nếu chưa gán
        if (_nameText == null)
        {
            _nameText = GetComponentInChildren<TMP_Text>();
        }
 
        if (_dotImage == null)
        {
            var images = GetComponentsInChildren<Image>();
            foreach (var img in images)
            {
                if (img.gameObject != gameObject && img.name.ToLower().Contains("dot"))
                {
                    _dotImage = img;
                    break;
                }
            }
        }
    }
    #endif
}
