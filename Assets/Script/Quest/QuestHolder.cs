using UnityEngine;

public class QuestHolder : MonoBehaviour
{
    public QuestData Data;

    public void GiveQuest()
    {
        QuestManager.Instance.StartQuest(Data);
    }
}
