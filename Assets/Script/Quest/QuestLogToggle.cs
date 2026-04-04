using UnityEngine;
using UnityEngine.InputSystem;

public class QuestLogToggle : MonoBehaviour
{
    [SerializeField] private GameObject _questLogPanel;
    [SerializeField] private Key _toggleKey = Key.Q;

    void Update()
    {
        if (Keyboard.current[_toggleKey].wasPressedThisFrame)
        {
            _questLogPanel.SetActive(!_questLogPanel.activeSelf);
        }
    }
}
