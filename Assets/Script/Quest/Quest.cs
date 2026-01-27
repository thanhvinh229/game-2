using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour
{
   public QuestData Data;

    public List<Objective> Objectives = new();

    public bool IsComplete;

    public Quest(QuestData data)
    {
        Data = data;
    }

    internal void Complete()
    {
        throw new NotImplementedException();
    }

    internal void Start()
    {
        Debug.Log("start quest");
    }
}
