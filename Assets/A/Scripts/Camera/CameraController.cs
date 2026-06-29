using UnityEngine;
using static UnityEditor.SceneView;

public class CameraController : MonoBehaviour
{
    private LineOfSight los;
    private CameraModel model;

    [Header("Referencias")]
    [SerializeField] GameObject player;
    [SerializeField] GameObject indicator;

    [Header("Alert")]
    [SerializeField] private float alertRadius = 15f;
    [SerializeField] private LayerMask enemyLayer;

    private bool hasAlerted = false;

    private void Awake()
    {
        los = GetComponent<LineOfSight>();
        model = GetComponent<CameraModel>();
    }

    void Update()
    {
        bool canSeePlayer = los.CheckRange(transform, player.transform) && los.CheckAngle(transform, player.transform) && !los.CheckObstacles(transform, player.transform);

        if (canSeePlayer)
        {
            indicator.SetActive(true);

            if (model != null)
            {
                model.StartTracking(player.transform);
            }
                

            if (!hasAlerted)
            {
                hasAlerted = true;
                AlertNearbyEnemies(player.transform.position);
            }
        }
        else
        {
            indicator.SetActive(false);
            hasAlerted = false;

            if (model != null)
                model.StopTracking();
        }
    }

    private void AlertNearbyEnemies(Vector3 targetPosition)
    {
        Collider[] enemiesInArea = Physics.OverlapSphere(transform.position, alertRadius, enemyLayer);

        foreach (Collider col in enemiesInArea)
        {
            Enemy2ControllerFSM enemy2 = col.GetComponentInParent<Enemy2ControllerFSM>();
            if (enemy2 != null)
            {
                enemy2.ReceiveCameraAlert(targetPosition);
                continue; 
            }

            EnemyControllerFSM enemy1 = col.GetComponentInParent<EnemyControllerFSM>();
            if (enemy1 != null)
            {
                enemy1.ReceiveCameraAlert(targetPosition);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alertRadius);
    }
}
