using UnityEngine;

public class QuestHolder : MonoBehaviour
{
    
    [SerializeField] private QuestData _questData;

    public void GiveQuest()
    {
        QuestManager.Instance.ReceivedQuest(_questData);
    }
}
