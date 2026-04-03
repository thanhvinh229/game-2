using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class QuestItemUI : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private TMP_Text _nameText;        // "Text (TMP)" — đã gán sẵn
    [SerializeField] private Toggle _statusToggle;      // "Toggle" — đã gán sẵn
 
    [Header("Objectives")]
    [SerializeField] private RectTransform _objectiveContent; // child mới "ObjectiveContent"
    [SerializeField] private GameObject _objectiveItemPrefab; // drag prefab "QuestItem" vào
 
    private Dictionary<string, ObjectiveItemUI> _objectiveItems = new();
 
    public void Initialize(QuestData questData)
    {
        _nameText.text = questData.Description;
        _statusToggle.isOn = false;
        _statusToggle.interactable = false;
 
        foreach (var objData in questData.ObjectiveData)
        {
            var go = Instantiate(_objectiveItemPrefab, _objectiveContent);
            var objectiveUI = go.GetComponent<ObjectiveItemUI>();
 
            int required = objData is CollectObjectiveData collectData
                ? collectData.RequiredAmount : 1;
 
            objectiveUI.Initialize(objData.Description, current: 0, required);
            _objectiveItems.Add(objData.Id, objectiveUI);
        }
    }
 
    public void UpdateObjectiveProgress(string objectiveId, int current, int required)
    {
        if (_objectiveItems.TryGetValue(objectiveId, out var objectiveUI))
            objectiveUI.UpdateProgress(current, required);
    }
 
    public void UpdateStatus(bool isCompleted)
    {
        _statusToggle.isOn = isCompleted;
    }
}
