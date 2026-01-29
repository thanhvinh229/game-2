using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float detectRange = 15f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1.2f;
    public int damage = 10;

    private Transform player;
    private NavMeshAgent agent;
    private float attackTimer;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectRange)
        {
            agent.SetDestination(player.position);

            if (distance <= attackRange)
            {
                Attack();
            }
        }
    }

    void Attack()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;
            Debug.Log("get hit");
        }
    }
}