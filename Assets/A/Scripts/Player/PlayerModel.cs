using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    Rigidbody rb;

    [Header("Variables")]
    [SerializeField] private int speed;
    [SerializeField] private int speedRotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void Walk(Vector3 dir)
    {
        rb.linearVelocity = dir * speed;
    }

    public void Rotate(Vector3 dir)
    {
        transform.forward = Vector3.Lerp(transform.forward, dir, speedRotation * Time.deltaTime);
    }
}
