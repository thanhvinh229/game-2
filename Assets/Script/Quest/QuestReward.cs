using System;
using UnityEngine;

[Serializable]
public class ItemReward
{
    public Sprite Icon;
    public string ItemName;
    public int Quantity;
}

[Serializable]
public class QuestReward
{
    public int Gold;
    public int Exp;
    public ItemReward[] Items;
}
