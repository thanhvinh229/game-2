using UnityEngine;

public class FloatingItem : MonoBehaviour
{
    [Header("Cài đặt Nhấp nhô (Floating)")]
    public float floatSpeed = 2f;       // Tốc độ nhấp nhô (cao hơn = nhanh hơn)
    public float floatAmplitude = 0.15f; // Độ cao nhấp nhô (mét)

    private Vector3 _startPos;

    void Start()
    {
        // Lưu vị trí ban đầu làm mốc
        _startPos = transform.localPosition;
    }

    void Update()
    {
        // Tính toán độ cao mới bằng hàm Sin để tạo chuyển động lên xuống mượt
        // Time.time giúp giá trị thay đổi liên tục theo thời gian
        float newY = _startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        
        // Cập nhật vị trí mới, giữ nguyên X và Z
        transform.localPosition = new Vector3(_startPos.x, newY, _startPos.z);
    }
}
