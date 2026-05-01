using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private Color textColor;
    
    [Header("Settings")]
    public float moveYSpeed = 2f;      // Tốc độ bay lên
    public float disappearTimer = 1f;  // Thời gian hiển thị trước khi mờ đi
    public float disappearSpeed = 3f;  // Tốc độ mờ dần

    private Transform mainCamera;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    private void Start()
    {
        
        if (Camera.main != null)
        {
            mainCamera = Camera.main.transform;
        }
    }

    
    public void Setup(float damageAmount, bool isPlayerHit)
    {
        // Hiển thị số sát thương 
        textMesh.text = Mathf.RoundToInt(damageAmount).ToString();
        
        // Phân loại màu sắc: Quái bị đánh màu Vàng/Trắng, Player bị đánh màu Đỏ
        textColor = isPlayerHit ? Color.red : Color.white;
        textMesh.color = textColor;
    }

    private void Update()
    {
        // 1. Cho chữ bay lên trên
        transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;

        // 2. Xử lý thời gian mờ dần và tự hủy
        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            
            // Xóa object khi đã trong suốt hoàn toàn
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void LateUpdate()
    {
        // 3. Ép UI luôn nhìn thẳng vào mặt Camera
        if (mainCamera != null)
        {
            transform.forward = mainCamera.forward;
        }
    }
}
