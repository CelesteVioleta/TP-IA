using System.Collections;
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

    private EnemyView2 view;
    private Rigidbody rb;

    [Header("Variables")]
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float productionTime;
    [SerializeField] private float evadeDistance;

    [Header("Patrol")]
    [SerializeField] private int currentPatrolIndex = 0;   
    [SerializeField] private float patrolPointReachDistance;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    [Header("Investigate")]
    [SerializeField] private float investigateWaitTime = 2f;

    private Vector3 lastKnownPlayerPosition;
    private bool hasLastKnownPosition = false;

    private bool isInvestigating = false;
    private float investigateTimer = 0f;

    [Header("Attack")]
    [SerializeField] private float attackDuration = 1.2f;

    private bool isAttacking = false;
    private float attackTimer = 0f;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        los = GetComponent<LineOfSight>();
        playerRb = player.GetComponent<Rigidbody>();
        view = GetComponent<EnemyView2>();
    }


    void Update()
    {
        if (isAttacking)
        {
            Attack();
            return;
        }
        bool canSeePlayer = los.CheckRange(transform, player.transform) && los.CheckAngle(transform, player.transform) && !los.CheckObstacles(transform, player.transform);
        bool isClose = Vector3.Distance(transform.position, player.position) <= evadeDistance;

        if (canSeePlayer)
        {
            lastKnownPlayerPosition = player.position;
            hasLastKnownPosition = true;
            isInvestigating = false; 
        }

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
        actions.Add(new ActionOption("Investigate", !canSeePlayer && hasLastKnownPosition ? 50f : 0f, Investigate));
        actions.Add(new ActionOption("Chase", canSeePlayer && !isClose ? 80f : 0f, Chase));
        actions.Add(new ActionOption("Attack", isClose ? 100f : 0f, Attack));

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

    private void Investigate()
    {
        Vector3 dir = lastKnownPlayerPosition - transform.position;
        dir.y = 0;

        float distance = dir.magnitude;

        if (distance > 0.2f && !isInvestigating)
        {
            Move(dir.normalized);
            return;
        }

        rb.linearVelocity = Vector3.zero;

        if (!isInvestigating)
        {
            isInvestigating = true;
            investigateTimer = investigateWaitTime;
        }

        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        investigateTimer -= Time.deltaTime;

        if (investigateTimer <= 0f)
        {
            isInvestigating = false;
            hasLastKnownPosition = false;
        }
    }

    private void Attack()
    {
        rb.linearVelocity = Vector3.zero;

        Vector3 dir = player.position - transform.position;
        dir.y = 0;

        if (dir != Vector3.zero)
        {
            transform.forward = Vector3.Lerp(transform.forward, dir.normalized, Time.deltaTime * rotationSpeed);
        }

        // 👉 SOLO entra una vez
        if (!isAttacking)
        {
            isAttacking = true;
            attackTimer = attackDuration;

            Debug.Log("EMPIEZA ATAQUE");
            view.PlayAttack();
        }

        // 👉 Corre el tiempo del ataque
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            isAttacking = false;
            Debug.Log("TERMINA ATAQUE");
        }
    }
}
