using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform player;     
    public float mouseSensitivity = 3f;

    float xRotation = 0f;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 100 * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 100 * Time.deltaTime;

        // Pitch (lên / xuống)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -40f, 70f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Yaw (xoay player + camera root)
        player.Rotate(Vector3.up * mouseX);
    }
}
