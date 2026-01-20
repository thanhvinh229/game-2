using UnityEngine;

[CreateAssetMenu(fileName = "CollectObjectiveData", menuName = "Scriptable Objects/CollectObjectiveData")]
public  class CollectObjectiveData : ObjectiveData
{

    public override Objective CreateInstance(ObjectiveData data)
    {
        return new CollectObjective(data);  
    }
}
