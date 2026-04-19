using UnityEngine;

public class EnemyController3 : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform player;
    [SerializeField] private LineOfSight los;
    [SerializeField] private EnemyDecisionTree decisionTree;
    private EnemyContext context;

    [Header("Variables")]
    [SerializeField] private float speed;
    [SerializeField] private float patrolRotationSpeed;
    [SerializeField] private float chaseRotationSpeed;

    private void Awake()
    {
        los = GetComponent<LineOfSight>();
        decisionTree = GetComponent<EnemyDecisionTree>();
        context = new EnemyContext { self = transform, player = player, los = los };
    }

    public void Update()
    {
        context.player = player;
        decisionTree.Evaluate(this, context);
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
