using UnityEngine;

[CreateAssetMenu(fileName = "CollectObjectiveData", menuName = "Scriptable Objects/CollectObjectiveData")]
public class CollectObjectiveData : ObjectiveData
{
    public string TargetId;
    public int RequiredAmount;

    public override Objective CreateInstance()
    {
        return new CollectObjective(this);
    }


}
    