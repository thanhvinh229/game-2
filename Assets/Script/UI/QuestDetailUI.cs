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
    [SerializeField] private GameObject _objectiveItemPrefab;
 
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
 
            // Get required amount based on objective type
            int required = GetRequiredAmount(objData);
            int current = objData.Status == QuestStatus.Completed ? required : 0;
            
            ui.Initialize(objData.Id, current, required);
            _objectiveItems.Add(objData.Id, ui);
        }
    }
 
    /// <summary>
    /// Get required amount cho mỗi loại objective
    /// </summary>
    private int GetRequiredAmount(ObjectiveData objData)
    {
        if (objData is CollectObjectiveData collectData)
        {
            return collectData.RequiredAmount;
        }
        else if (objData is KillObjectiveData killData)
        {
            return killData.requiredKills;
        }
        else
        {
            return 1; // Default cho các objective types khác
        }
    }
 
    private void BuildRewards(QuestReward reward)
    {
        // Clear old rewards
        foreach (Transform child in _itemRewardContent)
            Destroy(child.gameObject);
 
        if (reward == null)
        {
            Debug.LogWarning("QuestDetailUI: No reward data!");
            return;
        }
 
        // Spawn Gold reward
        if (reward.Gold > 0)
        {
            var go = Instantiate(_rewardItemPrefab, _itemRewardContent);
            var ui = go.GetComponent<RewardItemUI>();
            ui.InitializeStat(_goldIcon, reward.Gold.ToString(), "Gold");
        }
 
        // Spawn EXP reward
        if (reward.Exp > 0)
        {
            var go = Instantiate(_rewardItemPrefab, _itemRewardContent);
            var ui = go.GetComponent<RewardItemUI>();
            ui.InitializeStat(_expIcon, reward.Exp.ToString(), "EXP");
        }
 
        // Spawn item rewards
        if (reward.Items != null && reward.Items.Length > 0)
        {
            foreach (var item in reward.Items)
            {
                var go = Instantiate(_rewardItemPrefab, _itemRewardContent);
                var ui = go.GetComponent<RewardItemUI>();
                ui.Initialize(item);
            }
        }
    }
}
 
 