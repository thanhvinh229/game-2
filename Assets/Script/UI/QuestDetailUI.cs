using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestDetailUI : MonoBehaviour
{
    [Header("Header")]
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _statusText;
    [SerializeField] private Image    _statusBadge;
    [SerializeField] private Color    _activeStatusColor;
    [SerializeField] private Color    _completedStatusColor;
 
    [Header("Description")]
    [SerializeField] private TMP_Text _descriptionText;
 
    [Header("Objectives")]
    [SerializeField] private Transform  _objectiveContent;
    [SerializeField] private GameObject _objectiveItemPrefab;
 
    [Header("Rewards")]
    [SerializeField] private Sprite     _goldIcon;
    [SerializeField] private Sprite     _expIcon;
    [SerializeField] private Transform  _itemRewardContent;
    [SerializeField] private GameObject _rewardItemPrefab;
 
    private Dictionary<string, ObjectiveItemUI> _objectiveItems = new();
 
    public void Show(QuestData questData, Quest questRuntime = null)
    {
        _titleText.text       = questData.Description;
        bool isCompleted      = questData.Status == QuestStatus.Completed;
        _statusText.text      = isCompleted ? "Hoàn thành" : "Đang hoạt động";
        _statusBadge.color    = isCompleted ? _completedStatusColor : _activeStatusColor;
        _descriptionText.text = questData.Description;
 
        BuildObjectives(questData, questRuntime);
        BuildRewards(questData.Reward);
    }
 
    public void UpdateObjectiveProgress(string objectiveId, int current, int required)
    {
        if (_objectiveItems.TryGetValue(objectiveId, out var ui))
            ui.UpdateProgress(current, required);
    }
 
    private void BuildObjectives(QuestData questData, Quest questRuntime)
    {
        foreach (Transform child in _objectiveContent)
            Destroy(child.gameObject);
        _objectiveItems.Clear();
 
        foreach (var objData in questData.ObjectiveData)
        {
            var go = Instantiate(_objectiveItemPrefab, _objectiveContent);
            var ui = go.GetComponent<ObjectiveItemUI>();
 
            int required = GetRequiredAmount(objData);
            int current  = GetCurrentProgress(objData, questRuntime, required);
 
            ui.Initialize(objData.Id, current, required);
            _objectiveItems.Add(objData.Id, ui);
        }
    }
 
    // Đọc tiến độ thật từ runtime Objective, khớp theo Id
    private int GetCurrentProgress(ObjectiveData objData, Quest questRuntime, int required)
    {
        if (objData.Status == QuestStatus.Completed)
            return required;
 
        if (questRuntime == null || questRuntime.Objectives == null)
            return 0;
 
        foreach (var obj in questRuntime.Objectives)
        {
            // So sánh bằng Id (giờ Objective đã expose public Id)
            if (obj.Id != objData.Id) continue;
 
            if (obj is KillObjective killObj)
                return killObj.CurrentKills;
 
            // Thêm CollectObjective, v.v. nếu cần:
            // if (obj is CollectObjective collectObj)
            //     return collectObj.CurrentAmount;
 
            // Objective đã hoàn thành nhưng chưa handle riêng
            if (obj.IsCompleted) return required;
        }
 
        return 0;
    }
 
    private int GetRequiredAmount(ObjectiveData objData)
    {
        if (objData is CollectObjectiveData collectData) return collectData.RequiredAmount;
        if (objData is KillObjectiveData killData)       return killData.requiredKills;
        return 1;
    }
 
    private void BuildRewards(QuestReward reward)
    {
        foreach (Transform child in _itemRewardContent)
            Destroy(child.gameObject);
 
        if (reward == null) { Debug.LogWarning("QuestDetailUI: No reward data!"); return; }
 
        if (reward.Gold > 0)
        {
            var go = Instantiate(_rewardItemPrefab, _itemRewardContent);
            go.GetComponent<RewardItemUI>().InitializeStat(_goldIcon, reward.Gold.ToString(), "Gold");
        }
 
        if (reward.Exp > 0)
        {
            var go = Instantiate(_rewardItemPrefab, _itemRewardContent);
            go.GetComponent<RewardItemUI>().InitializeStat(_expIcon, reward.Exp.ToString(), "EXP");
        }
 
        if (reward.Items != null)
            foreach (var item in reward.Items)
            {
                var go = Instantiate(_rewardItemPrefab, _itemRewardContent);
                go.GetComponent<RewardItemUI>().Initialize(item);
            }
    }
}
 
 