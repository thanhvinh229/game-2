using UnityEngine;

[CreateAssetMenu(fileName = "ObjectiveData", menuName = "Scriptable Objects/ObjectiveData")]
public abstract class ObjectiveData : ScriptableObject
{
    public string Id;
    public string Description;
    public QuestStatus Status;
    public QuestEventChannel EventChannel;
 
    // QuestId được set lúc runtime bởi Quest khi khởi tạo
    [HideInInspector] public string QuestId;
 
    public abstract Objective CreateInstance();
}
