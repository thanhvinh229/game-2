using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // INSPECTOR FIELDS
    // ─────────────────────────────────────────────
 
    [Header("── Bone Targets ──────────────────────")]
    [Tooltip("Bone bàn tay cầm kiếm  (vd: mixamorig:RightHand)")]
    public Transform handBone;
 
    [Tooltip("Bone cất kiếm khi không dùng (vd: mixamorig:Spine2 hoặc Hip)")]
    public Transform sheathBone;
 
    [Space]
    [Header("── Offset khi CẦM KIẾM ───────────────")]
    public Vector3 handPositionOffset   = Vector3.zero;
    public Vector3 handRotationOffset   = Vector3.zero;
 
    [Space]
    [Header("── Offset khi CẤT KIẾM ────────────────")]
    public Vector3 sheathPositionOffset = new Vector3(-0.15f, 0f, 0f);
    public Vector3 sheathRotationOffset = new Vector3(0f, 0f, 45f);
 
    [Space]
    [Header("── Tên bone (tự tìm nếu không assign) ─")]
    public string handBoneName   = "mixamorig:RightHand";
    public string sheathBoneName = "mixamorig:Spine2";
 
    [Space]
    [Header("── Trạng thái ban đầu ─────────────────")]
    public bool startEquipped = true;   // true = cầm tay, false = đã cất
 
    // ─────────────────────────────────────────────
    // RUNTIME
    // ─────────────────────────────────────────────
    public bool IsEquipped { get; private set; }
 
    // ─────────────────────────────────────────────
    // UNITY CALLBACKS
    // ─────────────────────────────────────────────
    void Awake()
    {
        // Tự tìm bone nếu chưa assign trong Inspector
        if (handBone == null)
        {
            GameObject go = GameObject.Find(handBoneName);
            if (go != null) handBone = go.transform;
            else Debug.LogWarning($"[WeaponHolder] Không tìm thấy hand bone '{handBoneName}'");
        }
 
        if (sheathBone == null)
        {
            GameObject go = GameObject.Find(sheathBoneName);
            if (go != null) sheathBone = go.transform;
            else Debug.LogWarning($"[WeaponHolder] Không tìm thấy sheath bone '{sheathBoneName}'");
        }
    }
 
    void Start()
    {
        // Gắn vào đúng vị trí ban đầu
        if (startEquipped)
            SnapToHand();
        else
            SnapToSheath();
    }
 
    // ─────────────────────────────────────────────
    // PUBLIC API — gọi từ PlayerController
    // ─────────────────────────────────────────────
 
    /// <summary>Rút kiếm ra — gắn vào tay với offset cầm kiếm</summary>
    public void DrawWeapon()
    {
        if (IsEquipped) return;
        SnapToHand();
    }
 
    /// <summary>Cất kiếm — gắn vào lưng/hông với offset cất kiếm</summary>
    public void SheathWeapon()
    {
        if (!IsEquipped) return;
        SnapToSheath();
    }
 
    /// <summary>Toggle rút/cất — tiện dùng với 1 phím</summary>
    public void ToggleWeapon()
    {
        if (IsEquipped) SheathWeapon();
        else            DrawWeapon();
    }
 
    // ─────────────────────────────────────────────
    // PRIVATE HELPERS
    // ─────────────────────────────────────────────
 
    void SnapToHand()
    {
        if (handBone == null) { Debug.LogError("[WeaponHolder] handBone chưa được set!"); return; }
 
        transform.SetParent(handBone, worldPositionStays: false);
        transform.localPosition    = handPositionOffset;
        transform.localEulerAngles = handRotationOffset;
        IsEquipped = true;
 
        Debug.Log("[WeaponHolder] Đã rút kiếm → tay phải");
    }
 
    void SnapToSheath()
    {
        if (sheathBone == null) { Debug.LogError("[WeaponHolder] sheathBone chưa được set!"); return; }
 
        transform.SetParent(sheathBone, worldPositionStays: false);
        transform.localPosition    = sheathPositionOffset;
        transform.localEulerAngles = sheathRotationOffset;
        IsEquipped = false;
 
        Debug.Log("[WeaponHolder] Đã cất kiếm → lưng");
    }
 
    // ─────────────────────────────────────────────
    // SCENE GIZMOS (chỉ hiện trong Editor)
    // ─────────────────────────────────────────────
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.04f);
        // Vẽ hướng lưỡi kiếm
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 0.5f);
    }
#endif
}
