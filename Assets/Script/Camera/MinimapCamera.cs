using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Kéo GameObject Player vào đây")]
    public Transform player;
 
    [Header("Camera Settings")]
    [Tooltip("Độ cao camera so với player (trục Y)")]
    public float height = 80f;
 
    [Tooltip("Kích thước vùng nhìn (Orthographic Size). Tăng = thấy rộng hơn")]
    public float orthographicSize = 50f;
 
    [Tooltip("Minimap xoay theo hướng player (true) hay cố định hướng Bắc (false)")]
    public bool rotateWithPlayer = false;
 
    private Camera minimapCamComponent;
 
    void Awake()
    {
        minimapCamComponent = GetComponent<Camera>();
        minimapCamComponent.orthographic     = true;
        minimapCamComponent.orthographicSize = orthographicSize;
    }
 
    void LateUpdate()
    {
        if (player == null) return;
 
        transform.position = new Vector3(
            player.position.x,
            player.position.y + height,
            player.position.z
        );
 
        float yaw = rotateWithPlayer ? player.eulerAngles.y : 0f;
        transform.rotation = Quaternion.Euler(90f, yaw, 0f);
    }
}
