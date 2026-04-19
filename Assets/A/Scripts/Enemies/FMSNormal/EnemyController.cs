using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform player;
    [SerializeField] private LineOfSight los;
    [SerializeField] private FSM fsm;

    [Header("Variables")]
    [SerializeField] private float speed;
    [SerializeField] private float patrolRotationSpeed;
    [SerializeField] private float chaseRotationSpeed;

    private void Awake()
    {
        los = GetComponent<LineOfSight>();
        fsm = GetComponent<FSM>();
    }

    private void Update()
    {
        bool canSeePlayer = los.CheckRange(transform, player.transform) && los.CheckAngle(transform, player.transform) && !los.CheckObstacles(transform, player.transform);
        fsm.UpdateState(canSeePlayer);

        ExcecuteState();
    }

    public void ExcecuteState()
    {
        switch (fsm.currentState)
        {
            case FSM.EnemyState.Patrol:
                Patrol();
                break;

            case FSM.EnemyState.Chase:
                Chase();
                break;
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
}
