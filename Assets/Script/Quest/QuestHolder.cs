using UnityEngine;

public class QuestHolder : MonoBehaviour
{
    [SerializeField] private QuestEventChannel _questEventChannel;
    [SerializeField] private QuestData _questData;

    public void GiveQuest()
    {
        _questEventChannel.OnReceivedQuest?.Invoke(_questData);
    }
}
