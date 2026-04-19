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
    [SerializeField] private float investigationRotationSpeed;

    [Header("Patrol")]
    [SerializeField] private PatrolPoint[] patrolPoints;

    private int currentPointIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    private Vector3 lastKnownPlayerPosition;

    private void Awake()
    {
        los = GetComponent<LineOfSight>();
        fsm = GetComponent<FSMClasses>();
    }

    private void Update()
    {
        bool canSeePlayer = los.CheckRange(transform, player.transform) && los.CheckAngle(transform, player.transform) && !los.CheckObstacles(transform, player.transform);
        fsm.UpdateState(canSeePlayer);

        if (canSeePlayer)
        {
            lastKnownPlayerPosition = player.position;
        }

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
        else if (fsm.currentState is InvestigateState)
        {
            Investigate();
        }
        else if (fsm.currentState is AttackState)
        {
            Attack();
        }
    }

    public void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        PatrolPoint currentPoint = patrolPoints[currentPointIndex];

        Vector3 dir = currentPoint.point.position - transform.position;
        dir.y = 0;

        float distance = dir.magnitude;

        if (distance < 0.2f)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                waitTimer = currentPoint.waitTime;
            }

            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
            {
                isWaiting = false;
                currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
            }

            return;
        }

        Vector3 moveDir = dir.normalized;
        transform.position += moveDir * speed * Time.deltaTime;

        transform.forward = Vector3.Lerp(transform.forward, moveDir, Time.deltaTime * patrolRotationSpeed);
    }

    public void Chase()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0;

        Vector3 moveDir = dir.normalized;

        transform.position += moveDir * speed * Time.deltaTime;
        transform.forward = Vector3.Lerp(transform.forward, moveDir, Time.deltaTime * chaseRotationSpeed);
    }

    public void Investigate()
    {
        Vector3 dir = lastKnownPlayerPosition - transform.position;
        dir.y = 0;

        float distance = dir.magnitude;

        if (distance > 0.2f)
        {
            Vector3 moveDir = dir.normalized;
            transform.position += moveDir * speed * Time.deltaTime;

            transform.forward = Vector3.Lerp(transform.forward, moveDir, Time.deltaTime * investigationRotationSpeed);
        }
        else
        {
            transform.Rotate(Vector3.up * investigationRotationSpeed * Time.deltaTime);
        }
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
