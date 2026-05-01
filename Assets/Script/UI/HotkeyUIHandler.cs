using UnityEngine;

public class HotkeyUIHandler : MonoBehaviour
{
    public GameObject hotkeyPanel;

    public void OpenPanel()
    {
        hotkeyPanel.SetActive(true);
        
        // Dừng thời gian toàn bộ game (quái ngừng di chuyển, player không đánh được)
        Time.timeScale = 0f; 
        
        // Hiện chuột để người chơi tương tác với bảng
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ClosePanel()
    {
        hotkeyPanel.SetActive(false);
        
        // Trả thời gian về bình thường để tiếp tục chơi
        Time.timeScale = 1f;
        
        // Khóa lại chuột để điều khiển nhân vật
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
