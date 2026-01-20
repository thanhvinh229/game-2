using UnityEngine;

[CreateAssetMenu(fileName = "Objective", menuName = "Scriptable Objects/Objective")]
public class Objective : ScriptableObject
{
    public ObjectiveData Data;

    public bool IsComplete {  get;  set; }

    public Objective(ObjectiveData data)
    {
        Data = data; 
    }
}
