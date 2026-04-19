using UnityEngine;

public class EnemyController2 : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform player;
    [SerializeField] private LineOfSight los;
    [SerializeField] private FSMClasses fsm;

    [Header("Variables")]
    [SerializeField] private float speed;
    [SerializeField] private float patrolRotationSpeed;
    [SerializeField] private float chaseRotationSpeed;

    private void Awake()
    {
        los = GetComponent<LineOfSight>();
        fsm = GetComponent<FSMClasses>();
    }

    private void Update()
    {
        bool canSeePlayer = los.CheckRange(transform, player.transform) && los.CheckAngle(transform, player.transform) && !los.CheckObstacles(transform, player.transform);
        fsm.UpdateState(canSeePlayer);

        ExcecuteState();
    }

    public void ExcecuteState()
    {
        if (fsm.currentState is PatrolState) 
        {
            Patrol();
        }
        else if (fsm.currentState is ChaseState)
        {
            Chase();
        }
        else if (fsm.currentState is AttackState)
        {
            Attack();
        }
    }

    public void Patrol()
    {
        transform.Rotate(Vector3.up * patrolRotationSpeed * Time.deltaTime);
    }

    public void Chase()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0;

        Vector3 moveDir = dir.normalized;

        transform.position += moveDir * speed * Time.deltaTime;
        transform.forward = Vector3.Lerp(transform.forward, moveDir, Time.deltaTime * chaseRotationSpeed);
    }

    public void Attack()
    {
        GetComponent<Renderer>().material.color = Color.red;
    }
    
    void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Attack();
            }
        }
}
