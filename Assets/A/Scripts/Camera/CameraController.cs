using UnityEngine;

public class CameraController : MonoBehaviour
{
    LineOfSight los;

    [Header("Referencias")]
    [SerializeField] GameObject player;
    [SerializeField] GameObject indicator;

    private void Awake()
    {
        los = GetComponent<LineOfSight>();
    }

    void Update()
    {
        if (los.CheckRange(transform, player.transform) && los.CheckAngle(transform, player.transform) && !los.CheckObstacles(transform, player.transform))
        {
            indicator.SetActive(true);
        }
        else
        {
            indicator.SetActive(false);
        }
    }
}
