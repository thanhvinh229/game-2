using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 20f;
    public GameObject hitEffect; // Hiệu ứng tung tóe khi trúng quái (nếu có)

    private void OnTriggerEnter(Collider foreign)
    {
        // Kiểm tra xem vật thể chạm phải có thể nhận sát thương không
        IDamageable target = foreign.GetComponent<IDamageable>();

        if (target != null)
        {
            target.TakeDamage(damage);

            // Tạo hiệu ứng trúng đòn tại điểm va chạm
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, transform.rotation);
            }

            
        }
    }
}
