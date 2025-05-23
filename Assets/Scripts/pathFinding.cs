using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public Transform seeker, target;
    GridManager grid;

    public List<Node> currentPath = new();

    void Awake()
    {
        grid = GetComponent<GridManager>();
    }

    public void FindPath(Vector2 startPos, Vector2 targetPos)
    {
        if (grid == null)
        {
            Debug.LogError("GridManager not found!");
            return;
        }

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode == null || targetNode == null)
        {
            Debug.LogWarning("Start or target node is null");
            return;
        }

        if (!targetNode.walkable)
        {
            targetNode = FindClosestWalkableNode(targetNode);
        }

        List<Node> openSet = new() { startNode };
        HashSet<Node> closedSet = new();

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost ||
                    openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;

                int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    Node FindClosestWalkableNode(Node targetNode)
    {
        List<Node> neighbors = grid.GetNeighbours(targetNode);
        Node closestNode = null;
        float closestDistance = float.MaxValue;

        foreach (Node node in neighbors)
        {
            if (node.walkable)
            {
                float distance = Vector2.Distance(node.worldPosition, targetNode.worldPosition);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestNode = node;
                }
            }
        }

        return closestNode ?? targetNode; 
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
            if (currentNode == null)
            {
                Debug.LogWarning("Path broken - parent node is null");
                break;
            }
        }
        path.Reverse();

        currentPath = path;

        Debug.Log("Chemin trouvé avec " + currentPath.Count + " points.");
    }

    int GetDistance(Node a, Node b)
    {
        int dstX = Mathf.Abs(a.gridX - b.gridX);
        int dstY = Mathf.Abs(a.gridY - b.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}