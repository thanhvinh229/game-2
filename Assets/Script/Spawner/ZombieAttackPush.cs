using UnityEngine;

public class ZombieAttackPush : MonoBehaviour
{
    public int damage = 15;
    public float attackCooldown = 1.5f;
    public float pushForce = 6f;

    private float lastAttackTime;

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;

        // DAMAGE
        PlayerHealth hp = other.GetComponent<PlayerHealth>();
        hp?.TakeDamage(damage);

        // KNOCKBACK
        PlayerKnockBack knockback = other.GetComponent<PlayerKnockBack>();
        

        if (knockback != null)
        {
            Vector3 dir =
                other.transform.position - transform.position;
            dir.y = 0; // không hất lên trời
            knockback.Knockback(dir, pushForce);
        }

        Debug.Log(" push (CharacterController)");
    }
}
