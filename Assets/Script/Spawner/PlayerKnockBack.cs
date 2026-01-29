using UnityEngine;

public class PlayerKnockBack : MonoBehaviour
{
    public float knockbackTime = 0.25f;

    private CharacterController controller;
    private Vector3 knockbackVelocity;
    private float timer;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    public void Knockback(Vector3 direction, float force)
    {
        knockbackVelocity = direction.normalized * force;
        timer = knockbackTime;
    }

    void Update()
    {
        if (timer > 0)
        {
            controller.Move(knockbackVelocity * Time.deltaTime);
            timer -= Time.deltaTime;
        }
    }
}
