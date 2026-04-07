using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Combat/SkillData")]
public class SkillData : ScriptableObject

{
    [Header("Skill Info")]
    public string skillName;
    public string description;
    public Sprite skillIcon; // Sau này dùng làm UI
    public bool isBuffSkill;

    [Header("Combat Stats")]
    public float manaCost;
    public float cooldown;
    public float damageMultiplier = 1f; // Hệ số sát thương so với đánh thường

    [Header("Visual & Audio")]
    public GameObject vfxPrefab;     // Prefab của vệt chém/vụ nổ
    public AudioClip attackSound;    // Tiếng vung kiếm/kích hoạt
    public AudioClip hitSound;       // Tiếng khi chém trúng mục tiêu

    [Header("Animation")]
    public string animationTriggerName;


    public bool isProjectile;
    public float projectileSpeed = 10f;
    public float projectileDuration = 4f;
}

