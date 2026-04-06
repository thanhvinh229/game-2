using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static bool IsUIOpen { get; private set; }

    public static void SetUIOpen(bool isOpen)
    {
        IsUIOpen = isOpen;
    }
}
