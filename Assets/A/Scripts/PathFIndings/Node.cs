using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> neighbours = new List<Node>();

    private void Start()
    {
        // Busca vecinos en las 4 direcciones.
        GetNeighbour(Vector3.right);
        GetNeighbour(Vector3.left);
        GetNeighbour(Vector3.forward);
        GetNeighbour(Vector3.back);
    }

    void GetNeighbour(Vector3 dir)
    {
        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, 2.2f))
        {
            var node = hit.collider.GetComponent<Node>();

            if (node != null && !neighbours.Contains(node))
            {
                neighbours.Add(node);
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (neighbours == null) return;

        Gizmos.color = Color.green;
        foreach (Node neighbour in neighbours)
        {
            if (neighbour != null)
            {
                Gizmos.DrawLine(transform.position, neighbour.transform.position);
            }
        }
    }
}