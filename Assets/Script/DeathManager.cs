using UnityEngine;
using UnityEngine.UI;
public class DeathManager : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject deathScreenPanel; // Kéo DeathScreenPanel vào đây

    [Header("Player Settings")]
    public GameObject player;           // Kéo Player vào đây
    public Transform respawnPoint;      // Kéo RespawnPoint ở trong hàng rào vào đây
    
    private Animator playerAnim;
    private CharacterController cc;     // Dùng nếu Player của bạn di chuyển bằng CharacterController

    void Start()
    {
        // Đảm bảo màn hình chết luôn tắt khi bắt đầu
        if (deathScreenPanel != null) deathScreenPanel.SetActive(false);
        
        if (player != null)
        {
            playerAnim = player.GetComponent<Animator>();
            cc = player.GetComponent<CharacterController>();
        }
    }

    // Hàm này sẽ được gọi khi máu Player <= 0
    public void TriggerDeath()
    {
        // 1. Chạy Animation gục ngã
        playerAnim.SetTrigger("Die");
        playerAnim.SetBool("IsDead", true);
        playerAnim.SetLayerWeight(1, 0f);

        // 2. Tắt script điều khiển để Player không trượt đi trượt lại khi đang nằm
        // (Giả sử script điều khiển của bạn tên là PlayerController)
        player.GetComponent<PlayerController>().enabled = false;

        // 3. Hiện màn hình xám và nút bấm
        deathScreenPanel.SetActive(true);

        // Mở khóa con trỏ chuột để bấm nút (nếu game bạn đang khóa chuột ở giữa màn hình)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    
    public void RevivePlayer()
    {
       deathScreenPanel.SetActive(false);

      // 1. Reset Stats (Máu, Mana, biến isDead trong code)
    PlayerStats stats = player.GetComponent<PlayerStats>();
    if (stats != null) stats.ResetStats(); 

    // 2. Xử lý vị trí và Vật lý (SyncTransforms)
    CharacterController cc = player.GetComponent<CharacterController>();
    if (cc != null) cc.enabled = false; 

    player.transform.position = respawnPoint.position;
    player.transform.rotation = respawnPoint.rotation; // Đảm bảo xoay đúng hướng hàng rào

    // LỆNH QUAN TRỌNG: Cập nhật vật lý ngay lập tức
    Physics.SyncTransforms(); 

    if (cc != null) cc.enabled = true;

    // 3. FIX LỖI "NẰM LÌ": Ép Animator đứng dậy
    Animator anim = player.GetComponent<Animator>();
    if (anim != null)
    {
        // Reset tất cả tham số liên quan đến cái chết
        anim.SetBool("IsDead", false);
        anim.ResetTrigger("Die"); // Xóa trigger Die nếu nó còn đang chờ

        // Ép Animator chuyển ngay lập tức về trạng thái Idle hoặc Locomotion
        // Thay "Locomotion" bằng tên trạng thái di chuyển trong Animator của bạn
        anim.Play("Locomotion", 0, 0f); 

        // Nếu bạn có Combat Layer (Layer 1), hãy trả lại quyền điều khiển cho nó
        anim.SetLayerWeight(1, 1f); 
    }

    // 4. Kích hoạt lại điều khiển
    player.GetComponent<PlayerController>().enabled = true;
    
    // Mở lại chuột để chơi tiếp        
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
    
 }

    // Hàm này gán vào nút Quit
    public void QuitGame()
    {
        Debug.Log("Đang thoát game...");
        Application.Quit(); // Thoát game khi đã build ra file .exe

        // Dòng này giúp dừng test khi bạn đang ấn Play trong Unity Editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
