using System.Collections.Generic;
using UnityEngine;

public class Enemy2ControllerFSM : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject indicator;

    private LineOfSight los;
    private FSM2 fsm;
    private Rigidbody rb;
    private Rigidbody playerRb;
    private Enemy2View view;

    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Patrol")]
    [SerializeField] private List<PatrolPoint> patrolPoints;

    private int currentPatrolIndex;
    private float waitTimer;
    private bool isWaiting;

    [Header("Investigate")]
    [SerializeField] private float investigateWaitTime = 2f;
    [SerializeField] private float investigationRotationSpeed = 120f;

    private bool isInvestigating;
    private bool investigationFinished;
    private float investigateTimer;

    private bool hasLastKnownPosition;
    private Vector3 lastKnownPlayerPosition;

    [Header("Attack")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float shootCooldown = 1f;

    private float nextShootTime;

    [Header("Evade")]
    [SerializeField] private float evadeDistance = 3f;

    [Header("Pathfinding")]
    private List<Vector3> currentWaypoints = new List<Vector3>();
    private int currentPathIndex = 0;
    private float waypointReachDistance = 0.5f;

    private Node targetNode;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        los = GetComponent<LineOfSight>();
        fsm = GetComponent<FSM2>();
        view = GetComponent<Enemy2View>();

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
            <= evadeDistance;

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
            case FSM2.EnemyState.Patrol:
                Patrol();
                break;
            case FSM2.EnemyState.Attack:
                Attack();
                break;
            case FSM2.EnemyState.Investigate:
                Investigate();
                break;
            case FSM2.EnemyState.Evade:
                Evade();
                break;
        }
    }

    // PATHFINDING (DIJKSTRA)
    private void SetPathDijkstra(Vector3 targetPosition)
    {
        Node startNode = PathfindingManager.Instance.GetClosestNode(transform.position);
        targetNode = PathfindingManager.Instance.GetClosestNode(targetPosition);

        currentWaypoints.Clear();

        if (startNode != null && targetNode != null)
        {
            List<Node> path = Dijkstra.Run(startNode, IsSatisfied, GetConnections, GetCosts);

            for (int i = 0; i < path.Count; i++)
            {
                currentWaypoints.Add(path[i].transform.position);
            }

            currentPathIndex = 0;
        }
    }

    public bool IsSatisfied(Node node)
    {
        return node == targetNode;
    }

    public List<Node> GetConnections(Node node)
    {
        return node.neighbours;
    }

    public float GetCosts(Node node1, Node node2)
    {
        float costs = Vector3.Distance(node1.transform.position, node2.transform.position);

        return costs;
    }

    // MOVEMENT

    // Devuelve TRUE si ya llegó al último waypoint de la lista
    private bool FinishedPath(float speed)
    {
        if (currentWaypoints.Count == 0 || currentPathIndex >= currentWaypoints.Count)
            return true;

        Vector3 currentTarget = currentWaypoints[currentPathIndex];
        Vector3 offsetToNode = currentTarget - transform.position;
        offsetToNode.y = 0;

        if (offsetToNode.magnitude < waypointReachDistance)
        {
            currentPathIndex++;
            if (currentPathIndex >= currentWaypoints.Count)
            {
                currentWaypoints.Clear();
                return true;
            }
        }
        else
        {
            Vector3 steerDir = SteeringBehavior.Seek(transform, currentTarget);
            Move(steerDir, speed);
        }

        return false;
    }

    private void Move(Vector3 dir, float speed)
    {
        Vector3 velocity = dir * speed;
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;

        if (dir != Vector3.zero)
        {
            transform.forward = Vector3.Lerp(
                transform.forward,
                dir.normalized,
                Time.deltaTime * rotationSpeed);
        }
    }

    // STATES
    private void Patrol()
    {
        investigationFinished = false;
        if (patrolPoints.Count == 0) return;

        if (isWaiting)
        {
            rb.linearVelocity = Vector3.zero;
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0)
            {
                isWaiting = false;
                currentPatrolIndex++;
                if (currentPatrolIndex >= patrolPoints.Count) currentPatrolIndex = 0;
            }
            return;
        }

        PatrolPoint targetPoint = patrolPoints[currentPatrolIndex];

        if (currentWaypoints.Count == 0)
        {
            SetPathDijkstra(targetPoint.point.position);

            if (currentWaypoints.Count == 0)
            {
                Debug.LogWarning($"No hay ruta hacia el PatrolPoint {currentPatrolIndex}. Saltando al siguiente.");
                isWaiting = true;
                waitTimer = 1f;
                return;
            }
        }

        bool hasReachedDestination = FinishedPath(patrolSpeed);

        if (hasReachedDestination)
        {
            isWaiting = true;
            waitTimer = targetPoint.waitTime;
        }
    }

    private void Attack()
    {
        rb.linearVelocity = Vector3.zero;

        Vector3 dir = player.position - transform.position;
        dir.y = 0f;

        if (dir != Vector3.zero)
        {
            transform.forward = Vector3.Lerp(transform.forward, dir.normalized, Time.deltaTime * rotationSpeed);
        }

        if (Time.time >= nextShootTime)
        {
            Shoot();
            view.PlayAttack();
            nextShootTime = Time.time + shootCooldown;
        }
    }

    private void Shoot()
    {
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    }

    private void Investigate()
    {
        if (!isInvestigating && currentWaypoints.Count == 0)
        {
            SetPathDijkstra(lastKnownPlayerPosition);
        }

        if (currentWaypoints.Count > 0)
        {
            bool hasReachedDestination = FinishedPath(patrolSpeed);

            if (hasReachedDestination)
            {
                isInvestigating = true;
                investigateTimer = investigateWaitTime;
            }
            return;
        }

        rb.linearVelocity = Vector3.zero;

        if (!isInvestigating)
        {
            isInvestigating = true;
            investigateTimer = investigateWaitTime;
        }

        transform.Rotate(0, investigationRotationSpeed * Time.deltaTime, 0);
        investigateTimer -= Time.deltaTime;

        if (investigateTimer <= 0)
        {
            isInvestigating = false;
            hasLastKnownPosition = false;
            investigationFinished = true;
            currentWaypoints.Clear();
        }
    }

    public void ReceiveCameraAlert(Vector3 alertPosition)
    {
        if (fsm.currentState == FSM2.EnemyState.Attack || fsm.currentState == FSM2.EnemyState.Evade)
            return;

        lastKnownPlayerPosition = alertPosition;

        hasLastKnownPosition = true;
        investigationFinished = false;

        currentWaypoints.Clear();
    }

    private void Evade()
    {
        currentWaypoints.Clear();
        rb.linearVelocity = Vector3.zero;

        Vector3 dir = SteeringBehavior.Evade(transform, player, playerRb, 1f);
        Move(dir, chaseSpeed);
    }
}