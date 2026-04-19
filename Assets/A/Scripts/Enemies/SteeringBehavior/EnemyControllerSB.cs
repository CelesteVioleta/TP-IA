using UnityEngine;

public class EnemyControllerSB : MonoBehaviour
{
    public enum Mode
    {
        Seek,
        Flee,
        Arrive,
        Chase,
        Evade,
        Wander
    }

    [SerializeField] private Mode mode;

    [SerializeField] private Transform player;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float arriveRadius;
    [SerializeField] private float wanderAngleDirection;
    [SerializeField] private float wanderChangeInerval;

    private Vector3 wanderDirection;
    private float wanderTime;

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        wanderDirection = transform.forward;
        wanderTime = 0f;
    }

    private void Update()
    {
        Vector3 dir = Vector3.zero;

        switch (mode)
        {
            case Mode.Seek:
                dir = SteeringBehavior.Seek(transform, player.position);
                break;

            case Mode.Flee:
                dir = SteeringBehavior.Flee(transform, player.position);
                break;

            case Mode.Arrive:
                dir = SteeringBehavior.Arrive(transform, player.position, arriveRadius);
                break;
            case Mode.Chase:
                dir = SteeringBehavior.Chase(transform, player, playerRb, 1f);
                break;
            case Mode.Evade:
                dir = SteeringBehavior.Evade(transform, player, playerRb, 1f);
                break;
            case Mode.Wander:
                wanderTime -= Time.deltaTime;
                if (wanderTime < 0f)
                {
                    wanderDirection = SteeringBehavior.Wander(wanderDirection, wanderAngleDirection);
                    wanderTime = wanderChangeInerval;
                }
                dir = wanderDirection;
                break;
        }

        Move(dir);
    }

    private void Move(Vector3 dir)
    {
        transform.position += dir * speed * Time.deltaTime;

        if (dir != Vector3.zero) 
        {
            transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * rotationSpeed);
        }
    }
}
