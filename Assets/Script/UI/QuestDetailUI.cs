using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestDetailUI : MonoBehaviour
{
    [Header("Header")]
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _statusText;
    [SerializeField] private Image _statusBadge;
    [SerializeField] private Color _activeStatusColor;
    [SerializeField] private Color _completedStatusColor;
 
    [Header("Description")]
    [SerializeField] private TMP_Text _descriptionText;
 
    [Header("Objectives")]
    [SerializeField] private Transform _objectiveContent;
    [SerializeField] private GameObject _objectiveItemPrefab; // prefab ObjectiveItem
 
    [Header("Rewards")]
    [SerializeField] private Sprite _goldIcon;
    [SerializeField] private Sprite _expIcon;
    [SerializeField] private Transform _itemRewardContent;
    [SerializeField] private GameObject _rewardItemPrefab;
 
    private Dictionary<string, ObjectiveItemUI> _objectiveItems = new();
 
    public void Show(QuestData questData)
    {
        _titleText.text = questData.Description;
 
        bool isCompleted = questData.Status == QuestStatus.Completed;
        _statusText.text = isCompleted ? "Hoàn thành" : "Đang hoạt động";
        _statusBadge.color = isCompleted ? _completedStatusColor : _activeStatusColor;
 
        _descriptionText.text = questData.Description;
 
        BuildObjectives(questData);
        BuildRewards(questData.Reward);
    }
 
    public void UpdateObjectiveProgress(string objectiveId, int current, int required)
    {
        if (_objectiveItems.TryGetValue(objectiveId, out var ui))
            ui.UpdateProgress(current, required);
    }
 
    private void BuildObjectives(QuestData questData)
    {
        foreach (Transform child in _objectiveContent)
            Destroy(child.gameObject);
        _objectiveItems.Clear();
 
        foreach (var objData in questData.ObjectiveData)
        {
            var go = Instantiate(_objectiveItemPrefab, _objectiveContent);
            var ui = go.GetComponent<ObjectiveItemUI>();
 
            int required = objData is CollectObjectiveData collectData
                ? collectData.RequiredAmount : 1;
 
            int current = objData.Status == QuestStatus.Completed ? required : 0;
            ui.Initialize(objData.Description, current, required);
            _objectiveItems.Add(objData.Id, ui);
        }
    }
 
    private void BuildRewards(QuestReward reward)
    {
        if (reward == null) return;
 
        // Xóa hết reward cũ
        foreach (Transform child in _itemRewardContent)
            Destroy(child.gameObject);
 
        // Spawn Gold
        if (reward.Gold > 0)
        {
            var go = Instantiate(_rewardItemPrefab, _itemRewardContent);
            go.GetComponent<RewardItemUI>().InitializeStat(_goldIcon, $"{reward.Gold}", "Gold");
        }
 
        // Spawn EXP
        if (reward.Exp > 0)
        {
            var go = Instantiate(_rewardItemPrefab, _itemRewardContent);
            go.GetComponent<RewardItemUI>().InitializeStat(_expIcon, $"{reward.Exp}", "EXP");
        }
 
        // Spawn items
        if (reward.Items == null) return;
        foreach (var item in reward.Items)
        {
            var go = Instantiate(_rewardItemPrefab, _itemRewardContent);
            go.GetComponent<RewardItemUI>().Initialize(item);
        }
    }
}
