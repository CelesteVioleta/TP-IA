using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> neighbours = new();

    private void Awake()
    {
        neighbours.Clear();

        SearchNeighbour(Vector3.forward);
        SearchNeighbour(Vector3.back);
        SearchNeighbour(Vector3.left);
        SearchNeighbour(Vector3.right);
    }

    void SearchNeighbour(Vector3 direction)
    {
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, 2f))
        {
            Node node = hit.collider.GetComponent<Node>();

            if (node != null)
                neighbours.Add(node);
        }
    }
}