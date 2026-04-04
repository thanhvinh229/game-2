using UnityEngine;
using UnityEngine.UI;
using TMPro;
 
public class QuestLogLayoutFixer : MonoBehaviour
{
    [Header("Right Panel Children")]
    [SerializeField] private RectTransform _header;
    [SerializeField] private RectTransform _titleText;
    [SerializeField] private RectTransform _statusBadge;
    [SerializeField] private RectTransform _descriptionText;
    [SerializeField] private RectTransform _objectiveContent;
    [SerializeField] private RectTransform _rewardSection;
    [SerializeField] private RectTransform _statRewards;
    [SerializeField] private RectTransform _itemRewardContent;
 
    [Header("Left Panel")]
    [SerializeField] private RectTransform _leftPanel;
    [SerializeField] private RectTransform _rightPanel;
    [SerializeField] private RectTransform _questListContent;
 
    [ContextMenu("Fix All Layout")]
    public void FixAll()
    {
        FixPanels();
        FixHeader();
        FixDescriptionText();
        FixObjectiveContent();
        FixRewardSection();
        Debug.Log("Done! Press Play to test.");
    }
 
    void FixPanels()
    {
        // Reset anchors
        ResetAnchor(_leftPanel);
        ResetAnchor(_rightPanel);
        ResetAnchor(_questListContent);
 
        // Left panel
        SetLayoutElement(_leftPanel.gameObject, flexW: 3, flexH: 1);
        var leftVlg = GetOrAdd<VerticalLayoutGroup>(_leftPanel.gameObject);
        leftVlg.childAlignment = TextAnchor.UpperLeft;
        leftVlg.childControlWidth = true;
        leftVlg.childControlHeight = false;
        leftVlg.childForceExpandWidth = true;
        leftVlg.childForceExpandHeight = false;
        leftVlg.padding = new RectOffset(4, 4, 4, 4);
        leftVlg.spacing = 0;
 
        SetLayoutElement(_questListContent.gameObject, flexW: 1, flexH: 1);
        var qlcVlg = GetOrAdd<VerticalLayoutGroup>(_questListContent.gameObject);
        qlcVlg.childAlignment = TextAnchor.UpperLeft;
        qlcVlg.childControlWidth = true;
        qlcVlg.childControlHeight = false;
        qlcVlg.childForceExpandWidth = true;
        qlcVlg.childForceExpandHeight = false;
        qlcVlg.spacing = 2;
 
        // Right panel
        SetLayoutElement(_rightPanel.gameObject, flexW: 7, flexH: 1);
        var rightVlg = GetOrAdd<VerticalLayoutGroup>(_rightPanel.gameObject);
        rightVlg.childAlignment = TextAnchor.UpperLeft;
        rightVlg.childControlWidth = true;
        rightVlg.childControlHeight = false;
        rightVlg.childForceExpandWidth = true;
        rightVlg.childForceExpandHeight = false;
        rightVlg.padding = new RectOffset(16, 16, 16, 16);
        rightVlg.spacing = 10;
    }
 
    void FixHeader()
    {
        if (_header == null) return;
        SetLayoutElement(_header.gameObject, prefH: 36);
        var hlg = GetOrAdd<HorizontalLayoutGroup>(_header.gameObject);
        hlg.childAlignment = TextAnchor.MiddleLeft;
        hlg.childControlWidth = false;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = true;
        hlg.spacing = 8;
 
        // TitleText flexible
        if (_titleText != null)
            SetLayoutElement(_titleText.gameObject, flexW: 1);
 
        // StatusBadge fixed size
        if (_statusBadge != null)
            SetLayoutElement(_statusBadge.gameObject, prefW: 110, prefH: 24, flexW: 0, flexH: 0);
    }
 
    void FixDescriptionText()
    {
        if (_descriptionText == null) return;
        SetLayoutElement(_descriptionText.gameObject, prefH: 50, flexW: 1);
        var tmp = _descriptionText.GetComponent<TMP_Text>();
        if (tmp != null)
        {
            tmp.textWrappingMode = TextWrappingModes.Normal;
            tmp.overflowMode = TextOverflowModes.Overflow;
        }
    }
 
    void FixObjectiveContent()
    {
        if (_objectiveContent == null) return;
        SetLayoutElement(_objectiveContent.gameObject, flexW: 1, flexH: 0, prefH: 0);
        var vlg = GetOrAdd<VerticalLayoutGroup>(_objectiveContent.gameObject);
        vlg.childAlignment = TextAnchor.UpperLeft;
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.spacing = 4;
 
        // Add ContentSizeFitter so it grows with content
        var csf = GetOrAdd<ContentSizeFitter>(_objectiveContent.gameObject);
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
    }
 
    void FixRewardSection()
    {
        if (_rewardSection == null) return;
        SetLayoutElement(_rewardSection.gameObject, flexW: 1, prefH: 0);
        var vlg = GetOrAdd<VerticalLayoutGroup>(_rewardSection.gameObject);
        vlg.childAlignment = TextAnchor.UpperLeft;
        vlg.childControlWidth = false;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = false;
        vlg.childForceExpandHeight = false;
        vlg.spacing = 6;
 
        var csf = GetOrAdd<ContentSizeFitter>(_rewardSection.gameObject);
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
 
        // StatRewards
        if (_statRewards != null)
        {
            SetLayoutElement(_statRewards.gameObject, prefH: 36);
            var hlg = GetOrAdd<HorizontalLayoutGroup>(_statRewards.gameObject);
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.childControlWidth = false;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = true;
            hlg.spacing = 8;
        }
 
        // ItemRewardContent
        if (_itemRewardContent != null)
        {
            SetLayoutElement(_itemRewardContent.gameObject, prefH: 60);
            var itemHlg = GetOrAdd<HorizontalLayoutGroup>(_itemRewardContent.gameObject);
            itemHlg.childAlignment = TextAnchor.MiddleLeft;
            itemHlg.childControlWidth = false;
            itemHlg.childControlHeight = false;
            itemHlg.childForceExpandWidth = false;
            itemHlg.childForceExpandHeight = false;
            itemHlg.spacing = 8;
 
            var itemCsf = GetOrAdd<ContentSizeFitter>(_itemRewardContent.gameObject);
            itemCsf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            itemCsf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }
 
    void ResetAnchor(RectTransform rt)
    {
        if (rt == null) return;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.pivot = new Vector2(0.5f, 0.5f);
    }
 
    void SetLayoutElement(GameObject go, float prefW = -1, float prefH = -1,
        float flexW = -1, float flexH = -1)
    {
        var le = GetOrAdd<LayoutElement>(go);
        if (prefW >= 0) { le.preferredWidth = prefW; le.minWidth = -1; }
        if (prefH >= 0) { le.preferredHeight = prefH; le.minHeight = -1; }
        if (flexW >= 0) le.flexibleWidth = flexW;
        if (flexH >= 0) le.flexibleHeight = flexH;
    }
 
    T GetOrAdd<T>(GameObject go) where T : Component
    {
        var c = go.GetComponent<T>();
        if (c == null) c = go.AddComponent<T>();
        return c;
    }
}
