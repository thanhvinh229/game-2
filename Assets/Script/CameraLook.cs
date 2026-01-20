using UnityEngine;

public class CameraLook : MonoBehaviour
{
    public Transform yaw;
    public Transform pitch;

    public float sensitivity = 120f;
    public float minPitch = -35f;
    public float maxPitch = 70f;

    float xRotation;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        yaw.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minPitch, maxPitch);
        pitch.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
