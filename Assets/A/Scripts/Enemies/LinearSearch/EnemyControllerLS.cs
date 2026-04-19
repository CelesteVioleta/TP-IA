using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class EnemyControllerLS : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform player;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] LineOfSight los;
    [SerializeField] List<PatrolPoint> patrolPoints;

    [Header("Variables")]
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float productionTime;
    [SerializeField] private float evadeDistance;

    private Rigidbody rb;

    [Header("Patrol")]
    [SerializeField] private int currentPatrolIndex = 0;   
    [SerializeField] private float patrolPointReachDistance;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        los = GetComponent<LineOfSight>();
        playerRb = player.GetComponent<Rigidbody>();
    }


    void Update()
    {
        bool canSeePlayer = los.CheckRange(transform, player.transform) && los.CheckAngle(transform, player.transform) && !los.CheckObstacles(transform, player.transform);
        bool isClose = Vector3.Distance(transform.position, player.position) <= evadeDistance;

        List<ActionOption> actionOptions = BuildAction(canSeePlayer, isClose);

        ActionOption bestAction = SelectBestAction(actionOptions);
        bestAction.action?.Invoke();
    }

    private ActionOption SelectBestAction(List<ActionOption> actions)
    {
        ActionOption best = null;
        float bestScore = float.MinValue;

        foreach (var action in actions)
        {
            if (action.score > bestScore)
            {
                bestScore = action.score;
                best = action;
            }
        }

        return best;
    }

    private List<ActionOption> BuildAction(bool canSeePlayer, bool isClose)
    {
        List<ActionOption> actions = new List<ActionOption>();

        actions.Add(new ActionOption("Patrol", canSeePlayer ? 5f : 30f, Patrol));
        actions.Add(new ActionOption("Chase", canSeePlayer && !isClose ? 80f : 0f, Chase));

        return actions;
    }

    private void Move(Vector3 dir)
    {
        Vector3 velocity = dir * speed;
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;

        if (dir != Vector3.zero)
        {
            transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * rotationSpeed); 
        }
    }

    private void Chase()
    {
        Vector3 dir = SteeringBehavior.Chase(transform, player, playerRb, 1f);

        Move(dir);
    }

    private void Evade()
    {
        Vector3 dir = SteeringBehavior.Evade(transform, player, playerRb, 1f);

        Move(dir);
    }

    private void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        PatrolPoint targetPoint = patrolPoints[currentPatrolIndex];
        Transform target = targetPoint.point;

        if (isWaiting)
        {
            rb.linearVelocity = Vector3.zero;
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
            {
                isWaiting = false;
                currentPatrolIndex++;

                if (currentPatrolIndex >= patrolPoints.Count)
                    currentPatrolIndex = 0;
            }

            return;
        }

        Vector3 dir = target.position - transform.position;
        dir.y = 0f;

        if (dir.magnitude < patrolPointReachDistance)
        {
            isWaiting = true;
            waitTimer = targetPoint.waitTime;
            rb.linearVelocity = Vector3.zero;

            return;
        }

        Move(dir.normalized);
    }
}
