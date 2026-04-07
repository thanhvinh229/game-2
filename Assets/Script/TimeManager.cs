using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public enum TimeOfDay { Morning, Afternoon, Night }

    [Header("Time Settings")]
    [Tooltip("Số phút đời thực để trôi qua 1 ngày trong game (ví dụ: 10)")]
    public float dayLengthInMinutes = 10f;
    [Range(0, 24)]
    public float currentTimeInHours = 8f; 
    public float transitionSpeed = 1.0f;

    [Header("References")]
    public Light directionalLight;

    [Header("Morning Settings (5h - 12h)")]
    public Color morningLight = new Color(1f, 0.95f, 0.8f);
    public Color morningSkyTint = new Color(0.5f, 0.5f, 0.5f);
    public float morningIntensity = 1.2f;
    public float morningExposure = 1.0f;
    public Vector3 morningRot = new Vector3(45, -30, 0);

    [Header("Afternoon Settings (12h - 18h)")]
    public Color afternoonLight = new Color(1f, 0.5f, 0.2f);
    public Color afternoonSkyTint = new Color(0.8f, 0.5f, 0.4f);
    public float afternoonIntensity = 1.6f;
    public float afternoonExposure = 1.2f;
    public Vector3 afternoonRot = new Vector3(25, -30, 0);

    [Header("Night Settings (18h - 5h)")]
    public Color nightLight = new Color(0.3f, 0.4f, 0.8f);
    public Color nightSkyTint = new Color(0.05f, 0.1f, 0.2f);
    public float nightIntensity = 1.1f;
    public float nightExposure = 0.1f; // Thấp để mất mây trắng
    public Vector3 nightRot = new Vector3(50, -150, 0);

    private Color targetL_Color, targetS_Tint;
    private float targetIntensity, targetExposure;
    private Quaternion targetRot;

    void Update()
    {
        // 1. Logic trôi thời gian
        float timeMultiplier = 24f / (dayLengthInMinutes * 60f);
        currentTimeInHours += Time.deltaTime * timeMultiplier;
        if (currentTimeInHours >= 24f) currentTimeInHours = 0f;

        // 2. Xác định trạng thái dựa trên giờ
        UpdateTargetSettings();

        // 3. Thực hiện Lerp mượt mà
        ApplyTransitions();
    }

    void UpdateTargetSettings()
    {
        if (currentTimeInHours >= 5f && currentTimeInHours < 12f) {
            SetTargets(morningLight, morningSkyTint, morningIntensity, morningExposure, morningRot);
        } else if (currentTimeInHours >= 12f && currentTimeInHours < 18f) {
            SetTargets(afternoonLight, afternoonSkyTint, afternoonIntensity, afternoonExposure, afternoonRot);
        } else {
            SetTargets(nightLight, nightSkyTint, nightIntensity, nightExposure, nightRot);
        }
    }

    void SetTargets(Color lColor, Color sTint, float intens, float expos, Vector3 rot)
    {
        targetL_Color = lColor; targetS_Tint = sTint;
        targetIntensity = intens; targetExposure = expos;
        targetRot = Quaternion.Euler(rot);
    }

    void ApplyTransitions()
    {
        directionalLight.color = Color.Lerp(directionalLight.color, targetL_Color, Time.deltaTime * transitionSpeed);
        directionalLight.intensity = Mathf.Lerp(directionalLight.intensity, targetIntensity, Time.deltaTime * transitionSpeed);
        directionalLight.transform.rotation = Quaternion.Slerp(directionalLight.transform.rotation, targetRot, Time.deltaTime * transitionSpeed);

        RenderSettings.skybox.SetColor("_Tint", Color.Lerp(RenderSettings.skybox.GetColor("_Tint"), targetS_Tint, Time.deltaTime * transitionSpeed));
        RenderSettings.skybox.SetFloat("_Exposure", Mathf.Lerp(RenderSettings.skybox.GetFloat("_Exposure"), targetExposure, Time.deltaTime * transitionSpeed));
        
        // Cập nhật Fog theo màu đèn để ban đêm không bị lộ chân trời
        RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, targetL_Color * 0.4f, Time.deltaTime * transitionSpeed);
        RenderSettings.ambientLight = directionalLight.color * 0.4f;
    }
}
