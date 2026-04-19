using UnityEngine;

public class CameraModel : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float minAngle = -45f;
    [SerializeField] private float maxAngle = 45f;

    void Update()
    {
        //transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        float angle = Mathf.Lerp(minAngle, maxAngle, Mathf.PingPong(Time.time * rotationSpeed / 100f, 1));
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }
}
