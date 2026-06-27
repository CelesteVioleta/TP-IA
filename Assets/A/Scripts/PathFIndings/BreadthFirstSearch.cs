using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BreadthFirstSearch
{
    public static List<Node> Run(Node initialNode, Func<Node, bool> isSatified, Func<Node, List<Node>> getConnections)
    {
        Queue<Node> pending = new Queue<Node>();
        HashSet<Node> visited = new HashSet<Node>();
        Dictionary<Node, Node> parents = new Dictionary<Node, Node>();

        pending.Enqueue(initialNode);
        visited.Add(initialNode);

        while (pending.Count > 0)
        {
            Node node = pending.Dequeue();

            if (isSatified(node))
            {
                // Encuentra el nodo, hace la lista de todo el camino hacia el nodo y devuelve esa lista invertida.
                List<Node> path = new List<Node>();
                Node current = node;

                while(parents.ContainsKey(current))
                {
                    path.Add(current);
                    current = parents[current];
                }

                path.Reverse();
                return path;
            }
            else
            {
                List<Node> children = getConnections(node);

                for (int i = 0; i < children.Count; i++)
                {
                    if (visited.Contains(children[i])) continue;
                    pending.Enqueue(children[i]);
                    visited.Add(children[i]);
                    parents[children[i]] = node;
                }
            }
        }

        return new List<Node>();
    }
}
