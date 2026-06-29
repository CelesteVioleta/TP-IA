using UnityEngine;

public class PathfindingManager : MonoBehaviour
{
    public static PathfindingManager Instance;
    private Node[] allNodes;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        allNodes = FindObjectsOfType<Node>();
    }

    // Recibe una posición en el nivel y devuelve el nodo más cercano a esa posición.
    public Node GetClosestNode(Vector3 position)
    {
        Node closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Node node in allNodes)
        {
            float dist = Vector3.Distance(position, node.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = node;
            }
        }
        return closest;
    }
}
