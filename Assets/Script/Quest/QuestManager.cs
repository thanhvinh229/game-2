using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private static QuestManager _instance;

    public static QuestManager Instance => _instance;

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private Quest _quest;
    public Quest CurrentQuest => _quest;

    public void StartQuest( QuestData questData)
    {
        _quest = new Quest(questData);
        _quest.Start();
    }

}
