using System.Collections.Generic;
using UnityEngine;

public class EnemyControllerFSM : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject indicator;

    private LineOfSight los;
    private FSM fsm;
    private Rigidbody rb;
    private Rigidbody playerRb;
    private EnemyView view;

    [SerializeField] private List<PatrolPoint> patrolPoints;

    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Patrol")]
    [SerializeField] private float patrolPointReachDistance = 1f;

    private int currentPatrolIndex;
    private float waitTimer;
    private bool isWaiting;

    [Header("Investigate")]
    [SerializeField] private float investigateWaitTime = 2f;
    [SerializeField] private float investigationRotationSpeed = 120f;

    private bool isInvestigating;
    private bool investigationFinished;
    private float investigateTimer;

    private Vector3 lastKnownPlayerPosition;
    private bool hasLastKnownPosition;

    [Header("Attack")]
    [SerializeField] private float attackDistance = 2f;
    [SerializeField] private float attackDuration = 1.2f;

    private float attackTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        los = GetComponent<LineOfSight>();
        fsm = GetComponent<FSM>();
        view = GetComponent<EnemyView>();

        playerRb = player.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        bool canSeePlayer =
            los.CheckRange(transform, player) &&
            los.CheckAngle(transform, player) &&
            !los.CheckObstacles(transform, player);

        bool isClose =
            Vector3.Distance(transform.position, player.position)
            <= attackDistance;

        if (canSeePlayer)
        {
            lastKnownPlayerPosition = player.position;
            hasLastKnownPosition = true;

            indicator.SetActive(true);
        }
        else
        {
            indicator.SetActive(false);
        }

        fsm.UpdateState(
            canSeePlayer,
            isClose,
            hasLastKnownPosition,
            investigationFinished);

        ExecuteState();
    }

    private void ExecuteState()
    {
        switch (fsm.currentState)
        {
            case FSM.EnemyState.Patrol:
                Patrol();
                break;

            case FSM.EnemyState.Chase:
                Chase();
                break;

            case FSM.EnemyState.Investigate:
                Investigate();
                break;

            case FSM.EnemyState.Attack:
                Attack();
                break;
        }
    }

    private void Move(Vector3 dir, float speed)
    {
        Vector3 velocity = dir * speed;
        velocity.y = rb.linearVelocity.y;

        rb.linearVelocity = velocity;

        if (dir != Vector3.zero)
        {
            transform.forward =
                Vector3.Lerp(
                    transform.forward,
                    dir.normalized,
                    Time.deltaTime * rotationSpeed);
        }
    }

    private void Patrol()
    {
        investigationFinished = false;

        if (patrolPoints.Count == 0)
            return;

        PatrolPoint point = patrolPoints[currentPatrolIndex];

        if (isWaiting)
        {
            rb.linearVelocity = Vector3.zero;

            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0)
            {
                isWaiting = false;

                currentPatrolIndex++;

                if (currentPatrolIndex >= patrolPoints.Count)
                    currentPatrolIndex = 0;
            }

            return;
        }

        Vector3 dir =
            point.point.position - transform.position;

        dir.y = 0;

        if (dir.magnitude < patrolPointReachDistance)
        {
            isWaiting = true;
            waitTimer = point.waitTime;

            return;
        }

        Move(dir.normalized, patrolSpeed);
    }

    private void Chase()
    {
        investigationFinished = false;

        Vector3 dir =
            SteeringBehavior.Chase(
                transform,
                player,
                playerRb,
                1f);

        Move(dir, chaseSpeed);
    }

    private void Investigate()
    {
        Vector3 dir =
            lastKnownPlayerPosition - transform.position;

        dir.y = 0;

        if (dir.magnitude > 0.2f && !isInvestigating)
        {
            Move(dir.normalized, patrolSpeed);
            return;
        }

        rb.linearVelocity = Vector3.zero;

        if (!isInvestigating)
        {
            isInvestigating = true;
            investigateTimer = investigateWaitTime;
        }

        transform.Rotate(
            0,
            investigationRotationSpeed * Time.deltaTime,
            0);

        investigateTimer -= Time.deltaTime;

        if (investigateTimer <= 0)
        {
            isInvestigating = false;
            hasLastKnownPosition = false;
            investigationFinished = true;
        }
    }

    private void Attack()
    {
        rb.linearVelocity = Vector3.zero;

        Vector3 dir =
            player.position - transform.position;

        dir.y = 0;

        if (dir != Vector3.zero)
        {
            transform.forward =
                Vector3.Lerp(
                    transform.forward,
                    dir.normalized,
                    Time.deltaTime * rotationSpeed);
        }

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0)
        {
            attackTimer = attackDuration;

            if (view != null)
                view.PlayAttack();
        }
    }
}