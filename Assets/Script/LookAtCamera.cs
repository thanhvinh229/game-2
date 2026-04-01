using UnityEngine;


public class LookAtCamera : MonoBehaviour
{
    private Transform cam;

    void Start() => cam = Camera.main.transform;

    void LateUpdate()
    {
        // Làm thanh máu luôn quay mặt về phía camera người chơi
        transform.LookAt(transform.position + cam.forward);
    }
}
