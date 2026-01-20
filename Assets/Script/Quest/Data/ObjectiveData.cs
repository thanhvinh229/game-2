using UnityEngine;

[CreateAssetMenu(fileName = "ObjectiveData", menuName = "Scriptable Objects/ObjectiveData")]
public abstract class ObjectiveData : ScriptableObject
{
    public string Id;

    public string Description;

    public abstract Objective CreateInstance(ObjectiveData data);
}
