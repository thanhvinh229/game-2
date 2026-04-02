using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 20f;
    public float speed = 10f; // Tốc độ bay của vệt chém
    public float lifetime = 5f; // Thời gian tồn tại trước khi biến mất
    public GameObject hitEffect; // Hiệu ứng tung tóe khi trúng quái (nếu có)
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Cho vệt chém bay về phía trước (theo hướng trục Z cục bộ của nó)
        rb.linearVelocity = transform.forward * speed;

        // Tự động hủy vệt chém sau khoảng thời gian lifetime
        Destroy(gameObject, lifetime);
    }

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
