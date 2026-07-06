using System.Collections.Generic;
using UnityEngine;

public class Theta : MonoBehaviour
{
    private GridManager grid;

    private void Awake()
    {
        grid = FindFirstObjectByType<GridManager>();
    }
    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {


        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        foreach (Node node in grid.Grid)
        {
            node.gCost = int.MaxValue;
            node.hCost = 0;
            node.parent = null;
        }

        startNode.gCost = 0;


        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost ||
                   (openSet[i].fCost == currentNode.fCost &&
                    openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }
            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;

                UpdateVertex(currentNode, neighbour, targetNode, openSet);

            }
        }
        return null;
    }
    private int GetDistance(Node a, Node b)
    {
        int dstX = Mathf.Abs(a.gridX - b.gridX);
        int dstY = Mathf.Abs(a.gridY - b.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);

        return 14 * dstX + 10 * (dstY - dstX);
    }

    private bool LineOfSight(Node from, Node to)
    {
        Vector3 start = from.worldPosition + Vector3.up * 0.5f;
        Vector3 end = to.worldPosition + Vector3.up * 0.5f;

        Vector3 direction = end - start;
        float distance = direction.magnitude;

        return !Physics.Raycast( start, direction.normalized,distance, grid.ObstacleMask);
    }

    private void UpdateVertex(Node currentNode, Node neighbour, Node targetNode, List<Node> openSet)
    {
        Node bestParent = currentNode;
        int newCost;

        if (currentNode.parent != null &&
            LineOfSight(currentNode.parent, neighbour))
        {
            bestParent = currentNode.parent;

            newCost = bestParent.gCost + GetDistance(bestParent, neighbour);
        }
        else
        {
            newCost = currentNode.gCost + GetDistance(currentNode, neighbour);
        }

        if (newCost < neighbour.gCost || !openSet.Contains(neighbour))
        {
            neighbour.gCost = newCost;
            neighbour.hCost = GetDistance(neighbour, targetNode);

            neighbour.parent = bestParent;

            if (!openSet.Contains(neighbour))
                openSet.Add(neighbour);
        }
    }
    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();

        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        return path;
    }

}




