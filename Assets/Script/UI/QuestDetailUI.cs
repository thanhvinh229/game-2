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
    [SerializeField] private Color _activeStatusColor = Color.green;
    [SerializeField] private Color _completedStatusColor = Color.blue;
 
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
        if (questData == null)
        {
            Debug.LogError("QuestData is null!");
            return;
        }
 
        _titleText.text = questData.Id;
 
        bool isCompleted = questData.Status == QuestStatus.Completed;
        _statusText.text = isCompleted ? "Hoàn thành" : "Đang hoạt động";
        _statusBadge.color = isCompleted ? _completedStatusColor : _activeStatusColor;
 
        _descriptionText.text = questData.Description;
 
        BuildObjectives(questData);
        
        // Gọi BuildRewards nếu Reward tồn tại
        if (questData.Reward != null)
        {
            BuildRewards(questData.Reward);
        }
    }
 
    public void UpdateObjectiveProgress(string objectiveId, int current, int required)
    {
        if (_objectiveItems.TryGetValue(objectiveId, out var ui))
        {
            ui.UpdateProgress(current, required);
        }
    }
 
    private void BuildObjectives(QuestData questData)
    {
        // Clear old objectives
        foreach (Transform child in _objectiveContent)
        {
            Destroy(child.gameObject);
        }
        _objectiveItems.Clear();
 
        if (questData.ObjectiveData == null || questData.ObjectiveData.Count == 0)
        {
            Debug.LogWarning("No objectives found!");
            return;
        }
 
        foreach (var objData in questData.ObjectiveData)
        {
            var go = Instantiate(_objectiveItemPrefab, _objectiveContent);
            var ui = go.GetComponent<ObjectiveItemUI>();
            
            if (ui == null)
            {
                Debug.LogError("ObjectiveItemUI component not found!");
                continue;
            }
 
            int required = (objData is CollectObjectiveData collectData) ? collectData.RequiredAmount : 1;
            int current = (objData.Status == QuestStatus.Completed) ? required : 0;
            
            ui.Initialize(objData.Description, current, required);
            _objectiveItems.Add(objData.Id, ui);
        }
    }
 
    private void BuildRewards(QuestReward reward)
    {
        if (reward == null)
        {
            Debug.LogWarning("Reward is null!");
            return;
        }
 
        // Clear old rewards
        foreach (Transform child in _itemRewardContent)
        {
            Destroy(child.gameObject);
        }
 
        // Spawn Gold reward
        if (reward.Gold > 0)
        {
            if (_goldIcon == null)
            {
                Debug.LogError("Gold Icon not assigned in Inspector!");
            }
            else
            {
                SpawnRewardItem(_goldIcon, $"{reward.Gold}", "Gold");
            }
        }
 
        // Spawn EXP reward
        if (reward.Exp > 0)
        {
            if (_expIcon == null)
            {
                Debug.LogError("Exp Icon not assigned in Inspector!");
            }
            else
            {
                SpawnRewardItem(_expIcon, $"{reward.Exp}", "EXP");
            }
        }
    }
 
    private void SpawnRewardItem(Sprite icon, string amount, string label)
    {
        if (_rewardItemPrefab == null)
        {
            Debug.LogError("RewardItemPrefab not assigned!");
            return;
        }
 
        var go = Instantiate(_rewardItemPrefab, _itemRewardContent);
        var rewardUI = go.GetComponent<RewardItemUI>();
 
        if (rewardUI == null)
        {
            Debug.LogError("RewardItemUI component not found on prefab!");
            return;
        }
 
        rewardUI.InitializeStat(icon, amount, label);
    }
}
 