using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "Scriptable Objects/QuestData")]
public class QuestData : ScriptableObject
{
    public string Id;
    public string Description;
    public QuestStatus Status;
    public List<ObjectiveData> ObjectiveData = new();
}

