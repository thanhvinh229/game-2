    using UnityEngine;
using UnityEngine.UI;
public class MinimapUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("RawImage dùng để hiển thị minimap")]
    public RawImage minimapDisplay;
 
    [Tooltip("Script MinimapCamera trên Camera minimap")]
    public MinimapCamera minimapCam;
 
    [Header("RenderTexture")]
    [Tooltip("Kích thước texture render (256 hoặc 512)")]
    public int textureSize = 256;
 
    [Header("Zoom")]
    public float minZoom   = 20f;
    public float maxZoom   = 120f;
    public float zoomSpeed = 5f;
 
    [Header("Toggle")]
    public KeyCode toggleKey = KeyCode.M;
 
    // ── Internal ──────────────────────────────────────────────────────────
    private RenderTexture renderTexture;
    private Camera        minimapCamera;
    private bool          isVisible = true;
 
    void Start()
    {
        minimapCamera = minimapCam.GetComponent<Camera>();
 
        renderTexture              = new RenderTexture(textureSize, textureSize, 16);
        renderTexture.name         = "MinimapRT";
        minimapCamera.targetTexture = renderTexture;
        minimapDisplay.texture     = renderTexture;
    }
 
    void Update()
    {
        HandleZoom();
        HandleToggle();
    }
 
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Approximately(scroll, 0f)) return;
 
        RectTransform rt = minimapDisplay.rectTransform;
        if (!RectTransformUtility.RectangleContainsScreenPoint(rt, Input.mousePosition)) return;
 
        minimapCam.orthographicSize = Mathf.Clamp(
            minimapCam.orthographicSize - scroll * zoomSpeed * 10f,
            minZoom, maxZoom
        );
        minimapCamera.orthographicSize = minimapCam.orthographicSize;
    }
 
    void HandleToggle()
    {
        if (!Input.GetKeyDown(toggleKey)) return;
        isVisible = !isVisible;
        // Ẩn/hiện toàn bộ Panel cha của RawImage
        minimapDisplay.transform.parent.gameObject.SetActive(isVisible);
    }
 
    void OnDestroy()
    {
        if (renderTexture == null) return;
        minimapCamera.targetTexture = null;
        renderTexture.Release();
        Destroy(renderTexture);
    }
}
