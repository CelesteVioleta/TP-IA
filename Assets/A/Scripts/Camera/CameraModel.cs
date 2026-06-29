using UnityEngine;

public class CameraModel : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float minAngle = -45f;
    [SerializeField] private float maxAngle = 45f;

    [Header("Configuración de Seguimiento")]
    [SerializeField] private float trackingSpeed = 5f;

    private Transform targetToTrack;
    private float sweepTimer;

    public void StartTracking(Transform target)
    {
        targetToTrack = target;
    }

    public void StopTracking()
    {
        targetToTrack = null;
    }

    void Update()
    {
        if (targetToTrack != null)
        {
            Vector3 dir = targetToTrack.position - transform.position;
            
            if (dir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * trackingSpeed);
            }
        }
        else
        {
            sweepTimer += Time.deltaTime * (rotationSpeed / 100f);
            
            float angle = Mathf.Lerp(minAngle, maxAngle, Mathf.PingPong(sweepTimer, 1));
            Quaternion sweepRotation = Quaternion.Euler(0, angle, 0);

            transform.rotation = Quaternion.Lerp(transform.rotation, sweepRotation, Time.deltaTime * trackingSpeed);
        }
    }
}