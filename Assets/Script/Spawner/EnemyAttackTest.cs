using UnityEngine;

public class EnemyAttackTest : MonoBehaviour
{
    [Header("Attack Settings")]
    public float damagePerHit = 10f;
    public float attackRate = 1f; // Số giây giữa mỗi lần đánh
    private float nextAttackTime = 0f;

    void Update()
    {
        // Giả lập tấn công khi nhấn Chuột Trái
        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime)
        {
            SimulateAttack();
            nextAttackTime = Time.time + attackRate; // Đặt thời gian cho lần đánh sau
        }
    }

    void SimulateAttack()
    {
        // Sử dụng Raycast để "bắn" một tia từ chuột vào thế giới game
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Kiểm tra xem vật thể bị bắn trúng có script IDamageable không
            // (Nó không cần biết đây là PlayerStats, chỉ cần biết nó có thể bị thương)
            IDamageable damageableTarget = hit.collider.GetComponent<IDamageable>();

            if (damageableTarget != null)
            {
                // Gây sát thương!
                Debug.Log("Quái tấn công trúng mục tiêu! Gây " + damagePerHit + " damage.");
                damageableTarget.TakeDamage(damagePerHit);
            }
            else
            {
                Debug.Log("Bắn hụt! Vật thể không thể bị thương.");
            }
        }
    }
}
